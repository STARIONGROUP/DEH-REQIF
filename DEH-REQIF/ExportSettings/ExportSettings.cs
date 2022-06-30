//  -------------------------------------------------------------------------------------------------
//  <copyright file="ExportSettings.cs" company="RHEA System S.A.">
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
    using CDP4Common.EngineeringModelData;

    using ReqIFSharp;

    /// <summary>
    /// The purpose of the <see cref="ExportSettings"/> is to provide settings for the
    /// ReqIF creation process
    /// </summary>
    public class ExportSettings
    {
        /// <summary>
        /// Gets or sets the title of the target ReqIf
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The attribute definitions for <see cref="SpecObject"/> objects that are converted from <see cref="Requirement"/>s and <see cref="RequirementsGroup"/>s
        /// </summary>
        public AttributeDefinitions RequirementAttributeDefinitions { get; set; }

        /// <summary>
        /// The attribute definitions for <see cref="Specification"/> objects that are converted from <see cref="RequirementsSpecification"/>s
        /// </summary>
        public AttributeDefinitions SpecificationAttributeDefinitions { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ExternalIdentifierMap"/> used to map specific E-CSS-TM-10-25 objects to ReqIf objects
        /// </summary>
        public ExternalIdentifierMap ExternalIdentifierMap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the XHTML tags should be added to XHTML type elements in the ReqIF XML
        /// </summary>
        public bool AddXhtmlTags { get; set; }
    }
}
