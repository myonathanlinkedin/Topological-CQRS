using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using EGPO.Core.Models;
using Newtonsoft.Json;
using System.IO;

namespace EGPO.Retriever
{
    public class QdrantRetriever
    {
        private readonly QdrantClient _client;
        private readonly string _collectionName;

        public QdrantRetriever(string host, int port, string collectionName)
        {
            _client = new QdrantClient(host, port);
            _collectionName = collectionName;
        }

        public async Task<List<NodeData>> SearchAsync(float[] queryEmbedding, int limit = 50)
        {
            var results = await _client.SearchAsync(
                _collectionName,
                queryEmbedding,
                limit: (uint)limit
            );

            return results.Select(r => new NodeData
            {
                NodeId = r.Id.ToString(),
                Text = r.Payload["text"].StringValue,
                BaseSimilarity = r.Score,
                Timestamp = DateTime.Parse(r.Payload["timestamp"].StringValue),
                Reliability = (float)r.Payload["reliability"].DoubleValue,
                Tokens = (int)r.Payload["tokens"].IntegerValue,
                SourceId = r.Payload["source_id"].StringValue,
                Embedding = r.Vectors.Vector.Data.ToArray()
            }).ToList();
        }

        public void SaveSnapshot(string queryId, List<NodeData> candidates, string snapshotDir)
        {
            if (!Directory.Exists(snapshotDir)) Directory.CreateDirectory(snapshotDir);
            
            var path = Path.Combine(snapshotDir, $"{queryId}.json");
            var json = JsonConvert.SerializeObject(candidates, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
