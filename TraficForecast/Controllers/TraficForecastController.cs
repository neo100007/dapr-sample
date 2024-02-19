using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace TraficForecast.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TraficForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<TraficForecastController> _logger;

        public TraficForecastController(ILogger<TraficForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetTraficForecast")]
        public IEnumerable<TraficForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new TraficForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet(Name = "Hello")]
        public string Hello()
        {
            return "Hellow TraficForecast";
        }


        [HttpGet(Name = "Traficstate")]
        public async Task<string> Traficstate()
        {
            string DAPR_STORE_NAME = "statestore";
            //string CONFIG_STORE_NAME = "config-statestore";
            System.Threading.Thread.Sleep(5000);
            using var client = new DaprClientBuilder().Build();
            Random random = new Random();
            int orderId = random.Next(1, 1000);
            //Using Dapr SDK to save and get state
            await client.SaveStateAsync(DAPR_STORE_NAME, "order_1", orderId.ToString());
            await client.SaveStateAsync(DAPR_STORE_NAME, "order_2", orderId.ToString());
           // await client.SaveStateAsync(CONFIG_STORE_NAME, "order_3", orderId.ToString());
            return await client.GetStateAsync<string>(DAPR_STORE_NAME, "order_1");
        }

        [HttpGet(Name = "Traficsecret")]
        public async Task<Dictionary<string, string>> Traficsecret()
        {
            string SECRET_STORE_NAME = "localsecretstore";
            using var client = new DaprClientBuilder().Build();
            //Using Dapr SDK to get a secret
            return await client.GetSecretAsync(SECRET_STORE_NAME, "secret");
        }

        [HttpGet(Name = "Traficconfig")]
        public async Task<string> Traficconfig()
        {
            string CONFIG_STORE_NAME = "configstore";
            using var client = new DaprClientBuilder().Build();
            var configuration = await client.GetConfiguration(CONFIG_STORE_NAME, new List<string>() { "orderId1", "orderId2" });
            string result = string.Empty;
            foreach (var item in configuration.Items)
            {
                result += "{" + item.Key + "} -> {" + item.Value.Value + "}\n";
            }

            return result;
        }
    }
}