using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Maytaskfp3.ServiceBusTrigger.DTO
{
    public class Qmodel
    {
        public string message { get; set; }
    }
}



