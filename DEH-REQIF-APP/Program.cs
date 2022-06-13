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
    using AutofacSerilogIntegration;

    using DEHReqIF.Console.Commands;
    using DEHReqIF.Console.Resources;

    using Microsoft.Extensions.CommandLineUtils;
    using Microsoft.Extensions.Configuration;

    using ReqIFSharp.Extensions.Services;

    using Serilog;

    public static class Program
    {
        /// <summary>
        /// Inversion of Control container
        /// </summary>
        private static IContainer Container { get; set; }

        /// <summary>
        /// Main method that is the entry point for this BatchEditor
        /// </summary>
        /// <param name="args">The arguments</param>
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

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
            Log.Logger.Debug("Configure IoC Container");

            var builder = new ContainerBuilder();

            builder.RegisterType<ResourceLoader>().As<IResourceLoader>();
            builder.RegisterType<ReqIFLoaderService>().As<IReqIFLoaderService>();

            builder.RegisterLogger();

            Log.Logger.Debug("IoC Container Configured");

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
            Log.Logger.Debug("Configure Command Line Arguments");

            var commandLineApplication = new CommandLineApplication();
            commandLineApplication.Name = "DEH-REQIF";
            commandLineApplication.Description = "Console application to convert E-TM-10-25 requirements into a ReqIF document";

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

                Console.WriteLine(scope.Resolve<IResourceLoader>().
                    LoadEmbeddedResource("DEHReqIF.Console.Resources.ascii-art.txt")
                    .Replace("DEHReqIFVersion", DEHReqIFVersion));

                commandLineApplication.Command("convert", scope.Resolve<IConvertCommandFactory>().Register);
            }

            commandLineApplication.Execute(args);

            Log.Logger.Debug("Command Line Arguments Configured");
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
            return assembly.GetName().Version.ToString();
        }
    }
}