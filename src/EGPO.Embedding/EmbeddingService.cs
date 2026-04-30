using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace EGPO.Embedding
{
    public class EmbeddingService : IDisposable
    {
        private InferenceSession? _session;
        private readonly string _modelPath;

        public EmbeddingService(string modelPath = "models/bge-base-en-v1.5.onnx")
        {
            _modelPath = modelPath;
            // In a real environment, we'd load the session here
            // _session = new InferenceSession(_modelPath);
        }

        public float[] GenerateEmbedding(string text)
        {
            // Placeholder: Returning a dummy normalized vector for demonstration
            // Actual implementation would involve Tokenization -> ONNX Inference -> Pooling -> Normalization
            var rnd = new Random(text.GetHashCode());
            float[] vec = new float[768];
            double sumSq = 0;
            for (int i = 0; i < 768; i++)
            {
                vec[i] = (float)rnd.NextDouble();
                sumSq += vec[i] * vec[i];
            }
            double mag = Math.Sqrt(sumSq);
            return vec.Select(v => (float)(v / mag)).ToArray();
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}
