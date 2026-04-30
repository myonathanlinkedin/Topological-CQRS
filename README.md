# Topological CQRS & EGPO

![Status](https://img.shields.io/badge/status-alpha-orange?style=for-the-badge)
![License](https://img.shields.io/badge/license-Apache%202.0-blue?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-11-512bd4?style=for-the-badge&logo=dotnet)
![Python](https://img.shields.io/badge/python-3.10%2B-3776ab?style=for-the-badge&logo=python)

An advanced framework for **Distributed Consistency** and **Graph-RAG Optimization**, focusing on high-performance causal processing and entropy-governed information retrieval.

## Overview

This project implements two synergistic systems:
1.  **Topological CQRS (T-CQRS)**: A distributed consistency framework utilizing unmanaged memory management (Arena Allocators) and Directed Acyclic Graph (DAG) logic for causal state materialization.
2.  **Entropy-Governed Provenance Optimization (EGPO)**: An optimization module for Graph-RAG systems that uses information theory to select the most relevant, non-redundant context under strict token budgets.

## Key Features

### 🧩 Topological CQRS
- **Causal Consistency**: Ensures strict ordering of events using vector clocks and DAG-based resolution.
- **High Performance**: Utilizes unmanaged Arena Allocators and TPL Dataflow for efficient memory usage and parallel processing.
- **Dynamic PGO**: Optimized state materialization through dynamic Profile-Guided Optimization patterns.

### 🔍 EGPO (Graph-RAG Optimization)
- **Entropy-Guided Selection**: Implements a selection mechanism based on Shannon entropy and Information Gain (IG) ratios to maximize information density.
- **Provenance Awareness**: Integrates temporal decay and source reliability weighting into the retrieval pipeline.
- **Redundancy Penalization**: Naturally filters out redundant information by evaluating the state of the entire selected set rather than individual nodes.

## Technical Stack

- **Core Logic**: .NET 11 (C#)
- **Vector Store**: Qdrant
- **Inference**: ONNXRuntime for local embedding, LM Studio for LLM evaluation.
- **Analysis**: Python 3.10+ (NumPy, SciPy, Matplotlib)
- **Architecture**: Distributed CQRS with Causal Watermarking.

## Getting Started

### Prerequisites
- .NET 11 SDK
- Python 3.10+
- Qdrant (Local or Docker)
- LM Studio (for evaluation)

### Installation
1.  **Clone the repository**:
    ```bash
    git clone https://github.com/myonathanlinkedin/Topological-CQRS.git
    ```
2.  **Build the .NET Solution**:
    ```bash
    dotnet build
    ```
3.  **Setup Python Analysis Environment**:
    ```bash
    pip install -r requirements.txt
    ```

## Project Structure

- `src/TCQRS.Core/`: Memory management and causal primitives.
- `src/TCQRS.Evaluator/`: DAG resolution and topological sorting.
- `src/EGPO.Core/`: Entropy calculations and IG selection logic.
- `src/EGPO.Retriever/`: Qdrant integration and snapshot management.
- `analysis/`: Statistical evaluation and metric computation scripts.

## Author

**Mateus Yonathan** - [GitHub](https://github.com/myonathanlinkedin)

## License

This project is licensed under the **Apache License 2.0** - see the [LICENSE](LICENSE) file for details.
