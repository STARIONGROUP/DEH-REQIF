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
    using CDP4Common.Helpers;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using NLog;

    using ReqIFSharp;

    /// <summary>
    /// The purpose of the <see cref="ReqIFBuilder"/> is to build/generate a ReqIF file based
    /// om an ECSS-E-TM-10-25 data set (requirements and reference data)
    /// </summary>
    public class ReqIFBuilder : IReqIFBuilder
    {
        /// <summary>
        /// Holds the <see cref="ILogger"/> for this class
        /// </summary>
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The <see cref="RequirementsSpecification"/>s
        /// </summary>
        private List<RequirementsSpecification> requirementsSpecifications;

        /// <summary>
        /// The target <see cref="ReqIF"/>
        /// </summary>
        private ReqIF targetReqIf;

        /// <summary>
        /// The <see cref="ExportSettings"/>
        /// </summary>
        private ExportSettings.ExportSettings exportSettings;

        /// <summary>
        /// The template <see cref="ReqIF"/>
        /// </summary>
        private ReqIF templateReqif;

        /// <summary>
        /// The <see cref="SpecObjectType"/>
        /// </summary>
        private SpecObjectType specObjectType;

        /// <summary>
        /// The <see cref="SpecificationType"/>
        /// </summary>
        private SpecificationType specificationType;

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
        /// <param name="exportSettings">
        /// The <see cref="ExportSettings"/>
        /// </param>
        /// <returns>
        /// An instance of <see cref="ReqIF"/>
        /// </returns>
        public ReqIF Build(ReqIF templateReqif, IEnumerable<RequirementsSpecification> requirementsSpecifications, ExportSettings.ExportSettings exportSettings)
        {
            this.exportSettings = exportSettings;
            this.requirementsSpecifications = requirementsSpecifications.ToList();
            this.templateReqif = templateReqif ?? throw new ArgumentNullException(nameof(templateReqif), "the template ReqIF may not be null");

            if (this.requirementsSpecifications == null)
            {
                throw new ArgumentNullException(nameof(requirementsSpecifications), "the requirementsSpecification may not be null");
            }

            if (!this.requirementsSpecifications.Any())
            {
                throw new ArgumentException("the requirementsSpecification may not be empty", nameof(requirementsSpecifications));
            }

            this.CreateTargetReqIF();

            this.ConvertSpecifications();

            return this.targetReqIf;
        }

        /// <summary>
        /// Creates the target <see cref="ReqIF"/> object that will contain the same
        /// <see cref="DatatypeDefinition"/> and <see cref="SpecificationType"/>s as the template <see cref="ReqIF"/>
        /// </summary>
        private void CreateTargetReqIF()
        {
            var requirementsSpecification = this.requirementsSpecifications.First();

            var iteration = (Iteration)requirementsSpecification.Container;
            var engineeringModel = (EngineeringModel)iteration.Container;

            this.targetReqIf = new ReqIF
            {
                TheHeader = new ReqIFHeader()
                {
                    Identifier = Guid.NewGuid().ToString(),
                    CreationTime = DateTime.UtcNow,
                    RepositoryId = $"EngineeringModel\\{engineeringModel.Iid}\\iteration\\{iteration.Iid}",
                    ReqIFToolId = "RHEA DEH-REQIF",
                    ReqIFVersion = "1.2",
                    SourceToolId = "RHEA COMET",
                    Title = this.exportSettings.Title
                },
                CoreContent = new ReqIFContent()
            };

            this.targetReqIf.CoreContent.DataTypes.AddRange(this.templateReqif.CoreContent.DataTypes);
            this.targetReqIf.CoreContent.SpecTypes.AddRange(this.templateReqif.CoreContent.SpecTypes);
            this.targetReqIf.ToolExtension.AddRange(this.templateReqif.ToolExtension);
        }

        /// <summary>
        /// Create Reqif specifications
        /// </summary>
        private void ConvertSpecifications()
        {
            var specificationTypes = this.targetReqIf.CoreContent.SpecTypes.OfType<SpecificationType>().ToList();

            if (specificationTypes.Count > 1)
            {
                logger.Warn($"Multiple {nameof(SpecificationType)}s were found. The first one found is selected as the {nameof(SpecificationType)} to use during conversion.");
            }

            this.specificationType = specificationTypes.FirstOrDefault();

            if (this.specificationType == null)
            {
                throw new NotSupportedException($"The template ReqIF file does not contain a {nameof(SpecificationType)}.");
            }

            var specObjectTypes = this.targetReqIf.CoreContent.SpecTypes.OfType<SpecObjectType>().ToList();

            if (specObjectTypes.Count > 1)
            {
                logger.Warn($"Multiple {nameof(SpecObjectType)}s were found. The first one found is selected as the {nameof(SpecObjectType)} to use during conversion.");
            }

            this.specObjectType = specObjectTypes.FirstOrDefault();

            if (this.specObjectType == null)
            {
                throw new NotSupportedException($"The template ReqIF file does not contain a {nameof(SpecObjectType)}.");
            }

            this.targetReqIf.CoreContent.SpecObjects.Clear();
            this.targetReqIf.CoreContent.Specifications.Clear();

            foreach (var requirementsSpecification in this.requirementsSpecifications)
            {
                var specification = new Specification
                {
                    Identifier = $"_{Guid.NewGuid()}",
                    Type = this.specificationType,
                    Description = string.Empty,
                    LongName = requirementsSpecification.Name,
                    LastChange = requirementsSpecification.ModifiedOn
                };

                var specificationAlternativeId = this.CreateAlternativeId(requirementsSpecification);
                specification.AlternativeId = specificationAlternativeId;
                specificationAlternativeId.Ident = specification;

                this.targetReqIf.CoreContent.Specifications.Add(specification);
                specification.ReqIFContent = this.targetReqIf.CoreContent;

                this.CreateSpecObjectAttributes(
                    requirementsSpecification.ParameterValue.ToDictionary(x => x.ParameterType, x => x.Value),
                    requirementsSpecification.Name,
                    requirementsSpecification,
                    specification);

                foreach (var requirement in requirementsSpecification.Requirement.Where(x => x.Group == null))
                {
                    var specHierarchy = this.ConvertRequirement(requirement);
                    specification.Children.Add(specHierarchy);
                }

                foreach (var requirementsContainer in requirementsSpecification.Group)
                {
                    var specHierarchy = this.ConvertRequirementsGroup(requirementsSpecification.Requirement, requirementsContainer);
                    specification.Children.Add(specHierarchy);
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="Requirement"/> to a <see cref="SpecObject"/> and returns a <see cref="SpecHierarchy"/> for it
        /// </summary>
        /// <param name="requirement">The <see cref="Requirement"/></param>
        /// <returns></returns>
        private SpecHierarchy ConvertRequirement(Requirement requirement)
        {
            // Top level requirements
            var specObject = this.CreateSpecObject(requirement);

            this.CreateSpecObjectAttributes(
                requirement.ParameterValue.ToDictionary(x => x.ParameterType, x => x.Value),
                requirement.Definition.FirstOrDefault()?.Content ?? string.Empty,
                requirement,
                specObject
            );

            return this.CreateSpecHierarchy(specObject);
        }

        /// <summary>
        /// Converts a <see cref="RequirementsGroup"/> to a <see cref="SpecObject"/> and returns its <see cref="SpecHierarchy"/> .
        /// Its underlying hierarchy of child <see cref="RequirementsGroup"/> is also converted
        /// </summary>
        /// <param name="requirements">The root <see cref="Requirement"/>s</param>
        /// <param name="requirementsContainer">The <see cref="RequirementsContainer"/></param>
        /// <returns>The <see cref="SpecHierarchy"/></returns>
        private SpecHierarchy ConvertRequirementsGroup(
            IReadOnlyList<Requirement> requirements,
            RequirementsContainer requirementsContainer
        )
        {
            var childGroupSpecObject = this.CreateSpecObject(requirementsContainer);

            var childGroupSpecHierarcy = this.CreateSpecHierarchy(childGroupSpecObject);

            this.CreateSpecObjectAttributes(
                requirementsContainer.ParameterValue.ToDictionary(x => x.ParameterType, x => x.Value),
                requirementsContainer.Name,
                requirementsContainer,
                childGroupSpecObject);

            foreach (var requirement in requirements.Where(
                         x => x.Group == (requirementsContainer is RequirementsSpecification ? null : requirementsContainer)))
            {
                var specHierarchy = this.ConvertRequirement(requirement);

                childGroupSpecHierarcy.Children.Add(specHierarchy);
            }

            foreach (var childRequirementsContainer in requirementsContainer.Group)
            {
                var specHierarchy = this.ConvertRequirementsGroup(requirements, childRequirementsContainer);
                childGroupSpecHierarcy.Children.Add(specHierarchy);
            }

            return childGroupSpecHierarcy;
        }

        /// <summary>
        /// Create a <see cref="SpecObject"/> based on a <see cref="DefinedThing"/>
        /// </summary>
        /// <param name="definedThing">The <see cref="DefinedThing"/></param>
        /// <returns>A new <see cref="SpecObject"/></returns>
        private SpecObject CreateSpecObject(DefinedThing definedThing)
        {
            var specObject = new SpecObject
            {
                Identifier = $"_{Guid.NewGuid()}",
                Type = this.specObjectType,
                Description = string.Empty,
                LongName = definedThing.Name,
                LastChange = definedThing.ModifiedOn
            };

            this.targetReqIf.CoreContent.SpecObjects.Add(specObject);
            specObject.ReqIFContent = this.targetReqIf.CoreContent;

            var childGroupSpecificationAlternativeId = this.CreateAlternativeId(definedThing);
            specObject.AlternativeId = childGroupSpecificationAlternativeId;
            childGroupSpecificationAlternativeId.Ident = specObject;

            return specObject;
        }

        /// <summary>
        /// Create <see cref="AttributeValue"/>s for a <see cref="SpecObject"/> based on a Dictionary of 10-15 <see cref="ParameterType"/>s and values
        /// </summary>
        /// <param name="parameterTypesAndValues">The <see cref="Requirement"/></param>
        /// <param name="content">The content of SpecObject's ObjectValue</param>
        /// <param name="thing">The original <see cref="Thing"/> for which a <see cref="SpecObject"/> was created</param>
        /// <param name="specObject"><see cref="SpecObject"/></param>
        private void CreateSpecObjectAttributes(
            Dictionary<ParameterType, ValueArray<string>> parameterTypesAndValues,
            string content,
            Thing thing,
            SpecElementWithAttributes specObject)
        {
            var attributeSettings = specObject is Specification ? this.exportSettings.SpecificationAttributeDefinitions : this.exportSettings.RequirementAttributeDefinitions;

            this.CreateExportSettingsRootAttributeValue(specObject, attributeSettings.TextAttributeDefinitionId, content);

            this.CreateExportSettingsRootAttributeValue(specObject, attributeSettings.ForeignModifiedOnAttributeDefinitionId, thing.ModifiedOn);

            if (thing is INamedThing namedThing)
            {
                this.CreateExportSettingsRootAttributeValue(specObject, attributeSettings.NameAttributeDefinitionId, namedThing.Name);
            }

            var isDeprecated = thing.GetContainerOfType<RequirementsSpecification>()?.IsDeprecated ?? false;

            if (thing is IDeprecatableThing deprecatableThing)
            {
                var foreignDeletedAttributeDefinitionId = attributeSettings.ForeignDeletedAttributeDefinitionId;

                isDeprecated = isDeprecated || deprecatableThing.IsDeprecated;

                this.CreateExportSettingsRootAttributeValue(specObject, foreignDeletedAttributeDefinitionId, isDeprecated);
            }

            foreach (var keyValuePair in parameterTypesAndValues)
            {
                var specTypeAttributeDefinitionId =
                    this.exportSettings
                        .ExternalIdentifierMap
                        .Correspondence
                        .SingleOrDefault(x => x.InternalThing == keyValuePair.Key.Iid);

                if (specTypeAttributeDefinitionId != null)
                {
                    var specAttributes = specObject is Specification ? this.specificationType as SpecType : this.specObjectType;

                    var specTypeAttributeDefinition =
                        specAttributes.SpecAttributes.SingleOrDefault(x => x.Identifier == specTypeAttributeDefinitionId.ExternalId);

                    if (specTypeAttributeDefinition != null)
                    {
                        var attributeValue = this.CreateAttributeValue(specTypeAttributeDefinition);
                        attributeValue.AttributeDefinition = specTypeAttributeDefinition;

                        if (!SetAttributeValueValue(keyValuePair.Key, keyValuePair.Value, specTypeAttributeDefinition, attributeValue))
                        {
                            continue;
                        }

                        specObject.Values.Add(attributeValue);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the value property of an <see cref="AttributeValue"/> according to the <see cref="ParameterType"/>'s data type.
        /// </summary>
        /// <param name="parameterType">The <see cref="ParameterType"/></param>
        /// <param name="valueArray">The <see cref="ValueArray{T}"/> of type <see cref="string"/> that contains the E-CSS-TM-10-25 parameter value</param>
        /// <param name="attributeDefinition">The <see cref="AttributeDefinition"/></param>
        /// <param name="attributeValue">The <see cref="AttributeValue"/></param>
        /// <returns>True if the value was set, otherwise false</returns>
        /// <exception cref="NotSupportedException">Throws if the <see cref="ParameterType"/> is not supported.</exception>
        private static bool SetAttributeValueValue(ParameterType parameterType, ValueArray<string> valueArray, AttributeDefinition attributeDefinition, AttributeValue attributeValue)
        {
            var value = valueArray.First();
            
            switch (parameterType)
            {
                case BooleanParameterType :
                    attributeValue.ObjectValue = value.ToValueSetObject(parameterType);
                    break;

                case DateParameterType :
                    attributeValue.ObjectValue = value.ToValueSetObject(parameterType);
                    break;

                case DateTimeParameterType :
                    attributeValue.ObjectValue = value.ToValueSetObject(parameterType);
                    break;

                case EnumerationParameterType :
                    if (attributeDefinition is AttributeDefinitionEnumeration attributeDefinitionEnumeration)
                    {
                        var enumerationSpecTypeAttributeDefinition = attributeDefinitionEnumeration;

                        var enumValue =
                            (enumerationSpecTypeAttributeDefinition.DatatypeDefinition as DatatypeDefinitionEnumeration)?
                            .SpecifiedValues
                            .SingleOrDefault(x => x.LongName == value);

                        if (enumValue == null)
                        {
                            return false;
                        }

                        attributeValue.ObjectValue = new[] { enumValue };
                    }

                    break;
                
                case QuantityKind :
                    attributeValue.ObjectValue = value.ToValueSetObject(parameterType);
                    break;

                case TextParameterType :
                    attributeValue.ObjectValue = value.ToValueSetObject(parameterType);
                    break;

                default:
                    throw new NotSupportedException($"The {parameterType.GetType().AssemblyQualifiedName} type is not supported");
            }

            return true;
        }

        /// <summary>
        /// Create an <see cref="AttributeValue"/> for a property at the root level of the <see cref="ExportSettings"/> object.
        /// </summary>
        /// <param name="specObject"><see cref="SpecObject"/></param>
        /// <param name="exportSettingName"></param>
        /// <param name="value">The value to set on the <see cref="AttributeValue"/></param>
        private void CreateExportSettingsRootAttributeValue(SpecElementWithAttributes specObject, string exportSettingName, object value)
        {
            if (string.IsNullOrWhiteSpace(exportSettingName))
            {
                // No setting found. Please ignore
                return;
            }

            var specAttributes = specObject is Specification ? this.specificationType as SpecType : this.specObjectType;

            var attributeDefinition =
                specAttributes.SpecAttributes.SingleOrDefault(x => x.Identifier == exportSettingName);

            if (attributeDefinition == null)
            {
                logger.Warn($"The expected export setting {exportSettingName} was not found in the template ReqIF file.");
            }
            else
            {
                var attributeValue = this.CreateAttributeValue(attributeDefinition);
                attributeValue.AttributeDefinition = attributeDefinition;
                attributeValue.ObjectValue = value;
                specObject.Values.Add(attributeValue);
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
                Identifier = $"_{thing.Iid}"
            };

            return alternativeId;
        }

        /// <summary>
        /// Creates an <see cref="AttributeValue"/> based on its according <see cref="AttributeDefinition"/>
        /// </summary>
        /// <param name="attributeDefinitionType">The <see cref="AttributeDefinition"/></param>
        /// <returns>An <see cref="AttributeValue"/></returns>
        private AttributeValue CreateAttributeValue(AttributeDefinition attributeDefinitionType)
        {
            switch (attributeDefinitionType)
            {
                case AttributeDefinitionBoolean:
                    return new AttributeValueBoolean();

                case AttributeDefinitionDate:
                    return new AttributeValueDate();

                case AttributeDefinitionEnumeration:
                    return new AttributeValueEnumeration();

                case AttributeDefinitionInteger:
                    return new AttributeValueInteger();

                case AttributeDefinitionReal:
                    return new AttributeValueReal();

                case AttributeDefinitionXHTML:
                    return new AttributeValueXHTML();

                default:
                    return new AttributeValueString();
            }
        }

        /// <summary>
        /// Create a <see cref="SpecHierarchy"/> for a specific <see cref="SpecObject"/>
        /// </summary>
        /// <param name="spectObject">The <see cref="SpecObject"/></param>
        /// <returns>A <see cref="SpecHierarchy"/></returns>
        private SpecHierarchy CreateSpecHierarchy(SpecObject spectObject)
        {
            return new SpecHierarchy
            {
                Identifier = $"_{Guid.NewGuid()}",
                Object = spectObject,
                ReqIfContent = this.targetReqIf.CoreContent
            };
        }
    }
}
