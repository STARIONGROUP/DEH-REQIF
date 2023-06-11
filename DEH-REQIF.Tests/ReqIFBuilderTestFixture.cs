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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using CDP4Dal;

    using DEHReqIF.ExportSettings;
    using DEHReqIF.Services;

    using NLog;
    using NLog.Config;
    using NLog.Targets;

    using NUnit.Framework;

    using ReqIFSharp;
    using ReqIFSharp.Extensions.Services;

    using ExternalIdentifierMap = DEHReqIF.ExportSettings.ExternalIdentifierMap;
    using File = CDP4Common.EngineeringModelData.File;
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
        private Guid codeParameterTypeGuid;
        private Guid enumerationParameterType1Guid;
        private Guid enumerationParameterType2Guid;
        private RequirementsGroup requirementsGroup1;
        private RequirementsGroup requirementsGroup2;
        private Requirement requirement0;
        private Requirement requirement1;
        private Requirement requirement2;

        [SetUp]
        public async Task Setup()
        {
            var configuration = new LoggingConfiguration();
            this.memoryTarget = new MemoryTarget { Name = "mem" };

            configuration.AddTarget(this.memoryTarget);
            configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Warn, this.memoryTarget));
            LogManager.Configuration = configuration;

            this.codeParameterTypeGuid = Guid.NewGuid();
            this.enumerationParameterType1Guid = Guid.NewGuid();
            this.enumerationParameterType2Guid = Guid.NewGuid();

            this.engineeringModelIid = Guid.NewGuid();
            this.iterationIid = Guid.NewGuid();

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
                ExternalId = "_IdInSource1",
                InternalThing = this.codeParameterTypeGuid
            });

            referenceMap.Correspondence.Add(new IdCorrespondence()
            {
                ExternalId = "_IdInSource2",
                InternalThing = this.codeParameterTypeGuid
            });

            referenceMap.Correspondence.Add(new IdCorrespondence()
            {
                ExternalId = "_IdInSourceSpecification",
                InternalThing = this.codeParameterTypeGuid
            });

            referenceMap.Correspondence.Add(new IdCorrespondence()
            {
                ExternalId = "_EnumerationParameter1",
                InternalThing = this.enumerationParameterType1Guid
            });

            referenceMap.Correspondence.Add(new IdCorrespondence()
            {
                ExternalId = "_EnumerationParameter2",
                InternalThing = this.enumerationParameterType2Guid
            });

            this.exportSettings = new ExportSettings
            {
                Title = "This is a test ReqIF document",
                RequirementAttributeDefinitions = new AttributeDefinitions()
                {
                    TextAttributeDefinitionId = "_TextParameter",
                    ForeignDeletedAttributeDefinitionId = "Pseudo-ForeignDeleted",
                    NameAttributeDefinitionId = "_NameParameter",
                    ForeignModifiedOnAttributeDefinitionId = "LASTMODIFIEDON"
                },
                SpecificationAttributeDefinitions = new AttributeDefinitions()
                {
                    TextAttributeDefinitionId = "_TextParameterSpecification",
                    NameAttributeDefinitionId = "_NameParameterSpecification"
                },
                ExternalIdentifierMap = referenceMap,
                AddXhtmlTags = true
            };
        }

        private async Task CreateTemplateReqIF()
        {
            this.reqifTemplatePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "Test.reqif");

            var reqIfDeserializer = new ReqIFDeserializer();
            var reqIFLoaderService = new ReqIFLoaderService(reqIfDeserializer);
            var cts = new CancellationTokenSource();

            await using var fileStream = new FileStream(this.reqifTemplatePath, FileMode.Open);
            await reqIFLoaderService.Load(fileStream, this.reqifTemplatePath.ConvertPathToSupportedFileExtensionKind(), cts.Token);

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

            this.requirementsGroup1 = new RequirementsGroup(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Name = "Group 1",
                ShortName = "GROUP1"
            };

            this.requirementsGroup2 = new RequirementsGroup(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Name = "Group 2",
                ShortName = "GROUP2"
            };

            this.requirement0 = new Requirement(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Name = "Requirement 0",
                ShortName = "REQ0"
            };

            this.requirement1 = new Requirement(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Name = "Requirement 1",
                ShortName = "REQ1"
            };

            this.requirement2 = new Requirement(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Name = "Requirement 2",
                ShortName = "REQ2"
            };

            this.requirement1.Group = this.requirementsGroup1;
            this.requirement2.Group = this.requirementsGroup2;
            this.requirementsGroup1.Group.Add(this.requirementsGroup2);
            specification.Group.Add(this.requirementsGroup1);
            specification.Requirement.Add(this.requirement0);
            specification.Requirement.Add(this.requirement1);
            specification.Requirement.Add(this.requirement2);

            var definition0 = new Definition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Content = "This is requirement definition 0 & < > ; &nbsp; <div></div> ?!@#$%^&*()-_=+[]{};'\\:|\",./<>?" };
            this.requirement0.Definition.Add(definition0);

            var definition1 = new Definition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Content = "This is requirement definition 1" };
            this.requirement1.Definition.Add(definition1);

            var definition2 = new Definition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Content = "This is requirement definition 2" };
            this.requirement2.Definition.Add(definition2);

            var codeParameterType = new TextParameterType(this.codeParameterTypeGuid, this.assembler.Cache, this.uri);
            var enumerationParameterType1 = new EnumerationParameterType(this.enumerationParameterType1Guid, this.assembler.Cache, this.uri);
            var enumerationParameterType2 = new EnumerationParameterType(this.enumerationParameterType2Guid, this.assembler.Cache, this.uri);

            enumerationParameterType1.ValueDefinition.Add(
                new EnumerationValueDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    Name = "Low",
                    ShortName = "L"
                });

            enumerationParameterType1.ValueDefinition.Add(
                new EnumerationValueDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    Name = "Medium",
                    ShortName = "M"
                });

            enumerationParameterType1.ValueDefinition.Add(
                new EnumerationValueDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    Name = "High",
                    ShortName = "H"
                });

            enumerationParameterType2.ValueDefinition.Add(
                new EnumerationValueDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    Name = "System Engineer",
                    ShortName = "SYS"
                });

            enumerationParameterType2.ValueDefinition.Add(
                new EnumerationValueDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    Name = "Power Engineer",
                    ShortName = "PWR"
                });

            this.iteration.RequirementsSpecification.Add(specification);

            this.requirement1.ParameterValue.Add(
                new SimpleParameterValue(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    ParameterType = enumerationParameterType2,
                    Value = new ValueArray<string>(new[] { enumerationParameterType2.ValueDefinition.First().Name })
                });

            this.requirement1.ParameterValue.Add(
                new SimpleParameterValue(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    ParameterType = enumerationParameterType1,
                    Value = new ValueArray<string>(new[] { enumerationParameterType1.ValueDefinition.First().Name })
                });

            this.requirement1.ParameterValue.Add(
                new SimpleParameterValue(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    ParameterType = codeParameterType,
                    Value = new ValueArray<string>(new[] { "REQ.A01" })
                });

            specification.ParameterValue.Add(
                new RequirementsContainerParameterValue(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    ParameterType = codeParameterType,
                    Value = new ValueArray<string>(new[] { "SPEC.A01" })
                });
        }

        [Test]
        public void Verify_that_a_list_of_specifications_can_be_converted()
        {
            var builder = new ReqIFBuilder();

            var targetReqIf = builder.Build(this.templateReqIF, this.iteration.RequirementsSpecification, this.exportSettings, true);

            var reqifSerializer = new ReqIFSerializer();

            var stream = new MemoryStream();
            reqifSerializer.Serialize(new List<ReqIF> { targetReqIf }, stream, SupportedFileExtensionKind.Reqif);

            stream.Position = 0;
            var reqIfXml = new StreamReader(stream).ReadToEnd();

            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrWhiteSpace(reqIfXml), Is.Not.True);
                Assert.That(reqIfXml.ToLower(), Does.Not.Contain("<alternative-id"));

                Assert.That(this.memoryTarget.Logs, Has.Count.EqualTo(0));

                Assert.That(targetReqIf.TheHeader.RepositoryId, Is.EqualTo(@$"EngineeringModel\{this.engineeringModelIid}\iteration\{this.iterationIid}"));
                Assert.That(targetReqIf.TheHeader.ReqIFToolId, Is.EqualTo("RHEA DEH-REQIF"));
                Assert.That(targetReqIf.TheHeader.ReqIFVersion, Is.EqualTo("1.2"));
                Assert.That(targetReqIf.TheHeader.SourceToolId, Is.EqualTo("RHEA COMET"));
                Assert.That(targetReqIf.TheHeader.Title, Is.EqualTo("This is a test ReqIF document"));
                
                Assert.That(targetReqIf.CoreContent.SpecRelations, Has.Count.EqualTo(0));
                Assert.That(targetReqIf.CoreContent.SpecRelationGroups, Has.Count.EqualTo(0));
                Assert.That(targetReqIf.CoreContent.DataTypes, Has.Count.EqualTo(6));
                Assert.That(targetReqIf.CoreContent.SpecTypes, Has.Count.EqualTo(2));
                Assert.That(targetReqIf.CoreContent.SpecTypes.OfType<SpecObjectType>().Count(), Is.EqualTo(1));
                Assert.That(targetReqIf.CoreContent.SpecTypes.OfType<SpecificationType>().Count(), Is.EqualTo(1));

                Assert.That(targetReqIf.CoreContent.SpecTypes.OfType<SpecRelationType>().Count(), Is.EqualTo(0));
                Assert.That(targetReqIf.ToolExtension, Has.Count.EqualTo(1));
                Assert.That(targetReqIf.CoreContent.SpecObjects, Has.Count.EqualTo(5));
                Assert.That(targetReqIf.CoreContent.Specifications, Has.Count.EqualTo(1));
                Assert.That(targetReqIf.CoreContent.Specifications.First().Children, Has.Count.EqualTo(2));

                var specification = targetReqIf
                    .CoreContent
                    .Specifications
                    .First();

                var group1 =
                    specification
                        .Children
                        .SingleOrDefault(x =>
                            x.Object.LongName == this.requirementsGroup1.Name);

                var group2 =
                    group1?.Children
                        .SingleOrDefault(x =>
                            x.Object.LongName == this.requirementsGroup2.Name);

                var req0 =
                    specification
                        .Children
                        .SingleOrDefault(x => x.Object.LongName == this.requirement0.Name);

                var req1 =
                    group1?
                        .Children
                        .SingleOrDefault(x => x.Object.LongName == this.requirement1.Name);

                var req2 =
                    group2?
                        .Children
                        .SingleOrDefault(x => x.Object.LongName == this.requirement2.Name);

                Assert.That(specification, Is.Not.Null);
                Assert.That(specification.Values, Has.Count.EqualTo(3));
                Assert.That(specification.Children, Has.Count.EqualTo(2));
                Assert.That(specification.Values.Where(x => x.ObjectValue == null).Count, Is.EqualTo(0));

                Assert.That(group1, Is.Not.Null);
                Assert.That(group1.Children.Count, Is.EqualTo(2));
                Assert.That(group1.Object.Values.Count, Is.EqualTo(3));
                Assert.That(group1.Object.Values.Where(x => x.ObjectValue == null).Count, Is.EqualTo(0));

                Assert.That(group2, Is.Not.Null);
                Assert.That(group2.Children.Count, Is.EqualTo(1));
                Assert.That(group2.Object.Values.Count, Is.EqualTo(3));
                Assert.That(group2.Object.Values.Where(x => x.ObjectValue == null).Count, Is.EqualTo(0));

                Assert.That(req0, Is.Not.Null);
                Assert.That(req0.Object.Values.Count, Is.EqualTo(4));
                Assert.That(req0.Object.Values.Where(x => x.ObjectValue == null).Count, Is.EqualTo(0));

                Assert.That(req1, Is.Not.Null);
                Assert.That(req1.Object.Values.Count, Is.EqualTo(8));
                Assert.That(req1.Object.Values.Where(x => x.ObjectValue == null).Count, Is.EqualTo(0));

                Assert.That(req2, Is.Not.Null);
                Assert.That(req2.Object.Values.Count, Is.EqualTo(4));
                Assert.That(req2.Object.Values.Where(x => x.ObjectValue == null).Count, Is.EqualTo(0));
            });
        }

        [Test]
        public void Verify_that_alternative_id_tags_are_added()
        {
            var builder = new ReqIFBuilder();

            var targetReqIf = builder.Build(this.templateReqIF, this.iteration.RequirementsSpecification, this.exportSettings, false);

            var reqifSerializer = new ReqIFSerializer();

            var stream = new MemoryStream();
            reqifSerializer.Serialize(new List<ReqIF> { targetReqIf }, stream, SupportedFileExtensionKind.Reqif);

            stream.Position = 0;
            var reqIfXml = new StreamReader(stream).ReadToEnd();

            Assert.IsTrue(reqIfXml.ToLower().Contains("<alternative-id")); 
        }

        [Test]
        public void Verify_that_reqifz_can_be_created()
        {
            var builder = new ReqIFBuilder();

            var targetReqIf = builder.Build(this.templateReqIF, this.iteration.RequirementsSpecification, this.exportSettings, false);

            var reqifSerializer = new ReqIFSerializer();

            var writer = new ReqIfFileWriter();

            var stream = new MemoryStream();
            Assert.DoesNotThrowAsync( () => writer.WriteReqIfFiles(targetReqIf, "tempfile.reqifz"));

            if (System.IO.File.Exists("tempfile.reqifz"))
            {
                System.IO.File.Delete("tempfile.reqifz");
            }
        }

        [Test]
        public void Verify_that_logging_works_for_RequirementAttributeDefinitions()
        {
            var builder = new ReqIFBuilder();

            var attritbuteDefinitionId = this.exportSettings.RequirementAttributeDefinitions.TextAttributeDefinitionId += "NOT_FOUND";

            builder.Build(this.templateReqIF, this.iteration.RequirementsSpecification, this.exportSettings, false);

            Assert.That(this.memoryTarget.Logs, Has.Count.EqualTo(5));

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

            builder.Build(this.templateReqIF, this.iteration.RequirementsSpecification, this.exportSettings, false);

            Assert.That(this.memoryTarget.Logs, Has.Count.EqualTo(1));

            Assert.That(this.memoryTarget.Logs[0].Contains($"The expected export setting {attritbuteDefinitionId} was not found in the template ReqIF file."), Is.True);
        }

        [Test]
        public void Verify_that_logging_works_for_Check_On_SpecObjectType()
        {
            var builder = new ReqIFBuilder();

            this.templateReqIF.CoreContent.SpecTypes.Add(new SpecObjectType());

            builder.Build(this.templateReqIF, this.iteration.RequirementsSpecification, this.exportSettings, false);

            Assert.That(this.memoryTarget.Logs, Has.Count.EqualTo(1));

            Assert.That(this.memoryTarget.Logs[0].Contains("Multiple SpecObjectTypes were found. The first one found is selected as the SpecObjectType to use during conversion."), Is.True);
        }

        [Test]
        public void Verify_that_logging_works_for_Check_On_SpeccificationType()
        {
            var builder = new ReqIFBuilder();

            this.templateReqIF.CoreContent.SpecTypes.Add(new SpecificationType());

            builder.Build(this.templateReqIF, this.iteration.RequirementsSpecification, this.exportSettings, false);

            Assert.That(this.memoryTarget.Logs, Has.Count.EqualTo(1));

            Assert.That(this.memoryTarget.Logs[0].Contains("Multiple SpecificationTypes were found. The first one found is selected as the SpecificationType to use during conversion."), Is.True);
        }

        [Test]
        public void Verify_that_Exception_is_thrown_when_no_SpecObjectTypes_were_found()
        {
            var builder = new ReqIFBuilder();

            this.templateReqIF.CoreContent.SpecTypes.Remove(this.templateReqIF.CoreContent.SpecTypes.First(x => x is SpecObjectType));

            Assert.Throws<NotSupportedException>(() => builder.Build(this.templateReqIF, this.iteration.RequirementsSpecification, this.exportSettings, false));
        }

        [Test]
        public void Verify_that_Exception_is_thrown_when_no_SpecificationTypes_were_found()
        {
            var builder = new ReqIFBuilder();

            this.templateReqIF.CoreContent.SpecTypes.Remove(this.templateReqIF.CoreContent.SpecTypes.First(x => x is SpecificationType));

            Assert.Throws<NotSupportedException>(() => builder.Build(this.templateReqIF, this.iteration.RequirementsSpecification, this.exportSettings, false));
        }
    }
}
