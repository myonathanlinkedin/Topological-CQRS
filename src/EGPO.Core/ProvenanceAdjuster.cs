using System;
using EGPO.Core.Models;

namespace EGPO.Core
{
    public class ProvenanceAdjuster
    {
        private readonly double _alpha; // Decay rate

        public ProvenanceAdjuster(double alpha = 0.001)
        {
            _alpha = alpha;
        }

        public double AdjustScore(NodeData node, DateTime queryTime)
        {
            // 1. Time Decay: exp(-alpha * delta_t)
            double deltaT = Math.Abs((queryTime - node.Timestamp).TotalHours);
            double timeDecay = Math.Exp(-_alpha * deltaT);

            // 2. Reliability Weighting
            // Reliability should be in range [0.5, 1.0] to avoid complete zeroing unless specified
            double relWeight = Math.Clamp(node.Reliability, 0.1, 1.0);

            // 3. Final Adjusted Score
            return node.BaseSimilarity * timeDecay * relWeight;
        }
    }
}
