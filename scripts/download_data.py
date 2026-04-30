import os
import argparse
from datasets import load_dataset

def download_fever_slice(output_path, slice_size=1000):
    print(f"Loading BeIR/fever corpus slice (0 to {slice_size})...")
    try:
        # We use BeIR/fever as it contains claims and wikipedia context together
        dataset = load_dataset("BeIR/fever", "corpus", split=f"corpus[:{slice_size}]")
        
        print(f"Saving to {output_path}...")
        dataset.to_json(output_path)
        print("Download complete.")
    except Exception as e:
        print(f"Error downloading dataset: {e}")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Download FEVER dataset slice for EGPO")
    parser.get_default("output")
    parser.add_argument("--limit", type=int, default=1000, help="Number of documents to download")
    parser.add_argument("--output", type=str, default="experiments/fever_sample.jsonl", help="Output path")
    
    args = parser.parse_args()
    
    # Ensure directory exists
    os.makedirs(os.path.dirname(args.output), exist_ok=True)
    
    download_fever_slice(args.output, args.limit)
