//  -------------------------------------------------------------------------------------------------
//  <copyright file="ScalarParameterTypeMappingRuleTestFixture.cs" company="RHEA System S.A.">
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

namespace DEHReqIF.Tests.MappingRules.ParameterTypes
{
    using System;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.SiteDirectoryData;
    
    using CDP4Dal;

    using DEHReqIF.MappingRules.ParameterTypes;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="ScalarParameterTypeMappingRule"/>
    /// </summary>
    [TestFixture]
    public class ScalarParameterTypeMappingRuleTestFixture
    {
        private readonly Uri uri = new Uri("https://www.rheagroup.com");
        private Assembler assembler;

        private ScalarParameterTypeMappingRule scalarParameterTypeMappingRule;

        [SetUp]
        public void SetUp()
        {
            var messagebus = new CDPMessageBus();

            this.assembler = new Assembler(this.uri, messagebus);

            this.scalarParameterTypeMappingRule = new ScalarParameterTypeMappingRule();
        }

        [Test]
        public void Verify_that_BooleanParameterType_is_mapped_to_DatatypeDefinitionBoolean()
        {
            var booleanParameterType = new BooleanParameterType(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ShortName = "bool",
                Name = "Boolean",
            };
            
            var datatypeDefinitionBoolean = this.scalarParameterTypeMappingRule.Transform(booleanParameterType);

            Assert.Multiple(() =>
            {
                Assert.That(datatypeDefinitionBoolean.LongName, Is.EqualTo(booleanParameterType.Name));
                Assert.That(datatypeDefinitionBoolean.Description, Is.Null.Or.Empty);
                Assert.That(datatypeDefinitionBoolean.AlternativeId.Identifier, Is.EqualTo(booleanParameterType.Iid.ToString()));
                Assert.That(datatypeDefinitionBoolean.LastChange, Is.LessThanOrEqualTo(DateTime.UtcNow));
            });
        }

        [Test]
        public void Verify_that_BooleanParameterType_with_definition_is_mapped_to_DatatypeDefinitionBoolean()
        {
            var definition = new Definition(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                LanguageCode = "en-GB",
                Content = "this is a boolean"
            };

            var booleanParameterType = new BooleanParameterType(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ShortName = "bool",
                Name = "Boolean",
            };

            booleanParameterType.Definition.Add(definition);
            
            var datatypeDefinitionBoolean = this.scalarParameterTypeMappingRule.Transform(booleanParameterType);

            Assert.Multiple(() =>
            {
                Assert.That(datatypeDefinitionBoolean.LongName, Is.EqualTo(booleanParameterType.Name));
                Assert.That(datatypeDefinitionBoolean.Description, Is.EqualTo(definition.Content));
                Assert.That(datatypeDefinitionBoolean.AlternativeId.Identifier, Is.EqualTo(booleanParameterType.Iid.ToString()));
                Assert.That(datatypeDefinitionBoolean.LastChange, Is.LessThanOrEqualTo(DateTime.UtcNow));
            });
        }

        [Test]
        public void Verify_that_DateParameterType_is_mapped_to_DatatypeDefinitionBoolean()
        {
            var dateParameterType = new DateParameterType(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ShortName = "date",
                Name = "Date",
            };
            
            var datatypeDefinitionDate = this.scalarParameterTypeMappingRule.Transform(dateParameterType);

            Assert.Multiple(() =>
            {
                Assert.That(datatypeDefinitionDate.LongName, Is.EqualTo(dateParameterType.Name));
                Assert.That(datatypeDefinitionDate.Description, Is.Null.Or.Empty);
                Assert.That(datatypeDefinitionDate.AlternativeId.Identifier, Is.EqualTo(dateParameterType.Iid.ToString()));
                Assert.That(datatypeDefinitionDate.LastChange, Is.LessThanOrEqualTo(DateTime.UtcNow));
            });
        }

