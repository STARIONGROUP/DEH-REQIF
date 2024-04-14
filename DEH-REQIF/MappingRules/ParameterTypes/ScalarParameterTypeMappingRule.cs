//  -------------------------------------------------------------------------------------------------
//  <copyright file="ScalarParameterTypeMappingRule.cs" company="RHEA System S.A.">
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

namespace DEHReqIF.MappingRules.ParameterTypes
{
    using System;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.SiteDirectoryData;

    using ReqIFSharp;

    /// <summary>
    /// The purpose of the <see cref="ScalarParameterTypeMappingRule"/> is to transform a <see cref="ScalarParameterType"/> to
    /// a <see cref="DatatypeDefinition"/>
    /// </summary>
    /// <remarks>
    /// There is only one <see cref="MappingRule{TInput,TOutput}"/> defined since at runtime we need to determine whether a <see cref="QuantityKind"/> is mapped to an
    /// <see cref="DatatypeDefinitionInteger"/> or <see cref="DatatypeDefinitionReal"/>
    /// </remarks>
    public class ScalarParameterTypeMappingRule : MappingRule<ScalarParameterType, DatatypeDefinition>
    {
        /// <summary>
        /// Transforms <see cref="ScalarParameterType"/> to a <see cref="DatatypeDefinition"/>
        /// </summary>
        public override DatatypeDefinition Transform(ScalarParameterType input)
        {
            switch (input)
            {
                case BooleanParameterType booleanParameterType:
                    return this.Transform(booleanParameterType);
                case DateParameterType dateParameterType:
                    return this.Transform(dateParameterType);
                case DateTimeParameterType dateTimeParameterType:
                    return this.Transform(dateTimeParameterType);
                case EnumerationParameterType enumerationParameterType:
                    return this.Transform(enumerationParameterType);
                case QuantityKind quantityKind:
                    return this.Transform(quantityKind);
                case TextParameterType textParameterType:
                    return this.Transform(textParameterType);
                default:
                    throw new NotSupportedException($"The {input.GetType().AssemblyQualifiedName} type is not supported");
            }
        }

        /// <summary>
        /// Transforms <see cref="BooleanParameterType"/> to a <see cref="DatatypeDefinitionBoolean"/>
        /// </summary>
        /// <param name="booleanParameterType">
        /// The <see cref="BooleanParameterType"/> that is to be transformed into a <see cref="DatatypeDefinitionBoolean"/>
        /// </param>
        /// <returns>
        /// an instance of <see cref="DatatypeDefinitionBoolean"/>
        /// </returns>
        private DatatypeDefinitionBoolean Transform(BooleanParameterType booleanParameterType)
        {
            var alternativeId = this.Create(booleanParameterType);

            var datatypeDefinitionBoolean = new DatatypeDefinitionBoolean
            {
                Identifier = $"_{Guid.NewGuid()}",
                LongName = booleanParameterType.Name,
                LastChange = DateTime.UtcNow,
                AlternativeId = alternativeId
            };

            this.UpdateDescription(datatypeDefinitionBoolean, booleanParameterType);
            
            return datatypeDefinitionBoolean;
        }

        /// <summary>
        /// Transforms <see cref="DateParameterType"/> to a <see cref="DatatypeDefinitionDate"/>
        /// </summary>
        /// <param name="dateParameterType">
        /// The <see cref="DateParameterType"/> that is to be transformed into a <see cref="DatatypeDefinitionDate"/>
        /// </param>
        /// <returns>
        /// an instance of <see cref="DatatypeDefinitionDate"/>
        /// </returns>
        private DatatypeDefinitionDate Transform(DateParameterType dateParameterType)
        {
            var alternativeId = this.Create(dateParameterType);

            var datatypeDefinitionDate = new DatatypeDefinitionDate
            {
                Identifier = $"_{Guid.NewGuid()}",
                LongName = dateParameterType.Name,
                LastChange = DateTime.UtcNow,
                AlternativeId = alternativeId
            };

            this.UpdateDescription(datatypeDefinitionDate, dateParameterType);

            return datatypeDefinitionDate;
        }

        /// <summary>
        /// Transforms <see cref="DateTimeParameterType"/> to a <see cref="DatatypeDefinitionDate"/>
        /// </summary>
        /// <param name="dateTimeParameterType">
        /// The <see cref="DateTimeParameterType"/> that is to be transformed into a <see cref="DatatypeDefinitionDate"/>
        /// </param>
        /// <returns>
        /// an instance of <see cref="DatatypeDefinitionDate"/>
        /// </returns>
        private DatatypeDefinitionDate Transform(DateTimeParameterType dateTimeParameterType)
        {
            var alternativeId = this.Create(dateTimeParameterType);

            var datatypeDefinitionDate = new DatatypeDefinitionDate
            {
                Identifier = $"_{Guid.NewGuid()}",
                LongName = dateTimeParameterType.Name,
                LastChange = DateTime.UtcNow,
                AlternativeId = alternativeId
            };

            this.UpdateDescription(datatypeDefinitionDate, dateTimeParameterType);

            return datatypeDefinitionDate;
        }

