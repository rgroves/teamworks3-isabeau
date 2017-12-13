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

    // Number of button pushes needed to win for this round.
    private int numberOfButtonPushesToWin;

     // Game States
    private enum State
    {
        NewGame,
        NewLevel,
        NewRound,
        GenerateWinPattern,
        ShowWinPattern,
        CollectPlayerInput,
        CheckPlayerGuess,
        EndGame
    }

    // The current game state.
    private State currentState;

    // TODO
    int curWinPatternIdx = 0;

    Queue<GameObject> playerGuesses;
    int guessResult;
    float timeOfLastGuess;

    // Use this for initialization
    void Start () {
        playerGuesses = new Queue<GameObject>();
        currentState = State.NewGame;
    }

    // Update is called once per frame
    void Update () {
        switch (currentState)
        {
            case State.NewGame:
                levelManager.NewGame(NumberOfLevels, NumberOfRounds);
                playerGuesses.Clear();
                curWinPatternIdx = 0;
                timeOfLastGuess = 0;
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
                numberOfButtonPushesToWin = (2 * playSurface.PushButtonCount) + (round - 2);

                Debug.Log(">>> " + levelManager.CurrentLevelRound + ", Pushes To Win: " + numberOfButtonPushesToWin + " <<<");

                // Generate the pattern that corresponds to the correct order of 
                // button pushes the player must make to win.
                playSurface.GenerateWinPattern(numberOfButtonPushesToWin);

                curWinPatternIdx = 0;
                currentState = State.ShowWinPattern;
                playSurface.ShowWinPattern();
                break;

            case State.ShowWinPattern:

                curWinPatternIdx = playSurface.ShowWinPattern(curWinPatternIdx) ? 
                    (curWinPatternIdx + 1) : curWinPatternIdx;

                currentState = curWinPatternIdx < numberOfButtonPushesToWin ? 
                        State.ShowWinPattern : State.CollectPlayerInput;
                break;

            case State.CollectPlayerInput:
                if (guessResult == 3) 
                {
                    if (playerGuesses.Count > 0)
                    {
                        currentState = State.CheckPlayerGuess;
                    }
                    else if (Time.time - timeOfLastGuess > 1)
                    {
                        currentState = State.NewRound;
                        guessResult = 0;
                    }
                }
                break;

            case State.CheckPlayerGuess:
                guessResult = playSurface.CheckPlayerGuess(playerGuesses.Dequeue());

                switch (guessResult)
                {
                    case 1:
                        currentState = State.CollectPlayerInput;
                        break;
                    case 3:
                        currentState = State.CollectPlayerInput;
                        break;
                    default:
                        currentState = State.EndGame;
                        break;
                }
                break;

            case State.EndGame:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                break;
        }
    }

    public void OnButtonPush(GameObject buttonPushed)
    {
        Debug.Log("here");

        if (currentState != State.CollectPlayerInput)
        {
            return;
        }

        timeOfLastGuess = Time.time;

        playerGuesses.Enqueue(buttonPushed);

        currentState = State.CheckPlayerGuess;
    }
}
