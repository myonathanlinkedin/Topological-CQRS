using System;
using System.Threading.Tasks;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace EGPO.Retriever
{
    public static class QdrantSchemaBuilder
    {
        public static async Task CreateCollectionAsync(QdrantClient client, string collectionName, ulong vectorSize)
        {
            var collections = await client.ListCollectionsAsync();
            if (collections.Contains(collectionName)) return;

            await client.CreateCollectionAsync(collectionName, new VectorParams
            {
                Size = vectorSize,
                Distance = Distance.Cosine
            });

            // Create indexes for metadata
            await client.CreatePayloadIndexAsync(collectionName, "timestamp", PayloadSchemaType.Datetime);
            await client.CreatePayloadIndexAsync(collectionName, "reliability", PayloadSchemaType.Float);
        }
    }
}
