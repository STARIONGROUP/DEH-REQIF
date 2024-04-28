//  -------------------------------------------------------------------------------------------------
//  <copyright file="ICommand.cs" company="Starion Group S.A.">
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
    using System.Threading.Tasks;

    /// <summary>
    /// Definition of the <see cref="ICommand"/> interface used to define command-line-arguments
    /// </summary>
    /// <remarks>
    /// Inspired by https://gist.github.com/iamarcel/9bdc3f40d95c13f80d259b7eb2bbcabb
    /// </remarks>
    public interface ICommand
    {
        /// <summary>
        /// Executes the <see cref="ICommand"/>
        /// </summary>
        Task ExecuteAsync();
    }
}
