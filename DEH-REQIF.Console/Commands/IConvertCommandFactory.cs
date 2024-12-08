//  -------------------------------------------------------------------------------------------------
//  <copyright file="IConvertCommandFactory.cs" company="Starion Group S.A.">
// 
//    Copyright 2022-2024 Starion Group S.A.
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

namespace DEHReqIF.Console.Commands
{
    using Microsoft.Extensions.CommandLineUtils;
    
    /// <summary>
    /// Definition of the <see cref="IConvertCommandFactory"/> interface
    /// </summary>
    public interface IConvertCommandFactory
    {
        /// <summary>
        /// Registers the <see cref="IConvertCommandFactory"/> with the <see cref="CommandLineApplication"/> and
        /// sets the properties of the <see cref="IConvertCommand"/>, ready for execution
        /// </summary>
        /// <param name="commandLineApplication">
        /// the subject <see cref="CommandLineApplication"/>
        /// </param>
        void Register(CommandLineApplication commandLineApplication);
    }
}