        /// <summary>
        /// Transforms <see cref="EnumerationParameterType"/> to a <see cref="DatatypeDefinitionEnumeration"/>
        /// </summary>
        /// <param name="enumerationParameterType">
        /// The <see cref="EnumerationParameterType"/> that is to be transformed into a <see cref="DatatypeDefinitionEnumeration"/>
        /// </param>
        /// <returns>
        /// an instance of <see cref="DatatypeDefinitionEnumeration"/>
        /// </returns>
        private DatatypeDefinitionEnumeration Transform(EnumerationParameterType enumerationParameterType)
        {
            var alternativeId = this.Create(enumerationParameterType);

            var datatypeDefinitionEnumeration = new DatatypeDefinitionEnumeration
            {
                Identifier = $"_{Guid.NewGuid()}",
                LongName = enumerationParameterType.Name,
                LastChange = DateTime.UtcNow,
                AlternativeId = alternativeId
            };

            this.UpdateDescription(datatypeDefinitionEnumeration, enumerationParameterType);

            foreach (EnumerationValueDefinition enumerationValueDefinition in enumerationParameterType.ValueDefinition)
            {
                var enumValue = this.Transform(enumerationValueDefinition);

                datatypeDefinitionEnumeration.SpecifiedValues.Add(enumValue);
                enumValue.DataTpeDefEnum = datatypeDefinitionEnumeration;
            }

            return datatypeDefinitionEnumeration;
        }

        /// <summary>
        /// Transforms <see cref="EnumerationValueDefinition"/> to a <see cref="EnumValue"/>
        /// </summary>
        /// <param name="enumerationValueDefinition">
        /// The <see cref="EnumerationValueDefinition"/> that is to be transformed into a <see cref="EnumValue"/>
        /// </param>
        /// <returns>
        /// an instance of <see cref="EnumValue"/>
        /// </returns>
        private EnumValue Transform(EnumerationValueDefinition enumerationValueDefinition)
        {
            var alternativeId = this.Create(enumerationValueDefinition);

            var enumValue = new EnumValue
            {
                Identifier = $"_{Guid.NewGuid()}",
                LongName = enumerationValueDefinition.Name,
                LastChange = DateTime.UtcNow,
                AlternativeId = alternativeId
            };

            return enumValue;
        }

        /// <summary>
        /// Transforms <see cref="QuantityKind"/> to a <see cref="DatatypeDefinitionInteger"/> or a <see cref="DatatypeDefinitionReal"/>
        /// </summary>
        /// <param name="quantityKind">
        /// The <see cref="QuantityKind"/> that is to be transformed
        /// </param>
        /// <returns>
        /// an instance of <see cref="DatatypeDefinitionInteger"/> or a <see cref="DatatypeDefinitionReal"/>
        /// </returns>
        private DatatypeDefinition Transform(QuantityKind quantityKind)
        {
            switch (quantityKind.DefaultScale.NumberSet)
            {
                case NumberSetKind.INTEGER_NUMBER_SET:
                    return this.TransformQuantityKindToDatatypeDefinitionInteger(quantityKind, quantityKind.DefaultScale);
                case NumberSetKind.NATURAL_NUMBER_SET:
                    return this.TransformQuantityKindToDatatypeDefinitionInteger(quantityKind, quantityKind.DefaultScale);
                case NumberSetKind.RATIONAL_NUMBER_SET:
                    return this.TransformQuantityKindToDatatypeDefinitionReal(quantityKind, quantityKind.DefaultScale);
                case NumberSetKind.REAL_NUMBER_SET:
                    return this.TransformQuantityKindToDatatypeDefinitionReal(quantityKind, quantityKind.DefaultScale);
            }

            throw new InvalidOperationException($"The {quantityKind.Name} could not be transformed");
        }

