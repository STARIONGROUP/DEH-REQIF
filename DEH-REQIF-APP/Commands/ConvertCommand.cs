//  -------------------------------------------------------------------------------------------------
//  <copyright file="ConvertCommand.cs" company="RHEA System S.A.">
// 
//    Copyright 2022 RHEA System S.A.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
//  </copyright>
//  -------------------------------------------------------------------------------------------------

namespace DEHReqIF.Console.Commands
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.DAL;

    using DEHReqIF.ExportSettings;
    using DEHReqIF.Services;

    using NLog;

    using ReqIFSharp;
    using ReqIFSharp.Extensions.Services;

    /// <summary>
    /// The purpose of the <see cref="ConvertCommand"/> is to convert an ECSS-E-TM-10-25 requirements set
    /// into a ReqIF document
    /// </summary>
    public class ConvertCommand : IConvertCommand
    {
        /// <summary>
        /// Holds the <see cref="ILogger"/> for this class
        /// </summary>
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The (injected) <see cref="IReqIFLoaderService"/> used to read a ReqIF document
        /// </summary>
        private readonly IReqIFLoaderService reqIfLoaderService;

        /// <summary>
        /// The (injected) <see cref="IExportSettingsReader"/> used to read export settings
        /// </summary>
        private readonly IExportSettingsReader exportSettingsReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertCommand"/>
        /// </summary>
        /// <param name="reqIfLoaderService">
        /// The (injected) <see cref="IReqIFLoaderService"/> used to read a ReqIF document
        /// </param>
        /// <param name="exportSettingsReader">
        /// The (injected) <see cref="IExportSettingsReader"/> used to read the export settings file
        /// </param>
        public ConvertCommand(IReqIFLoaderService reqIfLoaderService, IExportSettingsReader exportSettingsReader)
        {
            this.reqIfLoaderService = reqIfLoaderService;
            this.exportSettingsReader = exportSettingsReader;
        }

        /// <summary>
        /// Gets or sets the username that is used to connect to the ECSS-E-TM-10-25 <see cref="DataSource"/>
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password that is used to connect to the ECSS-E-TM-10-25 <see cref="DataSource"/>
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the ECSS-E-TM-10-25 source 
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// Gets or sets the path of the source ReqIF template (document)
        /// </summary>
        public string TemplateSource { get; set; }

        /// <summary>
        /// Gets or sets the path of the target ReqIF document
        /// </summary>
        public string TargetReqIF { get; set; }

        /// <summary>
        /// The location of the to be created reqif file
        /// </summary>
        private string TargetReqIFLocation => Path.ChangeExtension(this.TargetReqIF, ".reqif");

        /// <summary>
        /// The location of the to be created reqifz file
        /// </summary>
        private string TargetReqIFzLocation => Path.ChangeExtension(this.TargetReqIF, ".reqifz");

        /// <summary>
        /// The name of the entry in the reqifz (zip) file
        /// </summary>
        private string EntryName => new FileInfo(this.TargetReqIFLocation).Name;

        /// <summary>
        /// The ShortName of the EngineeringModelSetup
        /// </summary>
        public string EngineeringModelIid { get; set; }

        /// <summary>
        /// Gets or sets the location of the export settings file
        /// </summary>
        public string ExportSettings { get; set; } = "export-settings.json";

        /// <summary>
        /// Executes the <see cref="ConvertCommand"/>
        /// </summary>
        public async Task ExecuteAsync()
        {
            var sw = Stopwatch.StartNew();

            try
            {
                var session = await this.OpenSessionAndRetrieveData();
                var exportSettings = await this.exportSettingsReader.ReadFile(this.ExportSettings);
                var targetReqIf = await this.BuildReqIf(session, exportSettings);

                await this.CreateReqIfFiles(targetReqIf);

                sw.Stop();
                logger.Info("Conversion finished in {0}", sw.Elapsed.ToString("hh':'mm':'ss'.'fff"));
            }
            catch (Exception e)
            {
                logger.Fatal(e, "ECSS-E-TM-10-25 to ReqIF Conversion Error");
                Console.WriteLine($"Converting the ECSS-E-TM-10-25 data to ReqIF failed due to an error: {e.Message}");
                Console.WriteLine("Hit Enter to continue");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Build the ReqIf document
        /// </summary>
        /// <param name="session">The <see cref="ISession"/></param>
        /// <param name="exportSettings"></param>
        /// <returns>An awaitable <see cref="Task{T}"/> of type <see cref="ReqIF"/></returns>
        private async Task<ReqIF> BuildReqIf(ISession session, ExportSettings exportSettings)
        {
            var sw = Stopwatch.StartNew();

            await using var fileStream = new FileStream(this.TemplateSource, FileMode.Open);

            await this.reqIfLoaderService.Load(fileStream, new CancellationToken());

            var templateReqIF = this.reqIfLoaderService.ReqIFData.Single();

            logger.Info($"{this.TemplateSource} read in {sw.ElapsedMilliseconds} [ms]");
            sw.Restart();

            var builder = new ReqIFBuilder();
            var targetReqIf = builder.Build(templateReqIF, session.OpenIterations.First().Key.RequirementsSpecification, exportSettings);

            logger.Info($"Target ReqIf was built in {sw.ElapsedMilliseconds} [ms]");

            return targetReqIf;
        }

        /// <summary>
        /// Create the ReqIf files
        /// </summary>
        /// <param name="targetReqIf">The <see cref="ReqIF"/> document</param>
        /// <returns>an awaitable <see cref="Task"/></returns>
        private async Task CreateReqIfFiles(ReqIF targetReqIf)
        {
            var sw = Stopwatch.StartNew();

            if (System.IO.File.Exists(this.TargetReqIFLocation))
            {
                System.IO.File.Delete(this.TargetReqIFLocation);
            }

            await new ReqIFSerializer().SerializeAsync(targetReqIf, this.TargetReqIFLocation, new CancellationToken());

            if (System.IO.File.Exists(this.TargetReqIFzLocation))
            {
                System.IO.File.Delete(this.TargetReqIFzLocation);
            }

            using (var zipToOpen = new FileStream(this.TargetReqIFzLocation, FileMode.CreateNew))
            {
                using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    archive.CreateEntryFromFile(this.TargetReqIFLocation, this.EntryName);
                }
            }

            logger.Info($"ReqIf was created in {sw.ElapsedMilliseconds} [ms]");
        }

        /// <summary>
        /// Open the <see cref="Session"/> and retrieve the wanted data
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/> of type <see cref="ISession"/></returns>
        private async Task<ISession> OpenSessionAndRetrieveData()
        {
            var sw = Stopwatch.StartNew();

            var dal = new CDP4ServicesDal.CdpServicesDal();
            var credentials = new Credentials(this.Username, this.Password, new Uri(this.DataSource));

            var session = new Session(dal, credentials);
            await session.Open(false);

            var siteDirectory = session.RetrieveSiteDirectory();

            await session.Read(siteDirectory.SiteReferenceDataLibrary.First());

            var engineeringModelSetup = siteDirectory.Model.First(x => x.EngineeringModelIid == Guid.Parse(this.EngineeringModelIid));

            var iterationIid = engineeringModelSetup.IterationSetup.OrderByDescending(x => x.IterationNumber).First(x => !x.IsDeleted).IterationIid;

            var model = new EngineeringModel(engineeringModelSetup.EngineeringModelIid, session.Assembler.Cache, session.Credentials.Uri)
                { EngineeringModelSetup = engineeringModelSetup };

            var iteration = new Iteration(iterationIid, session.Assembler.Cache, session.Credentials.Uri);

            model.Iteration.Add(iteration);

            DomainOfExpertise initialDomain;

            if (session.ActivePerson.DefaultDomain != null && engineeringModelSetup.ActiveDomain.Contains(session.ActivePerson.DefaultDomain))
            {
                initialDomain = session.ActivePerson.DefaultDomain;
            }
            else
            {
                initialDomain = engineeringModelSetup.ActiveDomain.FirstOrDefault();
            }

            await session.Read(iteration, initialDomain);

            logger.Info($"Session was opened and data was read in {sw.ElapsedMilliseconds} [ms]");

            return session;
        }
    }
}
