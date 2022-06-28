//  -------------------------------------------------------------------------------------------------
//  <copyright file="ConvertCommandTestFixture.cs" company="RHEA System S.A.">
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

namespace DEHReqIF.Console.Tests
{
    using System.IO;
    using System.Threading.Tasks;

    using DEHReqIF.Console.Commands;
    using DEHReqIF.Services;

    using NLog;

    using NUnit.Framework;

    using ReqIFSharp;
    using ReqIFSharp.Extensions.Services;

    /// <summary>
    /// Suite of tests for the <see cref="ConvertCommand"/>
    /// </summary>
    public class ConvertCommandTestFixture
    {
        private string reqifPath;

        private ConvertCommand convertCommand;

        private ReqIFLoaderService reqIFLoaderService;

        private string exportSettingsPath;

        private ExportSettingsReader exportSettingsReader;

        [SetUp]
        public void Setup()
        {
            LogManager.Setup().LoadConfiguration(
                builder =>
                {
                    builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToConsole();
                });

            this.reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");
            this.exportSettingsPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "export-settings.json");
            
            var reqIfDeserializer = new ReqIFDeserializer();
            this.reqIFLoaderService = new ReqIFLoaderService(reqIfDeserializer);
            this.exportSettingsReader = new ExportSettingsReader();

            this.convertCommand = new ConvertCommand(this.reqIFLoaderService, this.exportSettingsReader);
        }

        [Test]
        public async Task Verify_that_the_ConvertCommand_executes()
        {
            this.convertCommand.TemplateSource = this.reqifPath;

            this.convertCommand.Username = "admin";
            this.convertCommand.Password = "pass";
            this.convertCommand.DataSource = "http://localhost:81";
            this.convertCommand.TemplateSource = "c:\\ReqIf\\Def2.reqifz";
            this.convertCommand.TargetReqIF = "c:\\ReqIf\\output2.reqif";
            this.convertCommand.EngineeringModelIid = "694508eb-2730-488c-9405-6ca561df68dd";
            this.convertCommand.ExportSettings = this.exportSettingsPath;

            await this.convertCommand.ExecuteAsync();
        }
    }
}
