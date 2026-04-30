# EGPO Local Experiment Runner (Windows PowerShell)
# Verifies environment and runs baseline vs IG+Prov

Write-Host "Checking dependencies..." -ForegroundColor Cyan
if (!(Get-Command docker -ErrorAction SilentlyContinue)) { Write-Error "Docker not found"; exit 1 }
if (!(Get-Command dotnet -ErrorAction SilentlyContinue)) { Write-Error ".NET not found"; exit 1 }

# 1. Start Qdrant if not running
$qdrantRunning = docker ps --filter "name=egpo-qdrant" --format "{{.Names}}"
if (!$qdrantRunning) {
    Write-Host "Starting Qdrant Container..." -ForegroundColor Yellow
    docker run -d --name egpo-qdrant -p 6333:6333 -p 6334:6334 qdrant/qdrant:latest
    Start-Sleep -Seconds 5
} else {
    Write-Host "Qdrant is already running." -ForegroundColor Green
}

# 2. Download Real Data if missing
if (!(Test-Path "experiments/fever_sample.jsonl")) {
    Write-Host "fever_sample.jsonl not found. Downloading sample data..." -ForegroundColor Yellow
    python scripts/download_data.py --limit 1000
}

# 2. Build Solution
Write-Host "Building Solution..." -ForegroundColor Cyan
dotnet build src/EGPO.Pipeline/EGPO.Pipeline.csproj

# 3. Run Experiments
Write-Host "Running Baseline experiment..." -ForegroundColor Yellow
dotnet run --project src/EGPO.Pipeline/EGPO.Pipeline.csproj -- Baseline

Write-Host "Running IG+Prov experiment..." -ForegroundColor Yellow
dotnet run --project src/EGPO.Pipeline/EGPO.Pipeline.csproj -- IG+Prov

# 4. Analysis
Write-Host "Running Python Analysis..." -ForegroundColor Cyan
if (Get-Command pip -ErrorAction SilentlyContinue) {
    pip install -r analysis/requirements.txt
    python analysis/compute_metrics.py
} else {
    Write-Warning "Python/Pip not found. Skipping analysis step."
}

Write-Host "Experiment Round Complete." -ForegroundColor Green
