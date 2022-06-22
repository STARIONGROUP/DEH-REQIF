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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.IO;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Dal;

    using DEHReqIF;

    using ReqIFSharp;
    using ReqIFSharp.Extensions.Services;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ReqIFBuilder"/> class
    /// </summary>
    [TestFixture]
    public class ReqIFBuilderTestFixture
    {
        private readonly Uri uri = new Uri("https://www.rheagroup.com");
        private Assembler assembler;

        private Iteration iteration;

        private Guid engineeringModelIid;
        private Guid iterationIid;

        private string reqifTemplatePath;
        private ReqIF templateReqIF;

        private ExportSettings exportSettings;
        
        [SetUp]
        public async Task Setup()
        {
            this.engineeringModelIid = Guid.Parse("71b700a5-b6e7-418e-a38f-ca6697a0e7e0");
            this.iterationIid = Guid.Parse("d82f4d39-3c5f-4d86-aad0-8eac9be2c6cc");

            this.assembler = new Assembler(this.uri);
            
            this.CreateExportSettings();
            await this.CreateTemplateReqIF();
            this.PopulateRequirementsData();
        }

        private void CreateExportSettings()
        {
            this.exportSettings = new ExportSettings
            {
                Title = "This is a test ReqIF document",
                RequirementTextDataTypeDefinitionId = "_37267b33-5f52-4bf7-833d-78bee976f875"
            };
        }
        
        private async Task CreateTemplateReqIF()
        {
            this.reqifTemplatePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");

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
            engineeringModel.Iteration.Add(iteration);
            
            var specification = new RequirementsSpecification(Guid.NewGuid(), this.assembler.Cache, this.uri);

            var requirement = new Requirement(Guid.NewGuid(), this.assembler.Cache, this.uri);
            var definition = new Definition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Content = "def0" };
            requirement.Definition.Add(definition);

            specification.Requirement.Add(requirement);

            iteration.RequirementsSpecification.Add(specification);
        }

        [Test]
        public void Verify_that_a_list_of_specifications_can_be_converted()
        {
            var builder = new ReqIFBuilder();

            var targetReqIf = builder.Build(this.templateReqIF, this.iteration.RequirementsSpecification, this.exportSettings);

            Assert.Multiple(() =>
            {
                Assert.That(targetReqIf.TheHeader.RepositoryId, Is.EqualTo(@"EngineeringModel\71b700a5-b6e7-418e-a38f-ca6697a0e7e0\iteration\d82f4d39-3c5f-4d86-aad0-8eac9be2c6cc"));
                Assert.That(targetReqIf.TheHeader.ReqIFToolId, Is.EqualTo("RHEA DEH-REQIF"));
                Assert.That(targetReqIf.TheHeader.ReqIFVersion, Is.EqualTo("1.2"));
                Assert.That(targetReqIf.TheHeader.SourceToolId, Is.EqualTo("RHEA COMET"));
                Assert.That(targetReqIf.TheHeader.Title, Is.EqualTo("This is a test ReqIF document"));

                Assert.That(targetReqIf.CoreContent.DataTypes.Count, Is.EqualTo(8));
                Assert.That(targetReqIf.CoreContent.SpecTypes.Count, Is.EqualTo(3));
                Assert.That(targetReqIf.CoreContent.SpecTypes.OfType<SpecObjectType>().Count(), Is.EqualTo(1));
                Assert.That(targetReqIf.CoreContent.SpecTypes.OfType<SpecificationType>().Count(), Is.EqualTo(1));
                Assert.That(targetReqIf.CoreContent.SpecTypes.OfType<SpecRelationType>().Count(), Is.EqualTo(1));
                Assert.That(targetReqIf.ToolExtension.Count, Is.EqualTo(1));
            });
        }
    }
}
