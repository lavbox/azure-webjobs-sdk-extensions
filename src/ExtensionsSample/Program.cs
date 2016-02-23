﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using Microsoft.AspNet.WebHooks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions;
using Microsoft.Azure.WebJobs.Extensions.Files;
using Microsoft.Azure.WebJobs.Extensions.SendGrid;
using Microsoft.Azure.WebJobs.Extensions.WebHooks;
using Microsoft.Azure.WebJobs.Host;

namespace ExtensionsSample
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            JobHostConfiguration config = new JobHostConfiguration();
            FilesConfiguration filesConfig = new FilesConfiguration();
            
            // See https://github.com/Azure/azure-webjobs-sdk/wiki/Running-Locally for details
            // on how to set up your local environment
            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
                filesConfig.RootPath = @"c:\temp\files";
            }

            config.UseFiles(filesConfig);
            config.UseTimers();
            config.UseSample();
            config.UseCore();
            
            EnsureSampleDirectoriesExist(filesConfig.RootPath);

            WebHooksConfiguration webHooksConfig = new WebHooksConfiguration();
            webHooksConfig.UseReceiver<GitHubWebHookReceiver>();
            config.UseWebHooks(webHooksConfig);

            JobHost host = new JobHost(config);
            host.RunAndBlock();
        }

        /// <summary>
        /// Set up monitoring + notifications for WebJob errors. This shows how to set things up
        /// manually on startup. You can also use <see cref="ErrorTriggerAttribute"/> to designate
        /// error handler functions.
        /// </summary>
        private static void ConfigureTraceMonitor(JobHostConfiguration config, SendGridConfiguration sendGridConfiguration)
        {
            var notifier = new ErrorNotifier(sendGridConfiguration);

            var traceMonitor = new TraceMonitor()
                .Filter(new SlidingWindowTraceFilter(TimeSpan.FromMinutes(5), 3))
                .Filter(p =>
                {
                    FunctionInvocationException functionException = p.Exception as FunctionInvocationException;
                    return p.Level == TraceLevel.Error && functionException != null &&
                           functionException.MethodName == "ExtensionsSample.FileSamples.ImportFile";
                }, "ImportFile Job Failed")
                .Subscribe(notifier.WebNotify, notifier.EmailNotify)
                .Throttle(TimeSpan.FromMinutes(30));

            config.Tracing.Tracers.Add(traceMonitor);
        }

        private static void EnsureSampleDirectoriesExist(string rootFilesPath)
        {
            // Ensure all the directories referenced by the file sample bindings
            // exist
            Directory.CreateDirectory(rootFilesPath);
            Directory.CreateDirectory(Path.Combine(rootFilesPath, "import"));
            Directory.CreateDirectory(Path.Combine(rootFilesPath, "cache"));
            Directory.CreateDirectory(Path.Combine(rootFilesPath, "convert"));
            Directory.CreateDirectory(Path.Combine(rootFilesPath, "converted"));

            File.WriteAllText(Path.Combine(rootFilesPath, "input.txt"), "WebJobs SDK Extensions!");
        }
    }
}
