//  -------------------------------------------------------------------------------------------------
//  <copyright file="ConvertCommandTestFixture.cs" company="Starion Group S.A.">
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

namespace DEHReqIF.Console.Tests
{
    using System;
    using System.Threading.Tasks;

    using CDP4Dal;

    using DEHReqIF.Console.Commands;
    using DEHReqIF.ExportSettings;
    using DEHReqIF.Services;

    using Moq;

    using NLog;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="ConvertCommand"/>
    /// </summary>
    public class ConvertCommandTestFixture
    {
        private ConvertCommand convertCommand;

        private Mock<IExportSettingsReader> exportSettingsReader;

        private Mock<IReqIfFileWriter> fileWriter;

        private Mock<ISessionDataRetriever> sessionDataRetriever;

        private Mock<ITemplateBasedReqIfBuilder> templateBasedReqIfBuilder;

        [SetUp]
        public void Setup()
        {
            LogManager.Setup().LoadConfiguration(
                builder => { builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToConsole(); });

            this.exportSettingsReader = new Mock<IExportSettingsReader>();
            this.fileWriter = new Mock<IReqIfFileWriter>();
            this.sessionDataRetriever = new Mock<ISessionDataRetriever>();
            this.templateBasedReqIfBuilder = new Mock<ITemplateBasedReqIfBuilder>();

            this.convertCommand =
                new ConvertCommand(
                    this.exportSettingsReader.Object,
                    this.sessionDataRetriever.Object,
                    this.fileWriter.Object,
                    this.templateBasedReqIfBuilder.Object);
        }

        [Test]
        public async Task Verify_that_the_ConvertCommand_executes()
        {
            this.convertCommand.Username = "user";
            this.convertCommand.Password = "****";
            this.convertCommand.DataSource = "http://someuri";
            this.convertCommand.EngineeringModelIid = "694508eb-2730-488c-9405-6ca561df68dd";

            await this.convertCommand.ExecuteAsync();

            this.exportSettingsReader.Verify(
                x => x.ReadFile(
                    It.IsAny<string>()),
                Times.Once);

            this.fileWriter.Verify(
                x => x.WriteReqIfFiles(
                    It.IsAny<ReqIF>(),
                    It.IsAny<string>()),
                Times.Once);

            this.sessionDataRetriever.Verify(
                x => x.OpenSessionAndRetrieveData(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>()),
                Times.Once);

            this.templateBasedReqIfBuilder.Verify(
                x => x.Build(
                    It.IsAny<string>(),
                    It.IsAny<ISession>(),
                    It.IsAny<ExportSettings>(),
                    It.IsAny<bool>()),
                Times.Once);
        }
    }
}
