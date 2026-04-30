import json
import pandas as pd

def generate_table_1(summary_list):
    # summary_list is a list of dicts from compute_metrics.py
    df = pd.DataFrame(summary_list)
    
    table = {
        "Metric": ["Avg Delta H", "Avg Faithful Precision", "Avg Tokens"],
        "Value": [
            df['delta_h'].mean(),
            df['faithful_precision'].mean(),
            df['tokens'].mean()
        ]
    }
    
    return pd.DataFrame(table)

if __name__ == "__main__":
    print("Table Generator Module Initialized.")
    # Example usage code would follow here
