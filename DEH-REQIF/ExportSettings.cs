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

namespace DEHReqIF
{
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
        /// Gets or sets the unique identifier of the <see cref="DatatypeDefinition"/> that holds the requirement text of a <see cref="SpecObject"/>
        /// </summary>
        public string RequirementTextDataTypeDefinitionId { get; set; }
    }
}
