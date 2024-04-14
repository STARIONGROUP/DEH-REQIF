//  -------------------------------------------------------------------------------------------------
//  <copyright file="ExportSettingsReaderTestFixture.cs" company="RHEA System S.A.">
// 
//    Copyright 2022-2024 RHEA System S.A.
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
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    using DEHReqIF.ExportSettings;
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
            var exportSettings = await this.exportSettingsReader.ReadFile((this.settingsFilePath));

            Assert.Multiple(() => {
                Assert.That(exportSettings.Title, Is.EqualTo("This is a test ReqIF document"));

                Assert.That(exportSettings.RequirementAttributeDefinitions.TextAttributeDefinitionId, Is.EqualTo("OBJECTTEXT"));
                Assert.That(exportSettings.RequirementAttributeDefinitions.ForeignDeletedAttributeDefinitionId, Is.EqualTo("Pseudo-ForeignDeleted"));
                Assert.That(exportSettings.RequirementAttributeDefinitions.NameAttributeDefinitionId, Is.EqualTo("OBJECTSHORTTEXT"));
                Assert.That(exportSettings.RequirementAttributeDefinitions.ForeignModifiedOnAttributeDefinitionId, Is.EqualTo("LASTMODIFIEDON"));

                Assert.That(exportSettings.SpecificationAttributeDefinitions.NameAttributeDefinitionId, Is.EqualTo("NAME-DOORS-MODULE"));

                Assert.That(exportSettings.ExternalIdentifierMap, Is.Not.Null);
                Assert.That(exportSettings.ExternalIdentifierMap.Correspondence, Has.Count.EqualTo(1));
                Assert.That(exportSettings.ExternalIdentifierMap.Correspondence.First().ExternalId, Is.EqualTo("EXTERNAL_ID"));
                Assert.That(exportSettings.ExternalIdentifierMap.Correspondence.First().InternalThing, Is.EqualTo(Guid.Parse("7d936326-544e-4990-96cf-54f67f7aa365")));
            });            
        }

        [Test]
        public void Verify_that_a_settings_File_can_be_created_and_correctly_read()
        {
            var title = "This is a test ReqIF document";
            var requirementTextDataTypeDefinitionId = "OBJECTTEXT";
            var requirementForeignDeletedAttributeDefinitionId = "Pseudo-ForeignDeleted";
            var requirementNameAttributeDefinitionId = "OBJECTSHORTTEXT";
            var requirementForeignModifiedOnAttributeDefinitionId = "LASTMODIFIEDON";
            var specificationNameAttributeDefinitionId = "NAME-DOORS-MODULE";

            var externalId = "Test";
            var internalThing = Guid.NewGuid();

            var referenceMap = new ExternalIdentifierMap();
            referenceMap.Correspondence.Add(new IdCorrespondence() {ExternalId = externalId, InternalThing = internalThing});

            var exportSettings = new ExportSettings
            {
                Title = title,
                ExternalIdentifierMap = referenceMap,
                RequirementAttributeDefinitions = new AttributeDefinitions()
                {
                    TextAttributeDefinitionId = requirementTextDataTypeDefinitionId,
                    ForeignDeletedAttributeDefinitionId = requirementForeignDeletedAttributeDefinitionId,
                    NameAttributeDefinitionId = requirementNameAttributeDefinitionId,
                    ForeignModifiedOnAttributeDefinitionId = requirementForeignModifiedOnAttributeDefinitionId
                },
                SpecificationAttributeDefinitions = new AttributeDefinitions()
                {
                    NameAttributeDefinitionId = specificationNameAttributeDefinitionId,
                }
            };

            var json = JsonSerializer.Serialize(exportSettings, ExportSettingsReader.GetJsonSerializerOptions());

            var deserializedExportSettings = this.exportSettingsReader.Read(json);

            Assert.Multiple(() => {
                Assert.That(deserializedExportSettings.Title, Is.EqualTo(title));
                Assert.That(deserializedExportSettings.RequirementAttributeDefinitions.TextAttributeDefinitionId, Is.EqualTo(requirementTextDataTypeDefinitionId));
                Assert.That(deserializedExportSettings.RequirementAttributeDefinitions.ForeignDeletedAttributeDefinitionId, Is.EqualTo(requirementForeignDeletedAttributeDefinitionId));
                Assert.That(deserializedExportSettings.RequirementAttributeDefinitions.NameAttributeDefinitionId, Is.EqualTo(requirementNameAttributeDefinitionId));
                Assert.That(deserializedExportSettings.RequirementAttributeDefinitions.ForeignModifiedOnAttributeDefinitionId, Is.EqualTo(requirementForeignModifiedOnAttributeDefinitionId));
                Assert.That(deserializedExportSettings.SpecificationAttributeDefinitions.NameAttributeDefinitionId, Is.EqualTo(specificationNameAttributeDefinitionId));
                Assert.That(deserializedExportSettings.ExternalIdentifierMap, Is.Not.Null);
                Assert.That(deserializedExportSettings.ExternalIdentifierMap.Correspondence, Has.Count.EqualTo(1));
                Assert.That(deserializedExportSettings.ExternalIdentifierMap.Correspondence.First().ExternalId, Is.EqualTo(externalId));
                Assert.That(deserializedExportSettings.ExternalIdentifierMap.Correspondence.First().InternalThing, Is.EqualTo(internalThing));
            });    
        }
    }
}
