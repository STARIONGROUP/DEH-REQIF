//  -------------------------------------------------------------------------------------------------
//  <copyright file="ConvertCommand.cs" company="Starion Group S.A.">
// 
//    Copyright 2022-2024 Starion Group S.A.
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
    using System.Threading.Tasks;

    using CDP4Dal;

    using DEHReqIF.ExportSettings;
    using DEHReqIF.Services;

    using NLog;

    using ReqIFSharp;

    /// <summary>
    /// The purpose of the <see cref="ConvertCommand"/> is to convert an ECSS-E-TM-10-25 requirements set
    /// into a ReqIF document
    /// </summary>
    public class ConvertCommand : IConvertCommand
    {
        /// <summary>
        /// Holds the <see cref="ILogger"/> for this class
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The (injected) <see cref="IExportSettingsReader"/> used to read export settings
        /// </summary>
        private readonly IExportSettingsReader exportSettingsReader;

        /// <summary>
        /// The (injected) <see cref="ISessionDataRetriever"/> used to read session data
        /// </summary>
        private readonly ISessionDataRetriever sessionDataRetriever;

        /// <summary>
        /// The (injected) <see cref="IReqIfFileWriter"/> used to read session data
        /// </summary>
        private readonly IReqIfFileWriter reqifFileWriter;

        /// <summary>
        /// The (injected) <see cref="ITemplateBasedReqIfBuilder"/> used to read session data
        /// </summary>
        private readonly ITemplateBasedReqIfBuilder templateBasedReqIfBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertCommand"/>
        /// </summary>
        /// <param name="exportSettingsReader">
        /// The (injected) <see cref="IExportSettingsReader"/> used to read the export settings file
        /// </param>
        /// <param name="sessionDataRetriever">
        /// The (injected) <see cref="ISessionDataRetriever"/> used to read the necessary data from a data source
        /// </param>
        /// <param name="reqifFileWriter">
        /// The (injected) <see cref="IReqIfFileWriter"/> used to write the <see cref="ReqIF"/> document to files (reqif and reqifz)
        /// </param>
        /// <param name="templateBasedReqIfBuilder"></param>
        public ConvertCommand(
            IExportSettingsReader exportSettingsReader,
            ISessionDataRetriever sessionDataRetriever,
            IReqIfFileWriter reqifFileWriter,
            ITemplateBasedReqIfBuilder templateBasedReqIfBuilder)
        {
            this.exportSettingsReader = exportSettingsReader;
            this.sessionDataRetriever = sessionDataRetriever;
            this.reqifFileWriter = reqifFileWriter;
            this.templateBasedReqIfBuilder = templateBasedReqIfBuilder;
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
        /// The ShortName of the EngineeringModelSetup
        /// </summary>
        public string EngineeringModelIid { get; set; }

        /// <summary>
        /// Gets or sets the location of the export settings file
        /// </summary>
        public string ExportSettings { get; set; } = "export-settings.json";

        /// <summary>
        /// Gets or sets a value indicating that <ALTERNATIVE-ID /> tags should not be added to the result file
        /// </summary>
        public bool ExcludeAlternativeId { get; set; }

        /// <summary>
        /// Executes the <see cref="ConvertCommand"/>
        /// </summary>
        public async Task ExecuteAsync()
        {
            try
            {
                var sw = Stopwatch.StartNew();

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
        /// Open the <see cref="Session"/> and retrieve the wanted data
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/> of type <see cref="ISession"/></returns>
        private async Task<ISession> OpenSessionAndRetrieveData()
        {
            var sw = Stopwatch.StartNew();

            var session =
                await this.sessionDataRetriever
                    .OpenSessionAndRetrieveData(this.Username, this.Password, this.DataSource, Guid.Parse(this.EngineeringModelIid));

            logger.Info($"Session was opened and data was read in {sw.ElapsedMilliseconds} [ms]");

            return session;
        }

        /// <summary>
        /// Build the ReqIf document
        /// </summary>
        /// <param name="session">The <see cref="ISession"/></param>
        /// <param name="exportSettings">The <see cref="ExportSettings"/></param>
        /// <returns>An awaitable <see cref="Task{T}"/> of type <see cref="ReqIF"/></returns>
        private async Task<ReqIF> BuildReqIf(ISession session, ExportSettings exportSettings)
        {
            var sw = Stopwatch.StartNew();

            var targetReqIF = await this.templateBasedReqIfBuilder.Build(this.TemplateSource, session, exportSettings, this.ExcludeAlternativeId);

            logger.Info($"Target ReqIf was built in {sw.ElapsedMilliseconds} [ms]");

            return targetReqIF;
        }

        /// <summary>
        /// Create the ReqIf files
        /// </summary>
        /// <param name="targetReqIf">The <see cref="ReqIF"/> document</param>
        /// <returns>an awaitable <see cref="Task"/></returns>
        private async Task CreateReqIfFiles(ReqIF targetReqIf)
        {
            var sw = Stopwatch.StartNew();

            await this.reqifFileWriter.WriteReqIfFiles(targetReqIf, this.TargetReqIF);

            logger.Info($"ReqIf was created in {sw.ElapsedMilliseconds} [ms]");
        }
    }
}
