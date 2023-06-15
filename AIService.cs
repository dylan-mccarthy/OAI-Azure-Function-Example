using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SkillDefinition;

public interface IAIService
{
    Task<string> GetResponse(string input);
}

public class AIService : IAIService
{
    private string _endpoint;
    private string _key;
    private string _model;

    private IKernel _kernel;

    private string _prompt;

    private ISKFunction _exampleFunction;

    public AIService(IConfiguration configuration)
    {
        
        _endpoint = configuration["AIENDPOINT"];
        _key = configuration["AIKEY"];
        _model = configuration["AIMODEL"];

        Console.WriteLine($"Endpoint: {_endpoint}");
        Console.WriteLine($"Key: {_key}");
        Console.WriteLine($"Model: {_model}");

        var builder = new KernelBuilder();
        builder.WithAzureChatCompletionService(_model, _endpoint, _key);
        _kernel = builder.Build();

        _prompt = @"You are an example AI, you give examples of things that the user could do in response to their question.
        Please provide polite answers only, and do not use any offensive language.
        ===================
        {{$input}}
        ===================
        ";

        _exampleFunction = _kernel.CreateSemanticFunction(_prompt, "exampleFunction");
    }
    public async Task<string> GetResponse(string input)
    {
        var response = await _exampleFunction.InvokeAsync(input);
        return await Task.FromResult(response.Result);
    }
}