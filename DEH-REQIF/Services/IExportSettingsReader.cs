﻿//  -------------------------------------------------------------------------------------------------
//  <copyright file="IExportSettingsReader.cs" company="Starion Group S.A.">
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
    using System.Threading.Tasks;

    using DEHReqIF.ExportSettings;

    /// <summary>
    /// The purpose of the <see cref="IExportSettingsReader"/> is to read the
    /// <see cref="ExportSettings"/> from a JSON file 
    /// </summary>
    public interface IExportSettingsReader
    {
        /// <summary>
        /// Reads the <see cref="ExportSettings"/> from a JSON file
        /// </summary>
        /// <param name="path">
        /// the file path to the export settings file
        /// </param>
        /// <returns>
        /// An instance of <see cref="ExportSettings"/>
        /// </returns>
        Task<ExportSettings> ReadFile(string path);
    }
}
