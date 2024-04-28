//  -------------------------------------------------------------------------------------------------
//  <copyright file="ISessionDataRetriever.cs" company="Starion Group S.A.">
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

namespace DEHReqIF.Services
{
    using System;
    using System.Threading.Tasks;

    using CDP4Common.EngineeringModelData;

    using CDP4Dal;

    /// <summary>
    /// Definition of the <see cref="ISessionDataRetriever"/> interface
    /// </summary>
    public interface ISessionDataRetriever
    {
        /// <summary>
        /// Opens a <see cref="ISession"/> and retrieves the necessary data
        /// </summary>
        /// <param name="userName">
        /// The user name
        /// </param>
        /// <param name="password">
        /// The password
        /// </param>
        /// <param name="dataSource">
        /// The data source
        /// </param>
        /// <param name="engineeringModelIid">
        /// The unitqueidentifier of the <see cref="EngineeringModel"/> to open
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task{T}"/> of type <see cref="ISession"/>
        /// </returns>
        Task<ISession> OpenSessionAndRetrieveData(string userName, string password, string dataSource, Guid engineeringModelIid);
    }
}
