using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;

namespace FaaSPractice
{
    public static class FunctionSentenceCreator
    {
        static readonly HttpClient client = new HttpClient();

        [FunctionName("Sentence")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {



            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string sentence = "";
            sentence += await client.GetStringAsync("http://localhost:7071/api/Subject");
            sentence += " ";
            sentence += await client.GetStringAsync("http://localhost:7071/api/Verb");
            sentence += " ";
            sentence += await client.GetStringAsync("http://localhost:7071/api/DirectObject");
            sentence += ".";


            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(sentence);
        }
    }
}
