// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace ExtensionsSample
{
    public static class WebHookSamples
    {
        /// <summary>
        /// Demonstrates binding to <see cref="WebHookContext"/> which enables you to
        /// control the response returned.
        /// </summary>
        public static void HookD([WebHookTrigger] WebHookContext context)
        {
            var obj = context.Request.Content.ReadAsAsync<JObject>().Result;
context.Response = new HttpResponseMessage(HttpStatusCode.Accepted)
{
    Content = new StringContent(string.Format("{{  CToUpper: {0}, OnDate:{1}  }}", ((string)obj["Id"]).ToUpper(CultureInfo.CurrentCulture), System.DateTime.UtcNow.ToString()))
};
        }
    }
}