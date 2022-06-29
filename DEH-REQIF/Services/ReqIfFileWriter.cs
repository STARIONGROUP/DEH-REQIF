//  -------------------------------------------------------------------------------------------------
//  <copyright file="ReqIfFileWriter.cs" company="RHEA System S.A.">
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
    using System.IO;
    using System.IO.Compression;
    using System.Threading;
    using System.Threading.Tasks;

    using ReqIFSharp;

    /// <summary>
    /// Implements of the <see cref="IReqIfFileWriter"/> interface that writes data to a ReqIf file
    /// </summary>
    public class ReqIfFileWriter : IReqIfFileWriter
    {
        /// <summary>
        /// Writes data from a <see cref="ReqIF"/> document to a reqif file and adds it reqifz file.
        /// Both files will be available after this method ends.
        /// </summary>
        /// <param name="targetReqIf">
        /// The <see cref = "ReqIF"/> document to write to disk
        /// </param>
        /// <param name="targetLocation">
        /// The location on disk to write the files to
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> 
        /// </returns>
        public async Task WriteReqIfFiles(ReqIF targetReqIf, string targetLocation)
        {
            var targetReqIFLocation = Path.ChangeExtension(targetLocation, ".reqif");
            var targetReqIFzLocation = Path.ChangeExtension(targetLocation, ".reqifz");
            var entryName = new FileInfo(targetLocation).Name;

            if (File.Exists(targetReqIFLocation))
            {
                File.Delete(targetReqIFLocation);
            }

            await new ReqIFSerializer().SerializeAsync(targetReqIf, targetReqIFLocation, new CancellationToken());

            if (File.Exists(targetReqIFzLocation))
            {
                File.Delete(targetReqIFzLocation);
            }

            using (var zipToOpen = new FileStream(targetReqIFzLocation, FileMode.CreateNew))
            {
                using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    archive.CreateEntryFromFile(targetReqIFLocation, entryName);
                }
            }
        }
    }
}
