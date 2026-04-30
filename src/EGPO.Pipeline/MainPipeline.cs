using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EGPO.Core;
using EGPO.Core.Models;
using EGPO.Retriever;
using EGPO.Embedding;

namespace EGPO.Pipeline
{
    public class MainPipeline
    {
        private readonly QdrantRetriever _retriever;
        private readonly EmbeddingService _embeddingService;
        private readonly IGSelector _selector;
        private readonly ProvenanceAdjuster _provenanceAdjuster;
        private readonly LlmService _llmService;

        public MainPipeline(
            QdrantRetriever retriever, 
            EmbeddingService embeddingService,
            LlmService llmService,
            double temperature = 0.5,
            double diversityWeight = 1.0,
            double alpha = 0.001)
        {
            _retriever = retriever;
            _embeddingService = embeddingService;
            _llmService = llmService;
            _selector = new IGSelector(temperature, diversityWeight);
            _provenanceAdjuster = new ProvenanceAdjuster(alpha);
        }

        public async Task<SelectionResult> RunAsync(string queryText, string mode, int tokenBudget)
        {
            // 1. Embedding
            float[] queryEmb = _embeddingService.GenerateEmbedding(queryText);
            
            // 2. Retrieval
            var candidates = await _retriever.SearchAsync(queryEmb, limit: 100);
            
            // Log snapshot
            string queryId = Guid.NewGuid().ToString().Substring(0, 8);
            _retriever.SaveSnapshot(queryId, candidates, "experiments/retrieval_snapshots");

            // 3. Score Adjustment
            DateTime now = DateTime.UtcNow;
            foreach (var node in candidates)
            {
                if (mode == "IG+Prov")
                {
                    node.AdjustedScore = _provenanceAdjuster.AdjustScore(node, now);
                }
                else
                {
                    node.AdjustedScore = node.BaseSimilarity;
                }
            }

            // 4. Distribution Metrics
            double initialEntropy = EntropyCalculator.CalculateEntropy(candidates.Select(c => c.AdjustedScore));

            // 5. Selection
            List<NodeData> selected;
            if (mode == "Baseline")
            {
                selected = candidates.OrderByDescending(c => c.AdjustedScore)
                                     .TakeWhile((c, i) => candidates.Take(i + 1).Sum(x => x.Tokens) < tokenBudget)
                                     .ToList();
            }
            else
            {
                var simCache = new PairwiseSimCache();
                // populate cache (simplified: only for nearby nodes)
                for (int i = 0; i < candidates.Count; i++)
                {
                    for (int j = i + 1; j < candidates.Count; j++)
                    {
                        double sim = PairwiseSimCache.CosineSimilarity(candidates[i].Embedding, candidates[j].Embedding);
                        simCache.Add(candidates[i].NodeId, candidates[j].NodeId, sim);
                    }
                }
                selected = _selector.Select(candidates, tokenBudget, simCache);
            }

            double finalEntropy = EntropyCalculator.CalculateEntropy(selected.Select(s => s.AdjustedScore));

            // 6. LLM Call
            string prompt = _llmService.BuildPrompt(queryText, selected.Select(s => s.Text).ToList());
            string response = await _llmService.CallLlmAsync(prompt);

            return new SelectionResult
            {
                Query = new QueryData { QueryId = queryId, Text = queryText, Embedding = queryEmb },
                SelectedNodes = selected,
                InitialEntropy = initialEntropy,
                FinalEntropy = finalEntropy,
                TotalTokens = selected.Sum(s => s.Tokens),
                LLMResponse = response
            };
        }
    }
}
