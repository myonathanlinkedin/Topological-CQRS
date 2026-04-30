# Topological CQRS & Entropy-Governed Provenance Optimization (EGPO)

![Status](https://img.shields.io/badge/status-alpha-orange?style=for-the-badge)
![License](https://img.shields.io/badge/license-Apache%202.0-blue?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-11-512bd4?style=for-the-badge&logo=dotnet)
![Python](https://img.shields.io/badge/python-3.10%2B-3776ab?style=for-the-badge&logo=python)

## Overview

This repository introduces a high-performance framework designed at the intersection of **Distributed Systems Theory** and **Information Retrieval**. It integrates a **Topological Command Query Responsibility Segregation (T-CQRS)** architecture with an **Entropy-Governed Provenance Optimization (EGPO)** module to resolve the dual challenges of causal consistency in distributed states and information-dense context selection in Graph-RAG systems.

---

## 🏛️ 1. Topological CQRS: Causal Consistency via DAG-Based Materialization

T-CQRS extends traditional CQRS patterns by enforcing strict **Causal Ordering** through Directed Acyclic Graph (DAG) resolution. Unlike eventual consistency models that suffer from reordering anomalies, T-CQRS leverages topological sorting to ensure that state projections are always mathematically valid relative to their antecedents.

### Core Architectural Innovations:
- **Causal Watermarking**: Implements vector-clock-based pruning to prevent orphan state projections and ensure causal eventual consistency.
- **Unmanaged Memory Optimization**: Utilizes **Non-blocking Arena Allocators** (ArrayPool backed) to eliminate Gen 0 GC overhead during high-frequency event ingestion.
- **Dynamic PGO Materializers**: Employs Profile-Guided Optimization patterns for just-in-time state projection, maximizing throughput for topological resolution.

---

## 🔍 2. EGPO: Stochastic Context Selection under Token Constraints

EGPO represents a paradigm shift from traditional Top-K retrieval to **Information-Theoretic Context Selection**. It treats the retrieval problem as a constrained optimization task, seeking to maximize the Information Gain (IG) relative to the query's semantic entropy.

### Theoretical Framework:
The selection objective is defined as the maximization of the entropy delta $\Delta H$ subject to a token budget $B$:

$$ \max_{S \subset C} [H(P_\emptyset) - H(P_S)] \quad \text{s.t.} \quad \sum_{i \in S} \text{tokens}(i) \leq B $$

### Key Features:
- **Provenance-Aware Weighting**: Adjusts relevance scores $r'_i$ using a temporal decay function $\phi(\Delta t) = e^{-\alpha \Delta t}$ and source reliability coefficients $w_{rel}$.
- **Shannon-Entropy Minimization**: Iteratively selects nodes that yield the highest marginal reduction in total candidate space entropy.
- **Redundancy Orthogonalization**: Naturally penalizes semantically redundant nodes by evaluating the information density of the candidate *set* rather than isolated items.

---

## 🛠️ Technical Implementation

### High-Performance Stack
- **Compute Engine**: .NET 11 (C#) utilizing advanced TPL Dataflow for asynchronous DAG evaluation.
- **Vector Engine**: Qdrant (HNSW-indexed vector space with complex payload filtering).
- **Inference Pipeline**: ONNXRuntime for low-latency embedding (`bge-base-en-v1.5`) and LM Studio for LLM-based fidelity evaluation.
- **Analytical Core**: Python 3.10+ (NumPy/SciPy) for rigorous statistical validation (McNemar & Wilcoxon tests).

### Project Layout
- `src/TCQRS.Core/`: Causal primitives and unmanaged memory management.
- `src/TCQRS.Evaluator/`: TPL-driven DAG resolution and topological sorting.
- `src/EGPO.Core/`: Shannon entropy calculator and greedy IG ratio selectors.
- `analysis/`: Quantitative metrics and entropy delta distribution visualization.

---

## 🚀 Getting Started

### Prerequisites
- .NET 11 SDK
- Python 3.10+
- Qdrant Vector Database
- LM Studio (API access for LLM evaluation)

### Quick Build
```bash
# Initialize and build the solution
dotnet build

# Configure analysis environment
pip install -r requirements.txt
```

## Author

**Mateus Yonathan** - [GitHub](https://github.com/myonathanlinkedin)

## License

This project is licensed under the **Apache License 2.0** - see the [LICENSE](LICENSE) file for details.
