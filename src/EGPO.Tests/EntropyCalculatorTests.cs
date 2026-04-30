using Xunit;
using EGPO.Core;
using System.Collections.Generic;
using System.Linq;

namespace EGPO.Tests
{
    public class EntropyCalculatorTests
    {
        [Fact]
        public void CalculateEntropy_ShouldReturnZero_WhenEmpty()
        {
            var entropy = EntropyCalculator.CalculateEntropy(new List<double>());
            Assert.Equal(0, entropy);
        }

        [Fact]
        public void CalculateEntropy_ShouldBeHigher_WhenDistributionIsUniform()
        {
            var uniformScores = new List<double> { 0.5, 0.5, 0.5 };
            var skewedScores = new List<double> { 0.9, 0.1, 0.0 };

            var hUniform = EntropyCalculator.CalculateEntropy(uniformScores);
            var hSkewed = EntropyCalculator.CalculateEntropy(skewedScores);

            Assert.True(hUniform > hSkewed);
        }

        [Fact]
        public void GetSoftmaxDistribution_ShouldSumToOne()
        {
            var scores = new List<double> { 1.2, 3.4, 0.5, 2.1 };
            var distribution = EntropyCalculator.GetSoftmaxDistribution(scores);

            Assert.Equal(1.0, distribution.Sum(), 5);
        }
    }
}
