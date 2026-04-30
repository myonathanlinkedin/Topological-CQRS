import json
from scipy import stats
import numpy as np

def run_wilcoxon(deltas_ baseline, deltas_egpo):
    # Wilcoxon signed-rank test on entropy deltas
    stat, p = stats.wilcoxon(deltas_baseline, deltas_egpo)
    return stat, p

def run_mcnemar(correctness_baseline, correctness_egpo):
    # Simplified McNemar representation
    # [[a, b], [c, d]] where b and c are discordant pairs
    b = sum(1 for b, e in zip(correctness_baseline, correctness_egpo) if b and not e)
    c = sum(1 for b, e in zip(correctness_baseline, correctness_egpo) if not b and e)
    
    chi2 = ((abs(b - c) - 1)**2) / (b + c) if (b + c) > 0 else 0
    return chi2

if __name__ == "__main__":
    print("Statistical Test Module Initialized.")
    # In a real run, this would load summary_metrics.json for different modes
