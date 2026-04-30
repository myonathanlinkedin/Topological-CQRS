using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using EGPO.Core.Models;

namespace EGPO.Retriever
{
    public class QdrantUploader
    {
        private readonly QdrantClient _client;
        private readonly string _collectionName;

        public QdrantUploader(string host, int port, string collectionName)
        {
            _client = new QdrantClient(host, port);
            _collectionName = collectionName;
        }

        public async Task UpsertNodesAsync(List<NodeData> nodes)
        {
            var points = new List<PointStruct>();

            foreach (var node in nodes)
            {
                var point = new PointStruct
                {
                    Id = Guid.Parse(node.SourceId),
                    Vectors = node.Embedding,
                    Payload =
                    {
                        ["text"] = node.Text,
                        ["timestamp"] = node.Timestamp.ToString("o"),
                        ["reliability"] = node.Reliability,
                        ["tokens"] = node.Tokens,
                        ["source_id"] = node.SourceId
                    }
                };
                points.Add(point);
            }

            await _client.UpsertAsync(_collectionName, points);
        }
    }
}
