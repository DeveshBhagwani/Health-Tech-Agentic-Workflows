import sys
import json

def process_prescription(data):
    medication = data.get("MedicationName", "").lower()
    cost = float(data.get("Cost", 0))

    if "oxycodone" in medication or "fentanyl" in medication:
        return {"Status": "Denied", "Reason": "Controlled substance policy restricts automatic approval."}
    
    if cost > 500:
        return {"Status": "Pending", "Reason": "Cost exceeds $500 threshold. Requires manual clinical review."}
    
    if "amoxicillin" in medication or "lisinopril" in medication:
        return {"Status": "Approved", "Reason": "Standard tier 1 medication."}

    return {"Status": "Approved", "Reason": "Cost within limits and medication not restricted."}

def main():
    if len(sys.argv) < 2:
        print(json.dumps({"Status": "Error", "Reason": "No JSON payload provided."}))
        sys.exit(1)

    try:
        input_json = sys.argv[1]
        data = json.loads(input_json)
        
        result = process_prescription(data)
        
        print(json.dumps(result))
    except Exception as e:
        print(json.dumps({"Status": "Error", "Reason": str(e)}))
        sys.exit(1)

if __name__ == "__main__":
    main()
