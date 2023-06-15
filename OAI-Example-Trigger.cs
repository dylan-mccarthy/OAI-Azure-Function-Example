using System.Net;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class OAI_Example_Trigger
    {
        private readonly ILogger _logger;
        private readonly IAIService _aiService;

        public OAI_Example_Trigger(ILoggerFactory loggerFactory, IAIService aiService)
        {
            _logger = loggerFactory.CreateLogger<OAI_Example_Trigger>();
            _aiService = aiService;
        }

        [Function("OAI_Example_Trigger")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var question = req.Query["question"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody) ?? throw new Exception("Request body was null");
            question = question ?? data?.question;

            if(question == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
            
            var responseString = await _aiService.GetResponse(question);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString(responseString);

            return response;
        }
    }
}
