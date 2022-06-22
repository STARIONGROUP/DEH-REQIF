//  -------------------------------------------------------------------------------------------------
//  <copyright file="ExportSettingsReaderTestFixture.cs" company="RHEA System S.A.">
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

namespace DEHReqIF.Tests.Services
{
    using System.IO;
    using System.Threading.Tasks;

    using DEHReqIF.Services;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ExportSettingsReader"/> class
    /// </summary>
    [TestFixture]
    public class ExportSettingsReaderTestFixture
    {
        private string settingsFilePath;

        private ExportSettingsReader exportSettingsReader;

        [SetUp]
        public void SetUp()
        {
            this.settingsFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "export-settings.json");

            this.exportSettingsReader = new ExportSettingsReader();
        }

        [Test]
        public async Task Verify_that_a_settings_File_is_correctly_read()
        {
            var exportSettings = await this.exportSettingsReader.Read((this.settingsFilePath));


            Assert.That(exportSettings.Title, Is.EqualTo("This is a test ReqIF document"));
            Assert.That(exportSettings.RequirementTextDataTypeDefinitionId, Is.EqualTo("_37267b33-5f52-4bf7-833d-78bee976f875"));
        }
    }
}
