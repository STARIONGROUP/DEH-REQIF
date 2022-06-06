// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFBuilder.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2022 RHEA System S.A.
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
    using System.Threading.Tasks;

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
        /// <param name="requirementsSpecifications">
        /// The <see cref="RequirementsSpecification"/>s that needs to be converted to ReqIF content
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        public Task Build(IEnumerable<RequirementsSpecification> requirementsSpecifications)
        {
            throw new System.NotImplementedException();
        }
    }
}