        [Test]
        public void Verify_that_DateParameterType_with_definition_is_mapped_to_DatatypeDefinitionBoolean()
        {
            var definition = new Definition(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                LanguageCode = "en-GB",
                Content = "this is a date"
            };

            var dateParameterType = new DateParameterType(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ShortName = "date",
                Name = "Date",
            };

            dateParameterType.Definition.Add(definition);

            var datatypeDefinitionDate = this.scalarParameterTypeMappingRule.Transform(dateParameterType);

            Assert.Multiple(() =>
            {
                Assert.That(datatypeDefinitionDate.LongName, Is.EqualTo(dateParameterType.Name));
                Assert.That(datatypeDefinitionDate.Description, Is.EqualTo(definition.Content));
                Assert.That(datatypeDefinitionDate.AlternativeId.Identifier, Is.EqualTo(dateParameterType.Iid.ToString()));
                Assert.That(datatypeDefinitionDate.LastChange, Is.LessThanOrEqualTo(DateTime.UtcNow));
            });
        }

        [Test]
        public void Verify_that_DateTimeParameterType_is_mapped_to_DatatypeDefinitionBoolean()
        {
            var dateTimeParameterType = new DateTimeParameterType(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ShortName = "datetime",
                Name = "Date Time",
            };

            var datatypeDefinitionDate = this.scalarParameterTypeMappingRule.Transform(dateTimeParameterType);

            Assert.Multiple(() =>
            {
                Assert.That(datatypeDefinitionDate.LongName, Is.EqualTo(dateTimeParameterType.Name));
                Assert.That(datatypeDefinitionDate.Description, Is.Null.Or.Empty);
                Assert.That(datatypeDefinitionDate.AlternativeId.Identifier, Is.EqualTo(dateTimeParameterType.Iid.ToString()));
                Assert.That(datatypeDefinitionDate.LastChange, Is.LessThanOrEqualTo(DateTime.UtcNow));
            });
        }

        [Test]
        public void Verify_that_DateTimeParameterType_with_Definition_is_mapped_to_DatatypeDefinitionBoolean()
        {
            var definition = new Definition(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                LanguageCode = "en-GB",
                Content = "this is a date time"
            };

            var dateTimeParameterType = new DateTimeParameterType(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ShortName = "datetime",
                Name = "Date Time",
            };

            dateTimeParameterType.Definition.Add(definition);

            var datatypeDefinitionDate = this.scalarParameterTypeMappingRule.Transform(dateTimeParameterType);

            Assert.Multiple(() =>
            {
                Assert.That(datatypeDefinitionDate.LongName, Is.EqualTo(dateTimeParameterType.Name));
                Assert.That(datatypeDefinitionDate.Description, Is.EqualTo(definition.Content));
                Assert.That(datatypeDefinitionDate.AlternativeId.Identifier, Is.EqualTo(dateTimeParameterType.Iid.ToString()));
                Assert.That(datatypeDefinitionDate.LastChange, Is.LessThanOrEqualTo(DateTime.UtcNow));
            });
        }

        [Test]
        public void Verify_that_EnumerationParameterType_is_mapped_to_DatatypeDefinitionEnumeration()
        {
            var enumerationParameterType = new EnumerationParameterType(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ShortName = "enum",
                Name = "Enumeration",
            };

            var low = new EnumerationValueDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri);
            low.ShortName = "LOW";
            low.Name = "low";

            var high = new EnumerationValueDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri);
            high.ShortName = "HIGH";
            high.Name = "high";

            enumerationParameterType.ValueDefinition.Add(low);
            enumerationParameterType.ValueDefinition.Add(high);

            var datatypeDefinitionEnumeration = (DatatypeDefinitionEnumeration) this.scalarParameterTypeMappingRule.Transform(enumerationParameterType);

