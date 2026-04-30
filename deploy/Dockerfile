FROM mcr.microsoft.com/dotnet/sdk:11.0-preview AS build
WORKDIR /app

# Copy all projects
COPY src/ ./src/
COPY EGPO.sln ./

# Restore and build
RUN dotnet restore
RUN dotnet build -c Release

# Final image
FROM mcr.microsoft.com/dotnet/runtime:11.0-preview-alpine
WORKDIR /app
COPY --from=build /app/src/EGPO.Pipeline/bin/Release/net11.0/ ./

# Create directories for experiments
RUN mkdir -p experiments/results experiments/retrieval_snapshots

ENTRYPOINT ["dotnet", "EGPO.Pipeline.dll"]
