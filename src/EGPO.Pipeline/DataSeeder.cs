using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EGPO.Core.Models;
using EGPO.Retriever;
using EGPO.Embedding;
using Newtonsoft.Json;

namespace EGPO.Pipeline
{
    public class DataSeeder
    {
        private readonly QdrantUploader _uploader;
        private readonly EmbeddingService _embeddingService;

        public DataSeeder(QdrantUploader uploader, EmbeddingService embeddingService)
        {
            _uploader = uploader;
            _embeddingService = embeddingService;
        }

        public async Task SeedSampleFeverDataAsync()
        {
            string filePath = "experiments/fever_sample.jsonl";
            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine($"Error: {filePath} not found. Run 'python scripts/download_data.py' first.");
                return;
            }

            Console.WriteLine($"Reading data from {filePath}...");
            var lines = System.IO.File.ReadAllLines(filePath);
            var nodes = new List<NodeData>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                try 
                {
                    var data = JsonConvert.DeserializeObject<dynamic>(line);
                    string text = data?.text ?? data?.claim ?? ""; // handles different FEVER variants
                    string id = data?.id?.ToString() ?? Guid.NewGuid().ToString();

                    nodes.Add(new NodeData
                    {
                        SourceId = id,
                        Text = text,
                        Timestamp = DateTime.UtcNow.AddMinutes(-new Random().Next(10000)), // dummy variance for demo
                        Reliability = 1.0f,
                        Tokens = text.Split(' ').Length * 2,
                        Embedding = _embeddingService.GenerateEmbedding(text)
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Skip line due to error: {ex.Message}");
                }
            }

            await _uploader.UpsertNodesAsync(nodes);
            Console.WriteLine($"Successfully seeded {nodes.Count} nodes to Qdrant.");
        }
    }
}
