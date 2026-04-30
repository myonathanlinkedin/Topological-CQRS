// ini adalah alpha version
using System;
using System.Collections.Generic;

namespace TCQRS.Core.Causal
{
    public class CausalNode
    {
        public string EventId { get; set; } = string.Empty;
        public string StreamId { get; set; } = string.Empty;
        public object? Payload { get; set; }
        
        // Logical dependency: This event depends on EventId in Prerequisites
        public List<string> Prerequisites { get; set; } = new();
        
        public DateTime IngestedAt { get; set; } = DateTime.UtcNow;
        
        // For DAG traversal
        public bool IsResolved { get; set; } = false;
    }
}
