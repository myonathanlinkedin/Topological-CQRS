import json
import os
import numpy as np

def compute_faithful_precision(selected_nodes, llm_response):
    # Simplified string matching for demonstration
    if not llm_response:
        return 0.0
    
    hits = 0
    response_lower = llm_response.lower()
    for node in selected_nodes:
        if node['Text'].lower()[:50] in response_lower: # check for snippet existence
            hits += 1
            
    return hits / len(selected_nodes) if selected_nodes else 0.0

def process_results(results_dir):
    all_metrics = []
    
    for filename in os.listdir(results_dir):
        if filename.endswith(".json"):
            with open(os.path.join(results_dir, filename), 'r') as f:
                data = json.load(f)
                
                metrics = {
                    "query_id": data['Query']['QueryId'],
                    "initial_entropy": data['InitialEntropy'],
                    "final_entropy": data['FinalEntropy'],
                    "delta_h": data['EntropyDelta'],
                    "tokens": data['TotalTokens'],
                    "faithful_precision": compute_faithful_precision(data['SelectedNodes'], data['LLMResponse'])
                }
                all_metrics.append(metrics)
                
    return all_metrics

if __name__ == "__main__":
    results_path = "experiments/results/"
    if os.path.exists(results_path):
        metrics = process_results(results_path)
        print(f"Computed metrics for {len(metrics)} queries.")
        with open("experiments/results/summary_metrics.json", "w") as f:
            json.dump(metrics, f, indent=2)
    else:
        print("No results found to compute metrics.")
