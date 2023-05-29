using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Text.Json;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Maytaskfp3.ServiceBusTrigger.DTO;

namespace Maytaskfp3.ServiceBusTrigger.Controller
{
    public class Controller
    {

        public class Function1

        {
            protected virtual HttpClient Client { get; set; }

            //private virtual HttpClient client { get; set; };
            public Function1(IHttpClientFactory httpClientFactory)
            {
                Client = httpClientFactory.CreateClient("HttpClient");
            }





            [FunctionName("Function1")]
            public async Task Run([ServiceBusTrigger("jqueue", Connection = "SB:ConnectionString")] string myQueueItem, ILogger log)
            {
                //log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
                Qmodel qmodel = JsonSerializer.Deserialize<Qmodel>(myQueueItem);
                string qmessage = qmodel.message;
                Guid guid = Guid.NewGuid();
                var orbjsonobj = new
                {
                    requestId = guid,
                    message = qmessage
                };
                string orbjson = JsonSerializer.Serialize(orbjsonobj);
                if (myQueueItem != null)
                {


                    string baseUrl = Environment.GetEnvironmentVariable("Endpoint");
                    // Create a request object and set the HTTP method
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, baseUrl);

                    // Set the body content
                    string jsonBody = orbjson;
                    request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");


                    try
                    {

                        //HttpResponseMessage response = await client.SAsync(url);
                        HttpResponseMessage response = await Client.GetAsync(baseUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();
                            log.LogInformation($"the response from http trigger:\n {responseBody}");

                        }
                        else
                        {
                            log.LogInformation($"Request failed with status code: {response.StatusCode}");
                        }

                    }
                    catch (Exception ex)
                    {
                        log.LogInformation($"Error: {ex.Message}");
                    }


                }

            }
        }
    }
}