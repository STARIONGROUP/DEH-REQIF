//  -------------------------------------------------------------------------------------------------
//  <copyright file="IConvertCommand.cs" company="RHEA System S.A.">
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

namespace DEHReqIF.Console.Commands
{
    /// <summary>
    /// Definition of the <see cref="IConvertCommand"/> interface
    /// </summary>
    public interface IConvertCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the username that is used to connect to the ECSS-E-TM-10-25 <see cref="DataSource"/>
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password that is used to connect to the ECSS-E-TM-10-25 <see cref="DataSource"/>
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the ECSS-E-TM-10-25 source 
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// Gets or sets the path of the source ReqIF template (document)
        /// </summary>
        public string TemplateSource { get; set; }

        /// <summary>
        /// Gets or sets the path of the target ReqIF document
        /// </summary>
        public string TargetReqIF { get; set; }

        /// <summary>
        /// The ShortName of the EngineeringModelSetup
        /// </summary>
        string EngineeringModelIid { get; set; }

        /// <summary>
        /// Gets or sets the location of the export settings file
        /// </summary>
        string ExportSettings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that <ALTERNATIVE-ID /> tags should not be added to the result file
        /// </summary>
        bool ExcludeAlternativeId { get; set; }
    }
}
