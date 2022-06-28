//  -------------------------------------------------------------------------------------------------
//  <copyright file="AttributeDefinitions.cs" company="RHEA System S.A.">
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

namespace DEHReqIF.ExportSettings
{
    using ReqIFSharp;

    /// <summary>
    /// A class that defines the properties and methods of <see cref="AttributeDefinitions"/> which is part of an <see cref="ExportSettings"/> class
    /// </summary>
    public class AttributeDefinitions
    {
        /// <summary>
        /// Gets or sets the unique identifier of the <see cref="AttributeDefinition"/> that holds the requirement text of a <see cref="SpecObject"/>
        /// </summary>
        public string TextAttributeDefinitionId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AttributeDefinition"/> that holds the Foreign Id from the ReqIF files perspective
        /// </summary>
        public string ForeignDeletedAttributeDefinitionId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AttributeDefinition"/> that holds the creation date from the ReqIF files perspective
        /// </summary>
        public string ForeignModifiedOnAttributeDefinitionId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AttributeDefinition"/> that holds the Name from the ReqIF files perspective
        /// </summary>
        public string NameAttributeDefinitionId { get; set; }
    }
}
