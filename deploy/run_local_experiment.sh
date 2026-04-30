#!/bin/bash

# EGPO Local Experiment Runner
# Verifies environment and runs baseline vs IG+Prov

echo "Checking dependencies..."
docker --version || { echo "Docker not found"; exit 1; }
dotnet --version || { echo ".NET not found"; exit 1; }

# 1. Start Qdrant if not running
if ! docker ps | grep qdrant > /dev/null; then
    echo "Starting Qdrant..."
    docker run -d --name egpo-qdrant -p 6333:6333 -p 6334:6334 qdrant/qdrant:latest
    sleep 5
fi

# 2. Download Real Data if missing
if [ ! -f "experiments/fever_sample.jsonl" ]; then
    echo "fever_sample.jsonl not found. Downloading sample data..."
    python scripts/download_data.py --limit 1000
fi

# 2. Build and Seeding
echo "Building Solution..."
dotnet build src/EGPO.Pipeline/EGPO.Pipeline.csproj

# 3. Run Experiments
echo "Running Baseline experiment..."
dotnet run --project src/EGPO.Pipeline/EGPO.Pipeline.csproj -- Baseline

echo "Running IG+Prov experiment..."
dotnet run --project src/EGPO.Pipeline/EGPO.Pipeline.csproj -- IG+Prov

# 4. Analysis
echo "Running Python Analysis..."
pip install -r analysis/requirements.txt
python analysis/compute_metrics.py

echo "Experiment Round Complete."
