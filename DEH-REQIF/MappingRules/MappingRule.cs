//  -------------------------------------------------------------------------------------------------
//  <copyright file="IMappingRule.cs" company="RHEA System S.A.">
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

namespace DEHReqIF.MappingRules
{
    using System;

    /// <summary>
    /// Represents a Mappable property usable by the <see cref="MappingEngine"/>
    /// </summary>
    /// <typeparam name="TInput">The input <see cref="Type"/></typeparam>
    /// <typeparam name="TOutput">The output <see cref="Type"/></typeparam>
    public abstract class MappingRule<TInput, TOutput> : IMappingRule<TInput, TOutput>
    {
        /// <summary>
        /// Transforms <see cref="TInput"/> to a <see cref="TOutput"/>
        /// </summary>
        public abstract TOutput Transform(TInput input);
    }
}