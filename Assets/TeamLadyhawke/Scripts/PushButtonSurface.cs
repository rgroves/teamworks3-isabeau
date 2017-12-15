using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PushButtonSurface : MonoBehaviour {
    private const string pushButtonTag = "PushButton";

    private List<GameObject> pushButtons = new List<GameObject>();
    private List<int> winPattern = new List<int>();
    private int playerGuessIdx = 0;

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
        }

        playerGuessIdx = 0;
    }

    public void ShowWinPattern()
    {
        StringBuilder dbg = new StringBuilder("This is the win pattern: ");
        for (int i = 0; i < winPattern.Count; i++)
        {
            int btnIdx = winPattern[i];
            dbg.Append(btnIdx);
            dbg.Append(pushButtons[btnIdx].name);
            dbg.Append((i < winPattern.Count - 1) ? ", " : "");
        }
        Debug.Log(dbg);
    }

    public bool ShowWinPattern(int patternIdx)
    {
        int btnIdx = winPattern[patternIdx];
        Animator anim = pushButtons[btnIdx].GetComponent<Animator>();

        bool activated = anim.GetBool("HasBeenActivated");

        if (!activated)
        {
            anim.SetBool("Activate", true);
        }
        else
        {
            anim.SetBool("HasBeenActivated", false);
            activated = true;
        }
        return activated;
    }

    public int CheckPlayerGuess(GameObject obj)
    {

        if (playerGuessIdx == winPattern.Count)
        {
            Debug.Log("Too many guesses");
            return 0;
        }

        int btnIdx = winPattern[playerGuessIdx++];

        if (pushButtons[btnIdx] == obj)
        {
            if (playerGuessIdx < winPattern.Count)
            {
                Debug.Log("Correct guess, more");
                return 1;
            }
            else
            {
                Debug.Log("Correct guess, no more");
                return 3;
            }
        }
        else
        {
            Debug.Log("Wrong guess");
            return 0;
        }
    }
}
