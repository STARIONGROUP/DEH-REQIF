// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFBuilder.cs" company="RHEA System S.A.">
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

namespace DEHReqIF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using ReqIFSharp;

    /// <summary>
    /// The purpose of the <see cref="ReqIFBuilder"/> is to build/generate a ReqIF file based
    /// om an ECSS-E-TM-10-25 data set (requirements and reference data)
    /// </summary>
    public class ReqIFBuilder : IReqIFBuilder
    {
        /// <summary>
        /// Builds the ReqIF content from the provided <see cref="IEnumerable{RequirementsSpecification}"/>
        /// </summary>
        /// <param name="templateReqif">
        /// The template <see cref="ReqIF"/> used to map <see cref="DatatypeDefinition"/>s, <see cref="SpecObjectType"/>s, <see cref="SpecificationType"/>
        /// and <see cref="SpecRelationType"/>s
        /// </param>
        /// <param name="requirementsSpecifications">
        /// The <see cref="RequirementsSpecification"/>s that needs to be converted to ReqIF content
        /// </param>
        /// <returns>
        /// An instance of <see cref="ReqIF"/>
        /// </returns>
        public ReqIF Build(ReqIF templateReqif, IEnumerable<RequirementsSpecification> requirementsSpecifications, ExportSettings exportSettings)
        {
            if (templateReqif == null)
            {
                throw new ArgumentNullException("the template ReqIF may not be null", nameof(templateReqif));
            }

            if (requirementsSpecifications == null)
            {
                throw new ArgumentNullException("the requirementsSpecifications may not be null", nameof(requirementsSpecifications));
            }

            if (!requirementsSpecifications.Any())
            {
                throw new ArgumentException("the requirementsSpecifications may not be empty", nameof(requirementsSpecifications));
            }

            var targetReqIf = this.CreateTargetReqIF(templateReqif, requirementsSpecifications, exportSettings);

            this.CreateSpecifications(targetReqIf, requirementsSpecifications);
            
            return targetReqIf;
        }

        /// <summary>
        /// Creates the target <see cref="ReqIF"/> object that will contain the same
        /// <see cref="DatatypeDefinition"/> and <see cref="SpecificationType"/>s as the template <see cref="ReqIF"/>
        /// </summary>
        /// <param name="templateReqif">
        /// The template <see cref="ReqIF"/> that is used as basis for the target <see cref="ReqIF"/>
        /// </param>
        /// <returns>
        /// an instance of <see cref="ReqIF"/>
        /// </returns>
        private ReqIF CreateTargetReqIF(ReqIF templateReqif, IEnumerable<RequirementsSpecification> requirementsSpecifications, ExportSettings exportSettings)
        {
            var requirementsSpecification = requirementsSpecifications.First();

            var iteration = (Iteration)requirementsSpecification.Container;
            var engineeringModel = (EngineeringModel)iteration.Container;
            
            var targetReqIf = new ReqIF();
            targetReqIf.TheHeader = new ReqIFHeader()
            {
                Identifier = Guid.NewGuid().ToString(),
                CreationTime = DateTime.UtcNow,
                RepositoryId = $"EngineeringModel\\{engineeringModel.Iid}\\iteration\\{iteration.Iid}",
                ReqIFToolId = "RHEA DEH-REQIF",
                ReqIFVersion = "1.2",
                SourceToolId = "RHEA COMET",
                Title = exportSettings.Title
            };
            
            targetReqIf.CoreContent = new ReqIFContent();

            targetReqIf.CoreContent.DataTypes.AddRange(templateReqif.CoreContent.DataTypes);
            targetReqIf.CoreContent.SpecTypes.AddRange(templateReqif.CoreContent.SpecTypes);
            targetReqIf.ToolExtension.AddRange(templateReqif.ToolExtension);

            return targetReqIf;
        }

        private void CreateSpecifications(ReqIF targetReqIf, IEnumerable<RequirementsSpecification> requirementsSpecifications)
        {
            var specificationType = targetReqIf.CoreContent.SpecTypes.OfType<SpecificationType>().FirstOrDefault();
            var specObjectType = targetReqIf.CoreContent.SpecTypes.OfType<SpecObjectType>().FirstOrDefault();
            
            foreach (var requirementsSpecification in requirementsSpecifications)
            {
                var specification = new Specification();
                specification.Identifier = $"_{Guid.NewGuid()}";
                specification.Type = specificationType;
                specification.Description = string.Empty;
                specification.LongName = requirementsSpecification.Name;
                specification.LastChange = requirementsSpecification.ModifiedOn;

                var specificationAlternativeId = this.CreateAlternativeId(requirementsSpecification);
                specification.AlternativeId = specificationAlternativeId;
                specificationAlternativeId.Ident = specification;
                
                foreach (var requirement in requirementsSpecification.Requirement)
                {
                    var spectObject = new SpecObject();
                    spectObject.Identifier = $"_{Guid.NewGuid()}";
                    spectObject.Type = specObjectType;
                    spectObject.Description = string.Empty;
                    spectObject.LongName = requirement.Name;
                    spectObject.LastChange = requirement.ModifiedOn;

                    var requirementAlternativeId = this.CreateAlternativeId(requirement);
                    spectObject.AlternativeId = requirementAlternativeId;
                    requirementAlternativeId.Ident = spectObject;

                    targetReqIf.CoreContent.SpecObjects.Add(spectObject);
                    spectObject.ReqIFContent = targetReqIf.CoreContent;
                }
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="AlternativeId"/> based on the provided <see cref="Thing"/>
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> that is used to create the <see cref="AlternativeId"/>
        /// </param>
        /// <returns>
        /// An <see cref="AlternativeId"/> where the <see cref="AlternativeId.Identifier"/> property is
        /// set to the <see cref="Thing.Iid"/> property 
        /// </returns>
        private AlternativeId CreateAlternativeId(Thing thing)
        {
            var alternativeId = new AlternativeId
            {
                Identifier = thing.Iid.ToString()
            };

            return alternativeId;
        }
    }
}
