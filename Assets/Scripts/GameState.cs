using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {
    private int level;
    private int round;
    private const int maxRounds = 3;

    // The current level's play surface.
    private PushButtonSurface playSurface;

    // TODO refactor this into an actual state enum
    // Game States
    private const int NewLevel = 0;
    private const int ShowWinPattern = 1;
    private const int NextRound = 2;
    private const int EndGame = 99;

    // The current game state.
    private int curState;

    // This array contains references to the surfaces that make up the play area. 
    // These should be assigned from the editor using game objects from the scene that represent the surfaces that contain the push buttons which player will interact with.
    // The push button objects should be child objects of the surface game object and tagged as "PushButton".
    public GameObject[] pushButtonSurfaces;

    // Use this for initialization
    void Start () {
        // Initialize the starting level and round.
        level = 0;
        round = 0;

        curState = NewLevel;
    }

    // Update is called once per frame
    void Update () {
        switch (curState)
        {
            case NewLevel:
                // Increment to next level.
                level++;

                if ((level - 1) >= pushButtonSurfaces.Length)
                {
                    curState = EndGame;
                }
                else
                {
                    // Will be the first round of the new level.
                    round = 1;

                    // Set the current play surface for this level
                    playSurface = pushButtonSurfaces[level - 1].GetComponent<PushButtonSurface>();

                    // Set number of button pushes that should be in the win pattern for this level/round.
                    int numberOfButtonPushesToWin = (2 * playSurface.PushButtonCount) + (round - 2);

                    Debug.Log("***");
                    Debug.Log("Level: " + level + "; Round: " + round + "; Pushes To Win: " + numberOfButtonPushesToWin);

                    // Generate the pattern that corresponds to the correct order of 
                    // button pushes the player must make to win.
                    playSurface.GenerateWinPattern(numberOfButtonPushesToWin);
                    curState = ShowWinPattern;
                }

                break;

            case NextRound:
                // Increment to next round.
                round++;

                if (round <= maxRounds)
                {
                    // TODO Refactor this is similar code as in NewLevel state

                    // Set number of button pushes that should be in the win pattern for this level/round.
                    int numberOfButtonPushesToWin = (2 * playSurface.PushButtonCount) + (round - 2);

                    Debug.Log("===");
                    Debug.Log("Level: " + level + "; Round: " + round + "; Pushes To Win: " + numberOfButtonPushesToWin);

                    // Generate the pattern that corresponds to the correct order of 
                    // button pushes the player must make to win.
                    playSurface.GenerateWinPattern(numberOfButtonPushesToWin);
                    curState = ShowWinPattern;
                }
                else
                {
                    curState = NewLevel;
                }
                break;

            case ShowWinPattern:
                Debug.Log("Showing the win pattern.");
                playSurface.ShowWinPattern();

                /* TODO the next state should be to wait for the players input and evaluate their button presses
                        but for now just running through all levels/rounds to examine log output for correct play sequence. 
                */
                curState = NextRound;
                break;

            case EndGame:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                break;
        }

    }
}
