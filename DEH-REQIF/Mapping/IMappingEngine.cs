// -------------------------------------------------------------------------------------------------
// <copyright file="IMappingEngine.cs" company="Starion Group S.A.">
//
//   Copyright 2022-2024 Starion Group S.A.
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

namespace DEHReqIF.Mapping
{
    /// <summary>
    /// Interface definition for the <see cref="MappingEngine"/>
    /// </summary>
    public interface IMappingEngine
    {
        /// <summary>
        /// Maps the provided <see cref="object"/> to another type if a rule is found
        /// </summary>
        /// <param name="input">The object to map</param>
        /// <returns>The transformed <paramref name="input"/></returns>
        object Map(object input);
    }
}