        /// <summary>
        /// Transforms <see cref="QuantityKind"/> to a <see cref="DatatypeDefinitionInteger"/>
        /// </summary>
        /// <param name="quantityKind">
        /// The <see cref="QuantityKind"/> that is to be transformed into a <see cref="DatatypeDefinitionInteger"/>
        /// </param>
        /// <returns>
        /// an instance of <see cref="DatatypeDefinitionInteger"/>
        /// </returns>
        private DatatypeDefinitionInteger TransformQuantityKindToDatatypeDefinitionInteger(QuantityKind quantityKind, MeasurementScale measurementScale)
        {
            var alternativeId = this.Create(quantityKind);

            var datatypeDefinitionInteger = new DatatypeDefinitionInteger
            {
                Identifier = $"_{Guid.NewGuid()}",
                LongName = quantityKind.Name,
                LastChange = DateTime.UtcNow,
                AlternativeId = alternativeId
            };

            if (long.TryParse(measurementScale.MaximumPermissibleValue, out long maxValue))
            {
                datatypeDefinitionInteger.Max = maxValue;
            }
            else
            {
                // TODO: log error
            }

            if (long.TryParse(measurementScale.MinimumPermissibleValue, out long minValue))
            {
                datatypeDefinitionInteger.Min = minValue;
            }
            else
            {
                // TODO: log error
            }
            
            this.UpdateDescription(datatypeDefinitionInteger, quantityKind);

            return datatypeDefinitionInteger;
        }

        /// <summary>
        /// Transforms <see cref="QuantityKind"/> to a <see cref="DatatypeDefinitionReal"/>
        /// </summary>
        /// <param name="quantityKind">
        /// The <see cref="QuantityKind"/> that is to be transformed into a <see cref="DatatypeDefinitionReal"/>
        /// </param>
        /// <param name="measurementScale">
        /// The <see cref="MeasurementScale"/> that is to be used for the mapping
        /// </param>
        /// <returns>
        /// an instance of <see cref="DatatypeDefinitionReal"/>
        /// </returns>
        private DatatypeDefinitionReal TransformQuantityKindToDatatypeDefinitionReal(QuantityKind quantityKind, MeasurementScale measurementScale)
        {
            var alternativeId = this.Create(quantityKind);

            var datatypeDefinitionReal = new DatatypeDefinitionReal
            {
                Identifier = $"_{Guid.NewGuid()}",
                LongName = quantityKind.Name,
                LastChange = DateTime.UtcNow,
                AlternativeId = alternativeId
            };

            if (double.TryParse(measurementScale.MaximumPermissibleValue, out double maxValue))
            {
                datatypeDefinitionReal.Max = maxValue;
            }
            else
            {
                // TODO: log error
            }

            if (double.TryParse(measurementScale.MinimumPermissibleValue, out double minValue))
            {
                datatypeDefinitionReal.Min = minValue;
            }
            else
            {
                // TODO: log error
            }

            this.UpdateDescription(datatypeDefinitionReal, quantityKind);

            return datatypeDefinitionReal;
        }

        /// <summary>
        /// Transforms <see cref="TextParameterType"/> to a <see cref="DatatypeDefinitionXHTML"/>
        /// </summary>
        /// <param name="textParameterType">
        /// The <see cref="TextParameterType"/> that is to be transformed into a <see cref="DatatypeDefinitionXHTML"/>
        /// </param>
        /// <returns>
        /// an instance of <see cref="DatatypeDefinitionXHTML"/>
        /// </returns>
        private DatatypeDefinitionXHTML Transform(TextParameterType textParameterType)
        {
            var alternativeId = this.Create(textParameterType);

            var datatypeDefinitionXhtml = new DatatypeDefinitionXHTML
            {
                Identifier = $"_{Guid.NewGuid()}",
                LongName = textParameterType.Name,
                LastChange = DateTime.UtcNow,
                AlternativeId = alternativeId
            };

            this.UpdateDescription(datatypeDefinitionXhtml, textParameterType);

            return datatypeDefinitionXhtml;
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
        private AlternativeId Create(Thing thing)
        {
            var alternativeId = new AlternativeId
            {
                Identifier = thing.Iid.ToString()
            };

            return alternativeId;
        }

        /// <summary>
        /// Updates the description of the <see cref="DatatypeDefinition"/> using the first
        /// <see cref="Definition"/> of the provided <see cref="DefinedThing"/>
        /// </summary>
        /// <param name="datatypeDefinition">
        /// The <see cref="DatatypeDefinition"/> that is to be updated
        /// </param>
        /// <param name="definedThing">
        /// The <see cref="DefinedThing"/> from which the first <see cref="Definition"/> is
        /// used to update the Description of the <see cref="DatatypeDefinition"/>
        /// </param>
        private void UpdateDescription(DatatypeDefinition datatypeDefinition, DefinedThing definedThing)
        {
            var definition = definedThing.Definition.FirstOrDefault();
            if (definition != null)
            {
                datatypeDefinition.Description = definition.Content;
            }
        }
    }
}