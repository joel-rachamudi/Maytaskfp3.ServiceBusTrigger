using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Maytaskfp3.ServiceBusTrigger
{   public class Qmodel
    {
        public string message { get; set; }
    }
    public class Function1
    {
        [FunctionName("Function1")]
        public async Task Run([ServiceBusTrigger("jqueue", Connection = "SB:ConnectionString")]string myQueueItem, ILogger log)
        {   
            //log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            Qmodel qmodel= JsonSerializer.Deserialize<Qmodel>(myQueueItem);
            string qmessage=qmodel.message;
            Guid guid = Guid.NewGuid();
            var orbjsonobj = new 
            { requestId = guid,
              message = qmessage
            };
            string orbjson = JsonSerializer.Serialize(orbjsonobj);
            if (myQueueItem != null)
            {

                HttpClient client = new HttpClient();
                string baseUrl = Environment.GetEnvironmentVariable("Endpoint");
                // Create a request object and set the HTTP method
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, baseUrl);

                // Set the body content
                string jsonBody = orbjson;
                request.Content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");

                // Send the request and get the response
                HttpResponseMessage response = await client.SendAsync(request);

                // Check the response status code
                if (response.IsSuccessStatusCode)
                {
                    // Request was successful
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response: " + responseBody);
                }
                else
                {
                    // Request failed
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }
                try
                {

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        log.LogInformation($"the resoponse from http trigger:\n {responseBody}");

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
