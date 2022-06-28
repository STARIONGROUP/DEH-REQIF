// -------------------------------------------------------------------------------------------------
// <copyright file="IReqIFBuilder.cs" company="RHEA System S.A.">
//
//   Copyright 2022 RHEA System S.A.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace DEHReqIF.Tests
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using CDP4Dal;

    using DEHReqIF.ExportSettings;

    using NLog;
    using NLog.Config;
    using NLog.Targets;

    using NUnit.Framework;

    using ReqIFSharp;
    using ReqIFSharp.Extensions.Services;

    using ExternalIdentifierMap = DEHReqIF.ExportSettings.ExternalIdentifierMap;
    using IdCorrespondence = DEHReqIF.ExportSettings.IdCorrespondence;

    /// <summary>
    /// Suite of tests for the <see cref="ReqIFBuilder"/> class
    /// </summary>
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class ReqIFBuilderTestFixture
    {
        private readonly Uri uri = new("https://www.rheagroup.com");
        private Assembler assembler;

        private Iteration iteration;

        private Guid engineeringModelIid;
        private Guid iterationIid;

        private string reqifTemplatePath;
        private ReqIF templateReqIF;

        private ExportSettings exportSettings;
        private MemoryTarget memoryTarget;

        [SetUp]
        public async Task Setup()
        {
            var configuration = new LoggingConfiguration();
            this.memoryTarget = new MemoryTarget { Name = "mem" };

            configuration.AddTarget(this.memoryTarget);
            configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Warn, this.memoryTarget));
            LogManager.Configuration = configuration;

            this.engineeringModelIid = Guid.Parse("71b700a5-b6e7-418e-a38f-ca6697a0e7e0");
            this.iterationIid = Guid.Parse("d82f4d39-3c5f-4d86-aad0-8eac9be2c6cc");

            this.assembler = new Assembler(this.uri);

            this.CreateExportSettings();
            await this.CreateTemplateReqIF();
            this.PopulateRequirementsData();
        }

        private void CreateExportSettings()
        {
            var referenceMap = new ExternalIdentifierMap();

            referenceMap.Correspondence.Add(new IdCorrespondence()
            {
                ExternalId = "_21214a3a-b9c5-4242-ab90-32679435ad71",
                InternalThing = Guid.Parse("6503c390-5e9f-403e-b25d-ebe194a8fd73")
            });

            referenceMap.Correspondence.Add(new IdCorrespondence()
            {
                ExternalId = "_8cf7fb2f-f7b0-4c3a-9603-21082e01732b",
                InternalThing = Guid.Parse("14dcfc50-c410-4d8c-90ba-d8b3476f441e")
            });

            referenceMap.Correspondence.Add(new IdCorrespondence()
            {
                ExternalId = "_61b26c16-4007-46c5-a77c-ba2ad9c08ccc",
                InternalThing = Guid.Parse("7d936326-544e-4990-96cf-54f67f7aa365")
            });

            this.exportSettings = new ExportSettings
            {
                Title = "This is a test ReqIF document",
                RequirementAttributeDefinitions = new AttributeDefinitions()
                {
                    TextAttributeDefinitionId = "_9eae55c1-9c66-4308-a61e-25ba7ac63cb7_OBJECTTEXT",
                    ForeignDeletedAttributeDefinitionId = "_b72cd753-1960-4ff7-8427-7d8928a8d4ecPseudo-ForeignDeleted",
                    NameAttributeDefinitionId = "_9eae55c1-9c66-4308-a61e-25ba7ac63cb7_OBJECTSHORTTEXT",
                    ForeignModifiedOnAttributeDefinitionId = "_9eae55c1-9c66-4308-a61e-25ba7ac63cb7_LASTMODIFIEDON"
                },
                SpecificationAttributeDefinitions = new AttributeDefinitions()
                {
                    NameAttributeDefinitionId = "_4b071b6c-341d-4d14-812c-28c4a2d489b1_NAME-DOORS-MODULE"
                },
                ExternalIdentifierMap = referenceMap
            };
        }

        private async Task CreateTemplateReqIF()
        {
            //this.reqifTemplatePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "C:\\ReqIf\\";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    this.reqifTemplatePath = openFileDialog.FileName;
                }
            }

            var reqIfDeserializer = new ReqIFDeserializer();
            var reqIFLoaderService = new ReqIFLoaderService(reqIfDeserializer);

            var cts = new CancellationTokenSource();
            await using var fileStream = new FileStream(this.reqifTemplatePath, FileMode.Open);

            await reqIFLoaderService.Load(fileStream, cts.Token);

            this.templateReqIF = reqIFLoaderService.ReqIFData.Single();
        }

        private void PopulateRequirementsData()
        {
            var engineeringModel = new EngineeringModel(this.engineeringModelIid, this.assembler.Cache, this.uri);
            this.iteration = new Iteration(this.iterationIid, this.assembler.Cache, this.uri);
            engineeringModel.Iteration.Add(this.iteration);

            var specification = new RequirementsSpecification(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Name = "Requirements Specification", ShortName = "REQ"
            };

            var requirementsGroup1 = new RequirementsGroup(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Name = "Group 1",
                ShortName = "GROUP1"
            };

            var requirementsGroup2 = new RequirementsGroup(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Name = "Group 2",
                ShortName = "GROUP2"
            };

            var requirement0 = new Requirement(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Name = "Requirement 0",
                ShortName = "REQ0"
            };

            var requirement1 = new Requirement(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Name = "Requirement 1",
                ShortName = "REQ1"
            };

            var requirement2 = new Requirement(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Name = "Requirement 2",
                ShortName = "REQ2"
            };

            requirement1.Group = requirementsGroup1;
            requirement2.Group = requirementsGroup2;
            requirementsGroup1.Group.Add(requirementsGroup2);
            specification.Group.Add(requirementsGroup1);
            specification.Requirement.Add(requirement0);
            specification.Requirement.Add(requirement1);
            specification.Requirement.Add(requirement2);

            var definition0 = new Definition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Content = "This is requirement definition 0" };
            requirement0.Definition.Add(definition0);

            var definition1 = new Definition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Content = "This is requirement definition 1" };
            requirement1.Definition.Add(definition1);

            var definition2 = new Definition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Content = "This is requirement definition 2" };
            requirement2.Definition.Add(definition2);

            var reqidParameterType = new TextParameterType(Guid.Parse("14dcfc50-c410-4d8c-90ba-d8b3476f441e"), this.assembler.Cache, this.uri);
            var riskParameterType = new EnumerationParameterType(Guid.Parse("6503c390-5e9f-403e-b25d-ebe194a8fd73"), this.assembler.Cache, this.uri);
            var respParameterType = new EnumerationParameterType(Guid.Parse("7d936326-544e-4990-96cf-54f67f7aa365"), this.assembler.Cache, this.uri);

            riskParameterType.ValueDefinition.Add(
                new EnumerationValueDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    Name = "Low",
                    ShortName = "L"
                });

            riskParameterType.ValueDefinition.Add(
                new EnumerationValueDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    Name = "Medium",
                    ShortName = "M"
                });

            riskParameterType.ValueDefinition.Add(
                new EnumerationValueDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    Name = "High",
                    ShortName = "H"
                });

            respParameterType.ValueDefinition.Add(
                new EnumerationValueDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    Name = "System Engineer",
                    ShortName = "SYS"
                });

            respParameterType.ValueDefinition.Add(
                new EnumerationValueDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    Name = "Power Engineer",
                    ShortName = "PWR"
                });

            respParameterType.ValueDefinition.Add(
                new EnumerationValueDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    Name = "alexd",
                    ShortName = "alexd"
                });

            this.iteration.RequirementsSpecification.Add(specification);

            requirement1.ParameterValue.Add(
                new SimpleParameterValue(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    ParameterType = respParameterType,
                    Value = new ValueArray<string>(new[] { respParameterType.ValueDefinition.First().Name })
                });

            requirement1.ParameterValue.Add(
                new SimpleParameterValue(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    ParameterType = riskParameterType,
                    Value = new ValueArray<string>(new[] { riskParameterType.ValueDefinition.First().Name })
                });

            requirement1.ParameterValue.Add(
                new SimpleParameterValue(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    ParameterType = reqidParameterType,
                    Value = new ValueArray<string>(new[] { "REQ.A01" })
                });

            specification.ParameterValue.Add(
                new RequirementsContainerParameterValue(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    ParameterType = reqidParameterType,
                    Value = new ValueArray<string>(new[] { "SPEC.A01" })
                });
        }

        [Test]
        public void Verify_that_a_list_of_specifications_can_be_converted()
        {
            var builder = new ReqIFBuilder();

            var targetReqIf = builder.Build(this.templateReqIF, this.iteration.RequirementsSpecification, this.exportSettings);

            Assert.Multiple(() =>
            {
                Assert.That(this.memoryTarget.Logs.Count, Is.EqualTo(0));

                Assert.That(targetReqIf.TheHeader.RepositoryId, Is.EqualTo(@"EngineeringModel\71b700a5-b6e7-418e-a38f-ca6697a0e7e0\iteration\d82f4d39-3c5f-4d86-aad0-8eac9be2c6cc"));
                Assert.That(targetReqIf.TheHeader.ReqIFToolId, Is.EqualTo("RHEA DEH-REQIF"));
                Assert.That(targetReqIf.TheHeader.ReqIFVersion, Is.EqualTo("1.2"));
                Assert.That(targetReqIf.TheHeader.SourceToolId, Is.EqualTo("RHEA COMET"));
                Assert.That(targetReqIf.TheHeader.Title, Is.EqualTo("This is a test ReqIF document"));

                //Assert.That(targetReqIf.CoreContent.DataTypes.Count, Is.EqualTo(8));
                Assert.That(targetReqIf.CoreContent.DataTypes.Count, Is.EqualTo(15));

                //Assert.That(targetReqIf.CoreContent.SpecTypes.Count, Is.EqualTo(3));
                Assert.That(targetReqIf.CoreContent.SpecTypes.Count, Is.EqualTo(2));
                Assert.That(targetReqIf.CoreContent.SpecTypes.OfType<SpecObjectType>().Count(), Is.EqualTo(1));
                Assert.That(targetReqIf.CoreContent.SpecTypes.OfType<SpecificationType>().Count(), Is.EqualTo(1));

                //Assert.That(targetReqIf.CoreContent.SpecTypes.OfType<SpecRelationType>().Count(), Is.EqualTo(1));
                Assert.That(targetReqIf.CoreContent.SpecTypes.OfType<SpecRelationType>().Count(), Is.EqualTo(0));
                Assert.That(targetReqIf.ToolExtension.Count, Is.EqualTo(1));
            });

            if (true)
            {
                var entryName = "output.reqif";
                var reqifFileLocation = $"c:\\reqif\\{entryName}";
                var reqifzFileLocation = $"{reqifFileLocation}z";

                new ReqIFSerializer().Serialize(targetReqIf, "c:\\reqif\\output.reqif");

                if (System.IO.File.Exists(reqifzFileLocation))
                {
                    System.IO.File.Delete(reqifzFileLocation);
                }

                using (var zipToOpen = new FileStream(reqifzFileLocation, FileMode.CreateNew))
                {
                    using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                    {
                        archive.CreateEntryFromFile(reqifFileLocation, entryName);
                    }
                }
            }
            else
            {
                using (var stream = new MemoryStream())
                {
                    new ReqIFSerializer().Serialize(targetReqIf, stream);
                    stream.Position = 0;

                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var test = reader.ReadToEnd();
                    }
                }
            }
        }

        [Test]
        public void Verify_that_logging_works_for_RequirementAttributeDefinitions()
        {
            var builder = new ReqIFBuilder();

            var attritbuteDefinitionId = this.exportSettings.RequirementAttributeDefinitions.TextAttributeDefinitionId += "NOT_FOUND";

            builder.Build(this.templateReqIF, this.iteration.RequirementsSpecification, this.exportSettings);

            Assert.That(this.memoryTarget.Logs.Count, Is.EqualTo(5));

            foreach (var log in this.memoryTarget.Logs)
            {
                Assert.That(log.Contains($"The expected export setting {attritbuteDefinitionId} was not found in the template ReqIF file."), Is.True);
            }
        }

        [Test]
        public void Verify_that_logging_works_for_SpecificationAttributeDefinitions()
        {
            var builder = new ReqIFBuilder();

            var attritbuteDefinitionId = this.exportSettings.SpecificationAttributeDefinitions.NameAttributeDefinitionId += "NOT_FOUND";

            builder.Build(this.templateReqIF, this.iteration.RequirementsSpecification, this.exportSettings);

            Assert.That(this.memoryTarget.Logs.Count, Is.EqualTo(1));

            Assert.That(this.memoryTarget.Logs[0].Contains($"The expected export setting {attritbuteDefinitionId} was not found in the template ReqIF file."), Is.True);
        }

        [Test]
        public void Verify_that_logging_works_for_Check_On_SpecObjectType()
        {
            var builder = new ReqIFBuilder();

            this.templateReqIF.CoreContent.SpecTypes.Add(new SpecObjectType());

            builder.Build(this.templateReqIF, this.iteration.RequirementsSpecification, this.exportSettings);

            Assert.That(this.memoryTarget.Logs.Count, Is.EqualTo(1));

            Assert.That(this.memoryTarget.Logs[0].Contains("Multiple SpecObjectTypes were found. The first one found is selected as the SpecObjectType to use during conversion."), Is.True);
        }

        [Test]
        public void Verify_that_logging_works_for_Check_On_SpeccificationType()
        {
            var builder = new ReqIFBuilder();

            this.templateReqIF.CoreContent.SpecTypes.Add(new SpecificationType());

            builder.Build(this.templateReqIF, this.iteration.RequirementsSpecification, this.exportSettings);

            Assert.That(this.memoryTarget.Logs.Count, Is.EqualTo(1));

            Assert.That(this.memoryTarget.Logs[0].Contains("Multiple SpecificationTypes were found. The first one found is selected as the SpecificationType to use during conversion."), Is.True);
        }

        [Test]
        public void Verify_that_Exception_is_thrown_when_no_SpecObjectTypes_were_found()
        {
            var builder = new ReqIFBuilder();

            this.templateReqIF.CoreContent.SpecTypes.Remove(this.templateReqIF.CoreContent.SpecTypes.First(x => x is SpecObjectType));

            Assert.Throws<NotSupportedException>(() => builder.Build(this.templateReqIF, this.iteration.RequirementsSpecification, this.exportSettings));
        }

        [Test]
        public void Verify_that_Exception_is_thrown_when_no_SpecificationTypes_were_found()
        {
            var builder = new ReqIFBuilder();

            this.templateReqIF.CoreContent.SpecTypes.Remove(this.templateReqIF.CoreContent.SpecTypes.First(x => x is SpecificationType));

            Assert.Throws<NotSupportedException>(() => builder.Build(this.templateReqIF, this.iteration.RequirementsSpecification, this.exportSettings));
        }
    }
}
