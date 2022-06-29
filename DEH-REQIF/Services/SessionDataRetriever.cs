//  -------------------------------------------------------------------------------------------------
//  <copyright file="SessionDataRetriever.cs" company="RHEA System S.A.">
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

namespace DEHReqIF.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.DAL;

    /// <summary>
    /// Implementation of the <see cref="SessionDataRetriever"/> interface that retrieves data using an <see cref="ISession"/>
    /// </summary>
    public class SessionDataRetriever : ISessionDataRetriever
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
        public async Task<ISession> OpenSessionAndRetrieveData(string userName, string password, string dataSource, Guid engineeringModelIid)
        {
            var dal = new CDP4ServicesDal.CdpServicesDal();
            var credentials = new Credentials(userName, password, new Uri(dataSource));

            var session = new Session(dal, credentials);
            await session.Open(false);

            var siteDirectory = session.RetrieveSiteDirectory();

            await session.Read(siteDirectory.SiteReferenceDataLibrary.First());

            var engineeringModelSetup = siteDirectory.Model.First(x => x.EngineeringModelIid == engineeringModelIid);

            var iterationIid = engineeringModelSetup.IterationSetup.OrderByDescending(x => x.IterationNumber).First(x => !x.IsDeleted).IterationIid;

            var model = new EngineeringModel(engineeringModelSetup.EngineeringModelIid, session.Assembler.Cache, session.Credentials.Uri)
                { EngineeringModelSetup = engineeringModelSetup };

            var iteration = new Iteration(iterationIid, session.Assembler.Cache, session.Credentials.Uri);

            model.Iteration.Add(iteration);

            DomainOfExpertise initialDomain;

            if (session.ActivePerson.DefaultDomain != null && engineeringModelSetup.ActiveDomain.Contains(session.ActivePerson.DefaultDomain))
            {
                initialDomain = session.ActivePerson.DefaultDomain;
            }
            else
            {
                initialDomain = engineeringModelSetup.ActiveDomain.FirstOrDefault();
            }

            await session.Read(iteration, initialDomain);

            return session;
        }
    }
}
