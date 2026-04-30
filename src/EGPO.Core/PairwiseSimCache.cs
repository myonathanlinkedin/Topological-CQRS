using System;
using System.Collections.Generic;

namespace EGPO.Core
{
    public class PairwiseSimCache
    {
        private readonly Dictionary<(string, string), double> _cache = new();

        public void Add(string id1, string id2, double similarity)
        {
            var key = GetKey(id1, id2);
            _cache[key] = similarity;
        }

        public double Get(string id1, string id2)
        {
            var key = GetKey(id1, id2);
            return _cache.TryGetValue(key, out var sim) ? sim : 0;
        }

        private static (string, string) GetKey(string id1, string id2)
        {
            return string.Compare(id1, id2, StringComparison.Ordinal) < 0 
                ? (id1, id2) 
                : (id2, id1);
        }

        public static double CosineSimilarity(float[] v1, float[] v2)
        {
            if (v1.Length != v2.Length) return 0;
            double dot = 0;
            double mag1 = 0;
            double mag2 = 0;
            for (int i = 0; i < v1.Length; i++)
            {
                dot += v1[i] * v2[i];
                mag1 += v1[i] * v1[i];
                mag2 += v2[i] * v2[i];
            }
            return dot / (Math.Sqrt(mag1) * Math.Sqrt(mag2));
        }
    }
}
