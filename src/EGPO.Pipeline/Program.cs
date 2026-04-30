using System;
using System.Threading.Tasks;
using EGPO.Pipeline;
using EGPO.Retriever;
using EGPO.Embedding;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var llmSettings = config.GetSection("LlmSettings");
        var qdrantSettings = config.GetSection("QdrantSettings");
        var egpoSettings = config.GetSection("EgpoSettings");

        Console.WriteLine("EGPO Pipeline Runner");
        Console.WriteLine("--------------------");

        string mode = args.Length > 0 ? args[0] : "IG+Prov";
        int budget = egpoSettings.GetValue<int>("TokenBudget");

        var retriever = new QdrantRetriever(
            qdrantSettings["Host"]!, 
            qdrantSettings.GetValue<int>("Port"), 
            qdrantSettings["CollectionName"]!);
        
        var embedding = new EmbeddingService();
        
        var llm = new LlmService(
            llmSettings["BaseUrl"]!, 
            llmSettings["ModelName"]!,
            llmSettings.GetValue<double>("Temperature"));
            
        var pipeline = new MainPipeline(retriever, embedding, llm, 
            egpoSettings.GetValue<double>("DefaultTemperature"),
            egpoSettings.GetValue<double>("DiversityWeight"),
            egpoSettings.GetValue<double>("Alpha"));

        if (args.Length > 0 && args[0].ToLower() == "seed")
        {
            Console.WriteLine("Mode: DATA SEEDING");
            var uploader = new QdrantUploader(qdrantSettings["Host"]!, qdrantSettings.GetValue<int>("Port"), qdrantSettings["CollectionName"]!);
            var seeder = new DataSeeder(uploader, embedding);
            await seeder.SeedSampleFeverDataAsync();
            return;
        }

        string configPath = mode == "Baseline" ? "experiments/setup_fever.json" : "experiments/setup_fever.json"; // default for now
        if (args.Length > 1) configPath = args[1];

        Console.WriteLine($"Mode: EXPERIMENT ({mode})");
        Console.WriteLine($"Config: {configPath}");

        if (!File.Exists(configPath))
        {
            Console.WriteLine($"Error: Config file not found at {configPath}");
            return;
        }

        var experimentSetup = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(configPath));
        var queries = experimentSetup?.queries;

        if (queries == null)
        {
            Console.WriteLine("Error: No queries found in config.");
            return;
        }

        var summaryList = new List<dynamic>();
        foreach (var queryItem in queries)
        {
            string queryId = queryItem.id;
            string queryText = queryItem.text;

            Console.WriteLine($"\nProcessing Query [{queryId}]: {queryText}");
            
            try
            {
                var result = await pipeline.RunAsync(queryText, mode, budget);

                Console.WriteLine($" -> Selected Nodes: {result.SelectedNodes.Count}");
                Console.WriteLine($" -> Tokens Used: {result.TotalTokens}");
                Console.WriteLine($" -> Entropy: {result.InitialEntropy:F4} -> {result.FinalEntropy:F4}");

                // Save result
                string resultDir = "experiments/results";
                Directory.CreateDirectory(resultDir);
                string resultPath = Path.Combine(resultDir, $"{mode}_{queryId}.json");
                File.WriteAllText(resultPath, JsonConvert.SerializeObject(result, Formatting.Indented));

                summaryList.Add(new {
                   Id = queryId,
                   Nodes = result.SelectedNodes.Count,
                   DeltaH = result.InitialEntropy - result.FinalEntropy,
                   Tokens = result.TotalTokens
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($" -> Error: {ex.Message}");
            }
        }

        Console.WriteLine("\n=== EXPERIMENT SUMMARY TABLE ===");
        Console.WriteLine("| Query ID | Nodes | Delta H | Tokens |");
        Console.WriteLine("|----------|-------|---------|--------|");
        foreach (var s in summaryList)
        {
            Console.WriteLine($"| {s.Id,-8} | {s.Nodes,-5} | {s.DeltaH,7:F3} | {s.Tokens,6} |");
        }

        Console.WriteLine("\nBatch Experiment Complete.");
    }
}
