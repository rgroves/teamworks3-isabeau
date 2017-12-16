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
    private PushButtonSurface currentPlaySurface;

    // Number of button pushes needed to win for this round.
    private int numberOfButtonPushesToWin;

    // Game States
    private enum State
    {
        NewGame,
        NewLevel,
        NewRound,
        WaitForPlayerStartAction,
        GenerateWinPattern,
        ShowWinPattern,
        ShowingWinPattern,
        CollectPlayerGuesses,
        EndGame
    }

    // The current game state.
    private State currentState;

    // Use this for initialization
    private void Start () {
        currentState = State.NewGame;
    }

    // Update is called once per frame
    private void Update () {
        switch (currentState)
        {
            case State.NewGame:
                levelManager.NewGame(NumberOfLevels, NumberOfRounds);
                currentState = State.NewLevel;
                break;

            case State.NewLevel:
                levelManager.NewLevel();

                if (levelManager.HasMoreLevels)
                {
                    // Set the current play surface for this level and make it ready to play.
                    currentPlaySurface = pushButtonSurfaces[levelManager.CurrentLevel - 1].GetComponent<PushButtonSurface>();
                    currentPlaySurface.Mode = PushButtonSurface.SurfaceMode.READY_TO_PLAY;
                    currentPlaySurface.ClearPlayerGuesses();

                    // Update state to wait for player to start.
                    currentState = State.WaitForPlayerStartAction;
                }
                else
                {
                    currentPlaySurface = null;
                    currentState = State.EndGame;
                }

                break;

            case State.NewRound:
                levelManager.NewRound();
                currentPlaySurface.ClearPlayerGuesses();

                // If there are more rounds in the current level then continue play otherwise transition to next level.
                currentState = levelManager.HasMoreRounds ? State.GenerateWinPattern : State.NewLevel;
                break;

            case State.WaitForPlayerStartAction:
                // Nothing to do here. Wating for player to click the power 
                // button which will trigger a call to OnPowerButtonPush() 
                // which will transition the state.
                break;

            case State.GenerateWinPattern:
                // Set number of button pushes that should be in the win pattern for this level/round.
                numberOfButtonPushesToWin = (2 * currentPlaySurface.PushButtonCount) + (levelManager.CurrentRound - 2);

                Debug.Log(">>> " + levelManager.CurrentLevelRound + ", Pushes To Win: " + numberOfButtonPushesToWin + " <<<");

                // Generate the pattern that corresponds to the correct order of 
                // button pushes the player must make to win.
                currentPlaySurface.GenerateWinPattern(numberOfButtonPushesToWin);
                currentPlaySurface.LogWinPattern();

                currentState = State.ShowWinPattern;
                break;

            case State.ShowWinPattern:
                currentPlaySurface.ShowWinPattern();
                currentState = State.ShowingWinPattern;
                break;

            case State.ShowingWinPattern:
                if (!currentPlaySurface.IsShowingWinPattern)
                {
                    currentState = State.CollectPlayerGuesses;
                }
                break;

            case State.CollectPlayerGuesses:
                if (currentPlaySurface.HasBegunCheckingGuesses)
                {
                    if (currentPlaySurface.IsCorrectGuessCount && currentPlaySurface.IsLastPlayerGuessCorrect && currentPlaySurface.HasCheckDelayEnded)
                    {
                        currentPlaySurface.Mode = PushButtonSurface.SurfaceMode.PLAYED;
                        currentState = State.NewRound;
                    }
                    else if (!currentPlaySurface.IsLastPlayerGuessCorrect)
                    {
                        currentState = State.EndGame;
                    }
                }
                break;

            case State.EndGame:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                break;
        }
    }

    public void OnButtonPush(PushButton buttonPushed)
    {
        Debug.Log("Push Button Pushed");

        if (currentState != State.CollectPlayerGuesses)
        {
            return;
        }

        currentPlaySurface.StorePlayerGuess(buttonPushed);

        currentState = State.CollectPlayerGuesses;
    }

    public void OnPowerButtonPush(GameObject buttonPushed)
    {
        Debug.Log("Power Button Pushed");

        if (currentState != State.WaitForPlayerStartAction)
        {
            return;
        }

        currentPlaySurface.Mode = PushButtonSurface.SurfaceMode.PLAYING;
        currentState = State.GenerateWinPattern;
    }
}
