using System;
using System.Collections.Generic;
using System.Linq;
using EGPO.Core.Models;

namespace EGPO.Core
{
    public class IGSelector
    {
        private readonly double _temperature;
        private readonly double _diversityWeight;
        private readonly double _redundancyPenaltyFloor = 1e-3;

        public IGSelector(double temperature = 0.5, double diversityWeight = 1.0)
        {
            _temperature = temperature;
            _diversityWeight = diversityWeight;
        }

        public List<NodeData> Select(
            List<NodeData> candidates, 
            int tokenBudget, 
            PairwiseSimCache simCache)
        {
            var selected = new List<NodeData>();
            var remaining = new List<NodeData>(candidates);
            int currentTokens = 0;

            // Pre-calculate base distribution entropy
            double currentEntropy = EntropyCalculator.CalculateEntropy(
                candidates.Select(c => c.AdjustedScore), _temperature);

            while (remaining.Count > 0 && currentTokens < tokenBudget)
            {
                NodeData? bestNode = null;
                double maxIgRatio = double.MinValue;

                foreach (var node in remaining)
                {
                    if (currentTokens + node.Tokens > tokenBudget) continue;

                    // Apply Redundancy Penalty based on existing 'selected' set and Lambda (Diversity Weight)
                    double redundancyMultiplier = 1.0;
                    foreach (var s in selected)
                    {
                        double sim = simCache.Get(node.NodeId, s.NodeId);
                        // Penalty is scaled by _diversityWeight
                        redundancyMultiplier *= (1.0 - (_diversityWeight * Math.Max(sim, 0)));
                    }
                    
                    // Effective score for entropy calc
                    double effectiveScore = node.AdjustedScore * Math.Max(redundancyMultiplier, _redundancyPenaltyFloor);

                    // Hypothetical new entropy if we added this node's refined influence
                    // (Simplified: we compare entropy of distribution with vs without this node's contribution)
                    // In a true IG loop, we'd add it to the 'context' and re-evaluate distribution.
                    // Here we optimize for Information Gain: IG = H(S) - H(S + node)
                    
                    // We simulate the effect on the set by seeing how much this node uniquely contributes
                    double ig = CalculatePotentialIG(selected, node, remaining, _temperature);
                    double igRatio = ig / Math.Max(node.Tokens, 1);

                    if (igRatio > maxIgRatio)
                    {
                        maxIgRatio = igRatio;
                        bestNode = node;
                    }
                }

                if (bestNode == null || maxIgRatio <= 0) break;

                selected.Add(bestNode);
                remaining.Remove(bestNode);
                currentTokens += bestNode.Tokens;
            }

            return selected;
        }

        private double CalculatePotentialIG(List<NodeData> currentSet, NodeData candidate, List<NodeData> allCandidates, double temp)
        {
            // IG = H(current) - H(current + candidate)
            // We evaluate how the candidate's score affects the total entropy of the candidate space.
            
            var scoresBefore = allCandidates.Select(c => c.AdjustedScore).ToList();
            double hBefore = EntropyCalculator.CalculateEntropy(scoresBefore, temp);

            // After adding candidate, we assume the candidate is 'resolved' or 'fixed', 
            // so we look at the entropy of the REMAINING distribution.
            var remainingScores = allCandidates.Where(c => c.NodeId != candidate.NodeId)
                                               .Select(c => c.AdjustedScore).ToList();
            
            double hAfter = EntropyCalculator.CalculateEntropy(remainingScores, temp);

            // The reduction in entropy is our Information Gain
            double ig = hBefore - hAfter;
            
            // Ensure no negative IG due to numerical precision
            return Math.Max(ig, 1e-6); 
        }
    }
}
