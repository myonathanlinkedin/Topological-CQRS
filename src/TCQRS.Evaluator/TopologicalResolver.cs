// ini adalah alpha version
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TCQRS.Core.Causal;

namespace TCQRS.Evaluator
{
    public class TopologicalResolver
    {
        // Waiting sub-graphs: EventId -> Node
        private readonly ConcurrentDictionary<string, CausalNode> _pendingNodes = new();
        
        // Resolved event history (to check prerequisites)
        private readonly ConcurrentDictionary<string, bool> _resolvedIds = new();

        public event Action<CausalNode>? OnNodeResolved;

        public void Ingest(CausalNode node)
        {
            // Check if all prerequisites are already resolved
            bool canResolve = node.Prerequisites.Count == 0 || 
                             node.Prerequisites.All(p => _resolvedIds.ContainsKey(p));

            if (canResolve)
            {
                ResolveInternal(node);
            }
            else
            {
                // Park the node in the DAG
                _pendingNodes.TryAdd(node.EventId, node);
            }
        }

        private void ResolveInternal(CausalNode node)
        {
            node.IsResolved = true;
            _resolvedIds.TryAdd(node.EventId, true);
            
            OnNodeResolved?.Invoke(node);

            // Avalanche resolution: check if any pending nodes can now be resolved
            TriggerAvalanche();
        }

        private void TriggerAvalanche()
        {
            bool changed = true;
            while (changed)
            {
                changed = false;
                var readyToResolve = _pendingNodes.Values
                    .Where(n => n.Prerequisites.All(p => _resolvedIds.ContainsKey(p)))
                    .ToList();

                foreach (var node in readyToResolve)
                {
                    if (_pendingNodes.TryRemove(node.EventId, out _))
                    {
                        node.IsResolved = true;
                        _resolvedIds.TryAdd(node.EventId, true);
                        OnNodeResolved?.Invoke(node);
                        changed = true;
                    }
                }
            }
        }
        
        public int PendingCount => _pendingNodes.Count;
    }
}
