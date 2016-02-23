﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Sample.Extension;

namespace ExtensionsSample
{
    public static class SampleSamples
    {
        [NoAutomaticTrigger]
        public static void Sample_BindToStream([Sample(@"sample\path")] Stream stream)
        {
            using (StreamWriter sw = new StreamWriter(stream))
            {
                sw.Write("Sample");
            }
        }

        [NoAutomaticTrigger]
        public static void Sample_BindToString([Sample(@"sample\path")] out string data)
        {
            data = "Sample";
        }

        [NoAutomaticTrigger]
        public static void SampleTrigger([SampleTrigger(@"sample\path")] SampleTriggerValue value)
        {
            Console.WriteLine("Sample trigger job called!");
        }

        [NoAutomaticTrigger]
        public static void SampleTrigger_BindToString([SampleTrigger(@"sample\path")] string value)
        {
            Console.WriteLine("Sample trigger job called!");
        }
    }
}
