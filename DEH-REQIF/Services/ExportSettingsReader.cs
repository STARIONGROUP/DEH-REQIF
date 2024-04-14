//  -------------------------------------------------------------------------------------------------
//  <copyright file="ExportSettingsReader.cs" company="RHEA System S.A.">
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

namespace DEHReqIF.Services
{
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    using DEHReqIF.ExportSettings;

    /// <summary>
    /// The purpose of the <see cref="IExportSettingsReader"/> is to read the
    /// <see cref="ExportSettings"/> from a JSON file 
    /// </summary>
    public class ExportSettingsReader : IExportSettingsReader
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
        public async Task<ExportSettings> ReadFile(string path)
        {
            using var fileStream= File.OpenRead(path);

            var exportSettings = await JsonSerializer.DeserializeAsync<ExportSettings>(fileStream, GetJsonSerializerOptions());

            return exportSettings;
        }

        /// <summary>
        /// Reads the <see cref="ExportSettings"/> from a JSON string
        /// </summary>
        /// <param name="json">
        /// the file path to the export settings file
        /// </param>
        /// <returns>
        /// An instance of <see cref="ExportSettings"/>
        /// </returns>
        public ExportSettings Read(string json)
        {
            var exportSettings = JsonSerializer.Deserialize<ExportSettings>(json, GetJsonSerializerOptions());

            return exportSettings;
        }

        /// <summary>
        /// Retrieve the expected <see cref="JsonSerializerOptions"/>
        /// </summary>
        /// <returns>The <see cref="JsonSerializerOptions"/></returns>
        public static JsonSerializerOptions GetJsonSerializerOptions()
        {
            var serializerOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            };

            return serializerOptions;
        }
    }
}