            Assert.Multiple(() =>
            {
                Assert.That(datatypeDefinitionEnumeration.LongName, Is.EqualTo(enumerationParameterType.Name));
                Assert.That(datatypeDefinitionEnumeration.Description, Is.Null.Or.Empty);
                Assert.That(datatypeDefinitionEnumeration.AlternativeId.Identifier, Is.EqualTo(enumerationParameterType.Iid.ToString()));
                Assert.That(datatypeDefinitionEnumeration.LastChange, Is.LessThanOrEqualTo(DateTime.UtcNow));

                Assert.That(datatypeDefinitionEnumeration.SpecifiedValues, Has.Count.EqualTo(2));

                var enumValueLow = datatypeDefinitionEnumeration.SpecifiedValues.Single(x => x.LongName == "low");
                Assert.That(enumValueLow.AlternativeId.Identifier, Is.EqualTo(low.Iid.ToString()));
            });
        }

        [Test]
        public void Verify_that_TextParameterType_is_mapped_to_DatatypeDefinitionXHTML()
        {
            var textParameterType = new TextParameterType(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ShortName = "text",
                Name = "Text",
            };
            
            var datatypeDefinitionXhtml = (DatatypeDefinitionXHTML)this.scalarParameterTypeMappingRule.Transform(textParameterType);

            Assert.Multiple(() =>
            {
                Assert.That(datatypeDefinitionXhtml.LongName, Is.EqualTo(textParameterType.Name));
                Assert.That(datatypeDefinitionXhtml.Description, Is.Null.Or.Empty);
                Assert.That(datatypeDefinitionXhtml.AlternativeId.Identifier, Is.EqualTo(textParameterType.Iid.ToString()));
                Assert.That(datatypeDefinitionXhtml.LastChange, Is.LessThanOrEqualTo(DateTime.UtcNow));
            });
        }

        [Test]
        public void Verify_that_QuantityKind_with_INTEGER_NUMBER_SET_is_mapped_to_DatatypeDefinitionInteger()
        {
            var ratioScale = new RatioScale(Guid.NewGuid(), this.assembler.Cache, this.uri);
            ratioScale.NumberSet = NumberSetKind.INTEGER_NUMBER_SET;
            ratioScale.MinimumPermissibleValue = "-5";
            ratioScale.MaximumPermissibleValue = "10";

            var qauntityKind = new SimpleQuantityKind(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ShortName = "quantity",
                Name = "QuantityKind",
            };

            qauntityKind.DefaultScale = ratioScale;
            qauntityKind.PossibleScale.Add(ratioScale);

            var datatypeDefinitionInteger = (DatatypeDefinitionInteger)this.scalarParameterTypeMappingRule.Transform(qauntityKind);

            Assert.Multiple(() =>
            {
                Assert.That(datatypeDefinitionInteger.LongName, Is.EqualTo(qauntityKind.Name));
                Assert.That(datatypeDefinitionInteger.Description, Is.Null.Or.Empty);
                Assert.That(datatypeDefinitionInteger.AlternativeId.Identifier, Is.EqualTo(qauntityKind.Iid.ToString()));
                Assert.That(datatypeDefinitionInteger.LastChange, Is.LessThanOrEqualTo(DateTime.UtcNow));

                Assert.That(datatypeDefinitionInteger.Min, Is.EqualTo(-5));
                Assert.That(datatypeDefinitionInteger.Max, Is.EqualTo(10));
            });
        }

        [Test]
        public void Verify_that_QuantityKind_with_NATURAL_NUMBER_SET_is_mapped_to_DatatypeDefinitionInteger()
        {
            var ratioScale = new RatioScale(Guid.NewGuid(), this.assembler.Cache, this.uri);
            ratioScale.NumberSet = NumberSetKind.NATURAL_NUMBER_SET;
            ratioScale.MinimumPermissibleValue = "5";
            ratioScale.MaximumPermissibleValue = "10";

            var qauntityKind = new SimpleQuantityKind(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ShortName = "quantity",
                Name = "QuantityKind",
            };

            qauntityKind.DefaultScale = ratioScale;
            qauntityKind.PossibleScale.Add(ratioScale);

            var datatypeDefinitionInteger = (DatatypeDefinitionInteger)this.scalarParameterTypeMappingRule.Transform(qauntityKind);

            Assert.Multiple(() =>
            {
                Assert.That(datatypeDefinitionInteger.LongName, Is.EqualTo(qauntityKind.Name));
                Assert.That(datatypeDefinitionInteger.Description, Is.Null.Or.Empty);
                Assert.That(datatypeDefinitionInteger.AlternativeId.Identifier, Is.EqualTo(qauntityKind.Iid.ToString()));
                Assert.That(datatypeDefinitionInteger.LastChange, Is.LessThanOrEqualTo(DateTime.UtcNow));

                Assert.That(datatypeDefinitionInteger.Min, Is.EqualTo(5));
                Assert.That(datatypeDefinitionInteger.Max, Is.EqualTo(10));
            });
        }

        [Test]
        public void Verify_that_QuantityKind_with_RATIONAL_NUMBER_SET_is_mapped_to_DatatypeDefinitionReal()
        {
            var ratioScale = new RatioScale(Guid.NewGuid(), this.assembler.Cache, this.uri);
            ratioScale.NumberSet = NumberSetKind.RATIONAL_NUMBER_SET;
            ratioScale.MinimumPermissibleValue = "5";
            ratioScale.MaximumPermissibleValue = "10";

            var qauntityKind = new SimpleQuantityKind(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ShortName = "quantity",
                Name = "QuantityKind",
            };

            qauntityKind.DefaultScale = ratioScale;
            qauntityKind.PossibleScale.Add(ratioScale);

            var datatypeDefinitionReal = (DatatypeDefinitionReal)this.scalarParameterTypeMappingRule.Transform(qauntityKind);

            Assert.Multiple(() =>
            {
                Assert.That(datatypeDefinitionReal.LongName, Is.EqualTo(qauntityKind.Name));
                Assert.That(datatypeDefinitionReal.Description, Is.Null.Or.Empty);
                Assert.That(datatypeDefinitionReal.AlternativeId.Identifier, Is.EqualTo(qauntityKind.Iid.ToString()));
                Assert.That(datatypeDefinitionReal.LastChange, Is.LessThanOrEqualTo(DateTime.UtcNow));

                Assert.That(datatypeDefinitionReal.Min, Is.EqualTo(5));
                Assert.That(datatypeDefinitionReal.Max, Is.EqualTo(10));
            });
        }

        [Test]
        public void Verify_that_QuantityKind_with_REAL_NUMBER_SET_is_mapped_to_DatatypeDefinitionReal()
        {
            var ratioScale = new RatioScale(Guid.NewGuid(), this.assembler.Cache, this.uri);
            ratioScale.NumberSet = NumberSetKind.REAL_NUMBER_SET;
            ratioScale.MinimumPermissibleValue = "-5.102032";
            ratioScale.MaximumPermissibleValue = "10.234523423";

            var qauntityKind = new SimpleQuantityKind(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ShortName = "quantity",
                Name = "QuantityKind",
            };

            qauntityKind.DefaultScale = ratioScale;
            qauntityKind.PossibleScale.Add(ratioScale);

            var datatypeDefinitionReal = (DatatypeDefinitionReal)this.scalarParameterTypeMappingRule.Transform(qauntityKind);

            Assert.Multiple(() =>
            {
                Assert.That(datatypeDefinitionReal.LongName, Is.EqualTo(qauntityKind.Name));
                Assert.That(datatypeDefinitionReal.Description, Is.Null.Or.Empty);
                Assert.That(datatypeDefinitionReal.AlternativeId.Identifier, Is.EqualTo(qauntityKind.Iid.ToString()));
                Assert.That(datatypeDefinitionReal.LastChange, Is.LessThanOrEqualTo(DateTime.UtcNow));

                Assert.That(datatypeDefinitionReal.Min, Is.EqualTo(-5.102032));
                Assert.That(datatypeDefinitionReal.Max, Is.EqualTo(10.234523423));
            });
        }
    }
}

