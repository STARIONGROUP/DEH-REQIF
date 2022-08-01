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

namespace DEHReqIF
{
    using System.Collections.Generic;
    
    using CDP4Common.EngineeringModelData;

    using ReqIFSharp;

    /// <summary>
    /// The purpose of the <see cref="IReqIFBuilder"/> is to build/generate a ReqIF file based
    /// om an ECSS-E-TM-10-25 data set (requirements and reference data)
    /// </summary>
    public interface IReqIFBuilder
    {
        /// <summary>
        /// Builds the ReqIF content from the provided <see cref="IEnumerable{RequirementsSpecification}"/>
        /// </summary>
        /// <param name="templateReqif">
        /// The template <see cref="ReqIF"/> used to map <see cref="DatatypeDefinition"/>s, <see cref="SpecObjectType"/>s, <see cref="SpecificationType"/>
        /// and <see cref="SpecRelationType"/>s
        /// </param>
        /// <param name="requirementsSpecification">
        /// The <see cref="RequirementsSpecification"/>s that needs to be converted to ReqIF content
        /// </param>
        /// <param name="exportSettings">
        /// The settings to be used for converting the E-CSS-TM-10-25 objects to existing ReqIF objects in the template ReqIF file
        /// </param>
        /// <param name="excludeAlternativeId">
        /// A value indicating that <ALTERNATIVE-ID /> tags should not be added to the result <see cref="ReqIF"/>.
        /// </param>
        /// <returns>
        /// An instance of <see cref="ReqIF"/>
        /// </returns>
        ReqIF Build(ReqIF templateReqif, IEnumerable<RequirementsSpecification> requirementsSpecification, ExportSettings.ExportSettings exportSettings, bool excludeAlternativeId);
    }
}
