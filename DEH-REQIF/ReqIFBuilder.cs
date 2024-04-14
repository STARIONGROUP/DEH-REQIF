// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFBuilder.cs" company="RHEA System S.A.">
//
//   Copyright 2022-2024 RHEA System S.A.
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
    using System.Text.RegularExpressions;
    using System.Web;

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
        /// Gets or sets a value indicating that <ALTERNATIVE-ID /> tags should not be added to the result file
        /// </summary>
        private bool ExcludeAlternativeId { get; set; }

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
        /// <param name="excludeAlternativeId">
        /// A value indicating that <ALTERNATIVE-ID /> tags should not be added to the result <see cref="ReqIF"/>.
        /// </param>
        /// <returns>
        /// An instance of <see cref="ReqIF"/>
        /// </returns>
        public ReqIF Build(
            ReqIF templateReqif, 
            IEnumerable<RequirementsSpecification> requirementsSpecifications, 
            ExportSettings.ExportSettings exportSettings,
            bool excludeAlternativeId
            )
        {
            this.exportSettings = exportSettings;
            this.requirementsSpecifications = requirementsSpecifications.ToList();
            this.templateReqif = templateReqif ?? throw new ArgumentNullException(nameof(templateReqif), "the template ReqIF may not be null");
            this.ExcludeAlternativeId = excludeAlternativeId;

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

            this.targetReqIf.CoreContent.SpecRelations.Clear();
            this.targetReqIf.CoreContent.SpecRelationGroups.Clear();

            foreach (var requirementsSpecification in this.requirementsSpecifications.Where(x => !x.IsDeprecated))
            {
                var specification = new Specification
                {
                    Identifier = $"_{Guid.NewGuid()}",
                    Type = this.specificationType,
                    Description = string.Empty,
                    LongName = requirementsSpecification.Name,
                    LastChange = requirementsSpecification.ModifiedOn
                };

                var specificationAlternativeId = this.CreateAlternativeId(requirementsSpecification, specification);
                specification.AlternativeId = specificationAlternativeId;

                this.targetReqIf.CoreContent.Specifications.Add(specification);
                specification.ReqIFContent = this.targetReqIf.CoreContent;

                try
                {
                    this.CreateSpecObjectAttributes(
                        requirementsSpecification.ParameterValue.ToDictionary(x => x.ParameterType, x => x.Value),
                        requirementsSpecification.Name,
                        requirementsSpecification,
                        specification);
                }
                catch (Exception)
                {
                    logger.Warn($"RequirementsSpecification {requirementsSpecification.ShortName} has errors.");
                    throw;
                }

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
            var specObject = this.CreateSpecObject(requirement);

            try
            {
                this.CreateSpecObjectAttributes(
                    requirement.ParameterValue.ToDictionary(x => x.ParameterType, x => x.Value),
                    requirement.Definition.FirstOrDefault()?.Content ?? string.Empty,
                    requirement,
                    specObject
                );
            }
            catch (Exception)
            {
                logger.Warn($"Requirement {requirement.ShortName} has errors.");
                throw;
            }

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

            try
            {
                this.CreateSpecObjectAttributes(
                    requirementsContainer.ParameterValue.ToDictionary(x => x.ParameterType, x => x.Value),
                    requirementsContainer.Name,
                    requirementsContainer,
                    childGroupSpecObject);
            }
            catch (Exception)
            {
                logger.Warn($"RequirementsGroup {requirementsContainer.ShortName} has errors.");
                throw;
            }

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

            var childGroupSpecificationAlternativeId = this.CreateAlternativeId(definedThing, specObject);
            specObject.AlternativeId = childGroupSpecificationAlternativeId;

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
                var specTypeAttributeDefinitionIds =
                    this.exportSettings
                        .ExternalIdentifierMap
                        .Correspondence
                        .Where(x => x.InternalThing == keyValuePair.Key.Iid)
                        .ToList();

                if (specTypeAttributeDefinitionIds.Any())
                {
                    foreach (var specTypeAttributeDefinitionId in specTypeAttributeDefinitionIds)
                    {
                        var specAttributes = specObject is Specification ? this.specificationType as SpecType : this.specObjectType;

                        var specTypeAttributeDefinitions =
                            specAttributes.SpecAttributes.Where(x => x.Identifier == specTypeAttributeDefinitionId.ExternalId)
                                .ToList();

                        if (specTypeAttributeDefinitions.Any())
                        {
                            foreach (var specTypeAttributeDefinition in specTypeAttributeDefinitions)
                            {
                                var attributeValue = this.CreateAttributeValue(specTypeAttributeDefinition);
                                attributeValue.AttributeDefinition = specTypeAttributeDefinition;

                                if (!this.SetAttributeValueValue(keyValuePair.Key, keyValuePair.Value, specTypeAttributeDefinition, attributeValue))
                                {
                                    continue;
                                }

                                specObject.Values.Add(attributeValue);
                            }
                        }
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
        private bool SetAttributeValueValue(ParameterType parameterType, ValueArray<string> valueArray, AttributeDefinition attributeDefinition, AttributeValue attributeValue)
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

                    var objectValue = value.ToValueSetObject(parameterType).ToString();

                    if (this.exportSettings.AddXhtmlTags)
                    {
                        if (!objectValue.ToLower().Contains("<reqif-xhtml:div>"))
                        {
                            objectValue = $"<reqif-xhtml:div>{HttpUtility.HtmlEncode(objectValue)}</reqif-xhtml:div>";
                        }
                    }
                    else
                    {
                        objectValue = Regex.Replace(HttpUtility.HtmlDecode(objectValue), "<[\\/a-zA-Z0-9= \"\\\"'\'#;:()$_-]*?>", string.Empty);
                    }

                    attributeValue.ObjectValue = objectValue;

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

                var parameterType = this.GetParameterType(attributeDefinition);

                this.SetAttributeValueValue(
                    parameterType, 
                    new ValueArray<string>(new[] { value.ToString() }), 
                    attributeDefinition, 
                    attributeValue);

                specObject.Values.Add(attributeValue);
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="AlternativeId"/> based on the provided <see cref="Thing"/>
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> that is used to create the <see cref="AlternativeId"/>
        /// </param>
        /// <param name="ident">
        /// The parent <see cref="Identifiable"/>
        /// </param>
        /// <returns>
        /// An <see cref="AlternativeId"/> where the <see cref="AlternativeId.Identifier"/> property is
        /// set to the <see cref="Thing.Iid"/> property 
        /// </returns>
        private AlternativeId CreateAlternativeId(Thing thing, Identifiable ident = null)
        {
            if (this.ExcludeAlternativeId)
            {
                return null;
            }

            var alternativeId = new AlternativeId
            {
                Identifier = $"_{thing.Iid}"
            };

            if (ident != null)
            {
                alternativeId.Ident = ident;
            }

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
        /// Creates a <see cref="ParameterType"/> based on its according <see cref="AttributeDefinition"/>
        /// </summary>
        /// <param name="attributeDefinitionType">The <see cref="AttributeDefinition"/></param>
        /// <returns>The <see cref="ParameterType"/></returns>
        private ParameterType GetParameterType(AttributeDefinition attributeDefinitionType)
        {
            switch (attributeDefinitionType)
            {
                case AttributeDefinitionBoolean:
                    return new BooleanParameterType();

                case AttributeDefinitionDate:
                    return new DateTimeParameterType();

                case AttributeDefinitionEnumeration:
                    return new EnumerationParameterType();

                case AttributeDefinitionInteger:
                    return new SimpleQuantityKind();

                case AttributeDefinitionReal:
                    return new SimpleQuantityKind();

                case AttributeDefinitionXHTML:
                    return new TextParameterType();

                default:
                    return new TextParameterType();
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
