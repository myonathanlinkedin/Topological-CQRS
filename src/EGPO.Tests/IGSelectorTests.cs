using Xunit;
using EGPO.Core;
using EGPO.Core.Models;
using System.Collections.Generic;
using System;

namespace EGPO.Tests
{
    public class IGSelectorTests
    {
        [Fact]
        public void Select_ShouldRespectBudget()
        {
            var selector = new IGSelector();
            var candidates = new List<NodeData>
            {
                new() { NodeId = "1", AdjustedScore = 0.9, Tokens = 1000 },
                new() { NodeId = "2", AdjustedScore = 0.8, Tokens = 1500 },
                new() { NodeId = "3", AdjustedScore = 0.7, Tokens = 200 }
            };
            var cache = new PairwiseSimCache();

            var selected = selector.Select(candidates, 2000, cache);

            int totalTokens = 0;
            foreach (var s in selected) totalTokens += s.Tokens;

            Assert.True(totalTokens <= 2000);
        }

        [Fact]
        public void Select_ShouldPenalizeRedundancy()
        {
            var selector = new IGSelector();
            var candidates = new List<NodeData>
            {
                new() { NodeId = "1", AdjustedScore = 0.9, Tokens = 100 },
                new() { NodeId = "2", AdjustedScore = 0.89, Tokens = 100 } // Very similar to 1
            };
            
            var cache = new PairwiseSimCache();
            cache.Add("1", "2", 0.99); // 99% similar

            // With a budget of 150, it should probably only pick one because the second one is too redundant
            var selected = selector.Select(candidates, 150, cache);

            Assert.Single(selected);
        }
    }
}
