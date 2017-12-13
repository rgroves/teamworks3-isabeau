using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {
    // The number of levels for each game.
    public int NumberOfLevels = 1;

    // The number of rounds for each level.
    public int NumberOfRounds = 1;

    // A LevelManager responsible for managing the current level/round.
    public LevelManager levelManager;

    // This array contains references to the surfaces that make up the play area. 
    // These should be assigned from the editor using game objects from the scene that represent the surfaces that contain the push buttons which player will interact with.
    // The push button objects should be child objects of the surface game object and tagged as "PushButton".
    public GameObject[] pushButtonSurfaces;

    // The current level's play surface.
    private PushButtonSurface playSurface;

     // Game States
    private enum State
    {
        NewGame,
        NewLevel,
        NewRound,
        GenerateWinPattern,
        ShowWinPattern,
        CollectPlayerInput,
        EndGame
    }

    // The current game state.
    private State currentState;

    // Use this for initialization
    void Start () {
        currentState = State.NewGame;
    }

    // Update is called once per frame
    void Update () {
        switch (currentState)
        {
            case State.NewGame:
                levelManager.NewGame(NumberOfLevels, NumberOfRounds);
                currentState = State.NewLevel;
                break;

            case State.NewLevel:
                levelManager.NewLevel();

                // If there are more levels to play then continue play otherwise transition to game over.
                currentState = levelManager.HasMoreLevels ? State.GenerateWinPattern : State.EndGame;
                break;

            case State.NewRound:
                levelManager.NewRound();

                // If there are more rounds in the current level then continue play otherwise transition to next level.
                currentState = levelManager.HasMoreRounds ? State.GenerateWinPattern : State.NewLevel;
                break;

            case State.GenerateWinPattern:
                int level = levelManager.CurrentLevel;
                int round = levelManager.CurrentRound;

                // Set the current play surface for this level
                playSurface = pushButtonSurfaces[level - 1].GetComponent<PushButtonSurface>();

                // Set number of button pushes that should be in the win pattern for this level/round.
                int numberOfButtonPushesToWin = (2 * playSurface.PushButtonCount) + (round - 2);

                Debug.Log(">>> " + levelManager.CurrentLevelRound + ", Pushes To Win: " + numberOfButtonPushesToWin + " <<<");

                // Generate the pattern that corresponds to the correct order of 
                // button pushes the player must make to win.
                playSurface.GenerateWinPattern(numberOfButtonPushesToWin);

                currentState = State.ShowWinPattern;
                break;

            case State.ShowWinPattern:
                playSurface.ShowWinPattern();

                /* TODO the next state should be to wait for the players input and evaluate their button presses
                        but for now just running through all levels/rounds to examine log output for correct play sequence. 
                */
                currentState = State.NewRound;
                break;

            case State.EndGame:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                break;
        }

    }
}
