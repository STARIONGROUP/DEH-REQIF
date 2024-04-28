//  -------------------------------------------------------------------------------------------------
//  <copyright file="ConvertCommandFactory.cs" company="Starion Group S.A.">
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
    using System;
    using System.Linq;

    using Microsoft.Extensions.CommandLineUtils;

    /// <summary>
    /// The purpose of the <see cref="ConvertCommandFactory"/> is to register the
    /// <see cref="IConvertCommand"/> with a <see cref="CommandLineApplication"/>
    /// </summary>
    public class ConvertCommandFactory : IConvertCommandFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertCommandFactory"/>
        /// </summary>
        /// <param name="convertCommand">
        /// The <see cref="IConvertCommand"/> used to execute 
        /// </param>
        public ConvertCommandFactory(IConvertCommand convertCommand)
        {
            this.ConvertCommand = convertCommand;
        }

        /// <summary>
        /// Gets the <see cref="IConvertCommand"/>
        /// </summary>
        public IConvertCommand ConvertCommand { get; private set; }

        /// <summary>
        /// Registers the <see cref="IConvertCommandFactory"/> with the <see cref="CommandLineApplication"/> and
        /// sets the properties of the <see cref="IConvertCommand"/>, ready for execution
        /// </summary>
        /// <param name="commandLineApplication">
        /// the subject <see cref="CommandLineApplication"/>
        /// </param>
        public void Register(CommandLineApplication commandLineApplication)
        {
            commandLineApplication.Description = $"Convert an ECSS-E-TM-10-25 data source into ReqIF document";

            var usernameOption = commandLineApplication.Option("-u|--username <USERNAME>", "The username used to establish an authenticated connection",
                CommandOptionType.SingleValue);

            var passwordOption = commandLineApplication.Option("-s|--secret <SECRET>", "The secret or password used to establish an authenticated connection",
                CommandOptionType.SingleValue);

            var dataSourceOption = commandLineApplication.Option("-d|--datasource <DATASOURCE>", "The uri of the ECSS-E-TM-10-25 data source to convert",
                CommandOptionType.SingleValue);

            var templateSource = commandLineApplication.Option("-sr|--source-reqif <SOURCE_REQIF>", "The path to the ReqIF source template",
                CommandOptionType.SingleValue);

            var targetReqIF = commandLineApplication.Option("-tr|--target-reqif <TARGET_REQIF>", "The path to the ReqIF target file",
                CommandOptionType.SingleValue);

            var exportSettings = commandLineApplication.Option("-es|--export-settings <EXPORTSETTINGS>", "The path to the export setings json file",
                CommandOptionType.SingleValue);

            var engineeringModelIid = commandLineApplication.Option("-ei|--engineeringmodel-id <ENGINEERINGMODEL>", "The Iid of the EngineeringModel to export",
                CommandOptionType.SingleValue);

            var excludeAlternativeId = commandLineApplication.Option("-eai|--exclude-alternative-id", "Exclude ALTERNATIVE-ID tags from result file",
                CommandOptionType.NoValue);

            commandLineApplication.OnExecute(async () =>
            {
                this.ConvertCommand.Username = usernameOption.HasValue() ? usernameOption.Value() : throw new ArgumentNullException("username", "Please provide a username (-u|--username <USERNAME>)");
                this.ConvertCommand.Password = passwordOption.HasValue() ? passwordOption.Value() : throw new ArgumentNullException("passwordOption", "Please provide a password (-s|--secret <SECRET>)");
                this.ConvertCommand.DataSource = dataSourceOption.HasValue() ? dataSourceOption.Value() : throw new ArgumentNullException("dataSourceOption", "Please provide a data source (-d|--datasource <DATASOURCE>)");

                this.ConvertCommand.TemplateSource = templateSource.HasValue() ? templateSource.Value() : throw new ArgumentNullException("templateSource", "Please provide a template reqif file (-sr|--source-reqif <SOURCE_REQIF>)");
                this.ConvertCommand.TargetReqIF = targetReqIF.HasValue() ? targetReqIF.Value() : throw new ArgumentNullException("targetReqIF", "Please provide a target reqif file (-tr|--target-reqif <TARGET_REQIF>)");

                this.ConvertCommand.ExportSettings = exportSettings.HasValue() ? exportSettings.Value() : string.Empty;
                this.ConvertCommand.EngineeringModelIid = engineeringModelIid.HasValue() ? engineeringModelIid.Value() : throw new ArgumentNullException("engineeringModelIid", "Please provide an engineering model iid (-ei|--engineeringmodel-id <ENGINEERINGMODEL>)");

                this.ConvertCommand.ExcludeAlternativeId = excludeAlternativeId.Values?.FirstOrDefault() == "on";

                await this.ConvertCommand.ExecuteAsync();

                return 0;
            });
        }
    }
}
