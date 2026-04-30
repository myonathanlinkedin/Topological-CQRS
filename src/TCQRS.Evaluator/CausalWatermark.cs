// ini adalah alpha version
using System;
using System.Collections.Concurrent;
using System.Linq;
using TCQRS.Core.Causal;

namespace TCQRS.Evaluator
{
    /// <summary>
    /// Prunes the graph based on Time-To-Live (TTL) to prevent memory leaks from lost packets.
    /// </summary>
    public class CausalWatermark
    {
        private readonly TimeSpan _maxAllowedLatency;

        public CausalWatermark(TimeSpan maxAllowedLatency)
        {
            _maxAllowedLatency = maxAllowedLatency;
        }

        public void Prune(ConcurrentDictionary<string, CausalNode> pendingNodes, Action<CausalNode> onOrphanFound)
        {
            var now = DateTime.UtcNow;
            var orphans = pendingNodes.Values
                .Where(n => (now - n.IngestedAt) > _maxAllowedLatency)
                .ToList();

            foreach (var orphan in orphans)
            {
                if (pendingNodes.TryRemove(orphan.EventId, out _))
                {
                    onOrphanFound(orphan);
                }
            }
        }
    }
}
