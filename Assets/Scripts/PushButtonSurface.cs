using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushButtonSurface : MonoBehaviour {
    private const string pushButtonTag = "PushButton";

    private List<GameObject> pushButtons = new List<GameObject>();
    private List<int> winPattern = new List<int>();

    public int PushButtonCount { get { return pushButtons.Count;  } }

    // Use this for initialization
    void Start () {
        // Find all the push buttons on this surface and store them in the pushButtons list.
        foreach (Transform child in transform)
        {
            if (child.tag == pushButtonTag)
            {
                pushButtons.Add(child.gameObject);
            }
        }

        Debug.Log("pushButtons found: " + PushButtonCount);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GenerateWinPattern(int numberOfButtonActivations)
    {
        Debug.Log("Generating win pattern...");
        // Clear any previous pattern stored.
        winPattern.Clear();

        // Generate the pattern of button pushes that will cause a win condition.
        for (int i = 0; i < numberOfButtonActivations; i++)
        {
            int randomIdx = Random.Range(0, pushButtons.Count);
            winPattern.Add(randomIdx);
            Debug.Log("\tPicked: " + randomIdx);
        }
    }

    public void ShowWinPattern()
    {
        // TODO this should light up the push buttons in the sequence stored in the win pattern. For now just logging details for simulation purposes.

        Debug.Log("This is the win pattern:");
        for (int i = 0; i < winPattern.Count; i++)
        {
            pushButtons[winPattern[i]].GetComponent<PushButton>().Glowing = true;
            Debug.Log("\tPush " + (i + 1) + ": " + winPattern[i]);
        }
    }
}
