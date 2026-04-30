using System;
using System.Collections.Generic;
using System.Linq;

namespace EGPO.Core
{
    public static class EntropyCalculator
    {
        private const double Epsilon = 1e-12;

        /// <summary>
        /// Calculates Shannon Entropy H(P) for a set of scores.
        /// </summary>
        public static double CalculateEntropy(IEnumerable<double> scores, double temperature = 0.5)
        {
            var scoreList = scores.ToList();
            if (!scoreList.Any()) return 0;

            // 1. Softmax with temperature
            double maxScore = scoreList.Max();
            var expScores = scoreList.Select(s => Math.Exp((s - maxScore) / temperature)).ToList();
            double sumExp = expScores.Sum();
            
            var probabilities = expScores.Select(e => e / sumExp).ToList();

            // 2. Shannon Entropy: -sum(p * log2(p))
            double entropy = 0;
            foreach (var p in probabilities)
            {
                // Stability clip
                double clippedP = Math.Clamp(p, Epsilon, 1.0 - Epsilon);
                entropy -= clippedP * Math.Log2(clippedP);
            }

            return entropy;
        }

        public static List<double> GetSoftmaxDistribution(IEnumerable<double> scores, double temperature = 0.5)
        {
            var scoreList = scores.ToList();
            if (!scoreList.Any()) return new List<double>();

            double maxScore = scoreList.Max();
            var expScores = scoreList.Select(s => Math.Exp((s - maxScore) / temperature)).ToList();
            double sumExp = expScores.Sum();
            
            return expScores.Select(e => e / sumExp).ToList();
        }
    }
}
