// -------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="RHEA System S.A.">
//
//   Copyright 2022 RHEA System S.A.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace DEHReqIF.Console
{
    using System;

    using Autofac;

    using DEHReqIF.Console.Commands;
    using DEHReqIF.Console.Resources;
    using DEHReqIF.Services;

    using Microsoft.Extensions.CommandLineUtils;
    using Microsoft.Extensions.Configuration;

    using NLog;
    using NLog.Extensions.Logging;

    using ReqIFSharp;
    using ReqIFSharp.Extensions.Services;

    public static class Program
    {
        /// <summary>
        /// Inversion of Control container
        /// </summary>
        private static IContainer Container { get; set; }

        private static ILogger logger = LogManager.Setup().LoadConfiguration(
            builder => 
            {
                builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToConsole();
            })
            .GetCurrentClassLogger();

        /// <summary>
        /// Main method that is the entry point for this BatchEditor
        /// </summary>
        /// <param name="args">The arguments</param>
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));

            Container = ConfigureContainer();

            ConfigureCommandLineApplication(args);
        }

        /// <summary>
        /// Configure the IOC container
        /// </summary>
        /// <returns>
        /// an instance of <see cref="IContainer"/>
        /// </returns>
        private static IContainer ConfigureContainer()
        {
            logger.Debug("Configure IoC Container");

            var builder = new ContainerBuilder();

            builder.RegisterType<ResourceLoader>().As<IResourceLoader>();
            builder.RegisterType<ReqIFDeserializer>().As<IReqIFDeSerializer>();
            builder.RegisterType<ReqIFLoaderService>().As<IReqIFLoaderService>();
            builder.RegisterType<SessionDataRetriever>().As<ISessionDataRetriever>();
            builder.RegisterType<ReqIfFileWriter>().As<IReqIfFileWriter>();
            builder.RegisterType<ExportSettingsReader>().As<IExportSettingsReader>();
            builder.RegisterType<TemplateBasedReqIfBuilder>().As<ITemplateBasedReqIfBuilder>();
            builder.RegisterType<ConvertCommand>().As<IConvertCommand>();
            builder.RegisterType<ConvertCommandFactory>().As<IConvertCommandFactory>();

            logger.Debug("IoC Container Configured");

            return builder.Build();
        }

        /// <summary>
        /// Configure the <see cref="CommandLineApplication"/> with actions
        /// </summary>
        /// <param name="args">
        /// The command line arguments
        /// </param>
        public static void ConfigureCommandLineApplication(string[] args)
        {
            logger.Debug("Configure Command Line Arguments");

            var commandLineApplication = new CommandLineApplication
            {
                Name = "DEH-REQIF",
                Description = "Console application to convert E-TM-10-25 requirements into a ReqIF document"
            };

            commandLineApplication.HelpOption("-?|--help");

            commandLineApplication.OnExecute(() =>
            {
                Console.WriteLine();
                Console.WriteLine("        use -? or --help to display help");
                return 0;
            });

            using (var scope = Container.BeginLifetimeScope())
            {
                var DEHReqIFVersion = QueryVersion();

                Console.WriteLine(scope.Resolve<IResourceLoader>()
                    .LoadEmbeddedResource("DEHReqIF.Console.Resources.ascii-art.txt")
                    .Replace("DEHReqIFVersion", DEHReqIFVersion));

                commandLineApplication.Command("convert", scope.Resolve<IConvertCommandFactory>().Register);
            }

            commandLineApplication.Execute(args);

            logger.Debug("Command Line Arguments Configured");
        }

        /// <summary>
        /// queries the version number from the executing assembly
        /// </summary>
        /// <returns>
        /// a string representation of the version of the application
        /// </returns>
        public static string QueryVersion()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return assembly.GetName().Version?.ToString();
        }
    }
}