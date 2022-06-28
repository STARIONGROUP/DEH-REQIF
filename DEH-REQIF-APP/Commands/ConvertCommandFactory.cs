//  -------------------------------------------------------------------------------------------------
//  <copyright file="ConvertCommandFactory.cs" company="RHEA System S.A.">
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

namespace DEHReqIF.Console.Commands
{
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

            commandLineApplication.OnExecute(async () =>
            {
                this.ConvertCommand.Username = usernameOption.HasValue() ? usernameOption.Value() : string.Empty;
                this.ConvertCommand.Password = passwordOption.HasValue() ? passwordOption.Value() : string.Empty;
                this.ConvertCommand.DataSource = dataSourceOption.HasValue() ? dataSourceOption.Value() : string.Empty;

                this.ConvertCommand.TemplateSource = templateSource.HasValue() ? templateSource.Value() : string.Empty;
                this.ConvertCommand.TargetReqIF = targetReqIF.HasValue() ? targetReqIF.Value() : string.Empty;

                this.ConvertCommand.ExportSettings = exportSettings.HasValue() ? exportSettings.Value() : string.Empty;
                this.ConvertCommand.EngineeringModelIid = engineeringModelIid.HasValue() ? engineeringModelIid.Value() : string.Empty;
                
                await this.ConvertCommand.ExecuteAsync();

                return 0;
            });
        }
    }
}
