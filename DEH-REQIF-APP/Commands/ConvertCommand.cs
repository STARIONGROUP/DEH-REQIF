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
    using System.Threading;
    using System.Threading.Tasks;

    using ReqIFSharp.Extensions.Services;

    using Serilog;

    /// <summary>
    /// The purpose of the <see cref="ConvertCommand"/> is to convert an ECSS-E-TM-10-25 requirements set
    /// into a ReqIF document
    /// </summary>
    public class ConvertCommand : IConvertCommand
    {
        /// <summary>
        /// The (injected) <see cref="IReqIFLoaderService"/> used to read a ReqIF document
        /// </summary>
        private IReqIFLoaderService reqIfLoaderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertCommand"/>
        /// </summary>
        /// <param name="reqIfLoaderService">
        /// The (injected) <see cref="IReqIFLoaderService"/> used to read a ReqIF document
        /// </param>
        public ConvertCommand(IReqIFLoaderService reqIfLoaderService)
        {
            this.reqIfLoaderService = reqIfLoaderService;
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
        /// Executes the <see cref="ConvertCommand"/>
        /// </summary>
        public async Task ExecuteAsync()
        {
            try
            {
                var sw = Stopwatch.StartNew();
                
                await using var fileStream = new FileStream(this.TemplateSource, FileMode.Open);

                var cts = new CancellationTokenSource();
                await this.reqIfLoaderService.Load(fileStream, cts.Token);
                
                Log.Information($"{this.TemplateSource} read in {sw.ElapsedMilliseconds} [ms]");
            }
            catch (Exception e)
            {
                Log.Logger.Fatal(e, "ECSS-E-TM-10-25 to ReqIF Conversion Error");
                Console.WriteLine($"Converting the ECSS-E-TM-10-25 data to ReqIF failed due to an error: {e.Message}");
                Console.WriteLine("Hit Enter to continue");
                Console.ReadLine();
            }
        }
    }
}
