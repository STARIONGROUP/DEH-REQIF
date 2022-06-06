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

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Dal;

    using DEHReqIF;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ReqIFBuilder"/> class
    /// </summary>
    public class ReqIFBuilderTestFixture
    {
        private readonly Uri uri = new Uri("https://www.rheagroup.com");
        private Assembler assembler;

        private List<RequirementsSpecification> requirementsSpecifications;
        
        [SetUp]
        public void Setup()
        {
            this.assembler = new Assembler(this.uri);
            this.requirementsSpecifications = new List<RequirementsSpecification>();
        }

        private void PopulateRequirementsData()
        {
            this.requirementsSpecifications = new List<RequirementsSpecification>();
            
            var specification = new RequirementsSpecification(Guid.NewGuid(), this.assembler.Cache, this.uri);

            var requirement = new Requirement(Guid.NewGuid(), this.assembler.Cache, this.uri);
            var definition = new Definition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Content = "def0" };
            requirement.Definition.Add(definition);

            specification.Requirement.Add(requirement);

            this.requirementsSpecifications.Add(specification);
        }

        [Test]
        public void Verify_that_a_list_of_specifications_can_be_converted()
        {
            var builder = new ReqIFBuilder();

            Assert.That(async () => await builder.Build(this.requirementsSpecifications),
                Throws.Exception.TypeOf<NotImplementedException>());
        }
    }
}
