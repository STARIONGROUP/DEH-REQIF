﻿//  -------------------------------------------------------------------------------------------------
//  <copyright file="TemplateBasedReqIfBuilder.cs" company="Starion Group S.A.">
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
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CDP4Dal;

    using DEHReqIF.ExportSettings;

    using ReqIFSharp;
    using ReqIFSharp.Extensions.Services;

    /// <summary>
    /// Implements of the <see cref="ITemplateBasedReqIfBuilder"/> interface that writes data to a ReqIf file
    /// </summary>
    public class TemplateBasedReqIfBuilder : ITemplateBasedReqIfBuilder
    {
        private readonly IReqIFLoaderService reqIfLoaderService;

        /// <summary>
        /// Creates a new instance of the <see cref="TemplateBasedReqIfBuilder"/> class
        /// </summary>
        /// <param name="reqIfLoaderService">The <see cref="IReqIFLoaderService"/></param>
        public TemplateBasedReqIfBuilder(IReqIFLoaderService reqIfLoaderService)
        {
            this.reqIfLoaderService = reqIfLoaderService;
        }

        /// <summary>
        /// Build a <see cref="ReqIF"/> document based on a template <see cref="ReqIF"/> document using data from an <see cref="ISession"/>
        /// </summary>
        /// <param name="templateSourceLocation">The location of the template source</param>
        /// <param name="session">The <see cref="ISession"/></param>
        /// <param name="exportSettings">The <see cref="ExportSettings"/></param>
        /// <param name="excludeAlternativeId">
        /// A value indicating that <ALTERNATIVE-ID /> tags should not be added to the result <see cref="ReqIF"/>.
        /// </param>
        /// <returns>An awaitable <see cref="Task{T}"/> of type <see cref="ReqIF"/></returns>
        public async Task<ReqIF> Build(string templateSourceLocation, ISession session, ExportSettings exportSettings, bool excludeAlternativeId)
        {
            using var fileStream = new FileStream(templateSourceLocation, FileMode.Open);

            await this.reqIfLoaderService.Load(fileStream, templateSourceLocation.ConvertPathToSupportedFileExtensionKind(), new CancellationToken());

            var templateReqIF = this.reqIfLoaderService.ReqIFData.Single();

            var builder = new ReqIFBuilder();
            var targetReqIf = builder.Build(templateReqIF, session.OpenIterations.First().Key.RequirementsSpecification, exportSettings, excludeAlternativeId);

            return targetReqIf;
        }
    }
}
