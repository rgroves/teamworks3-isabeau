/********************************************************************** 
 * Udacity TeamWorks Team: Team Ladyhawke
 * Project: Doomsday Device
 **********************************************************************
 * File Name: PushButtonSurface.cs
 * Author(s): Robert Groves (rwgdev@gmail.com)
 * Description: 
**********************************************************************/ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class PushButtonSurface : MonoBehaviour {
    // Tag string that should be used to tag the game object that represents 
    // the power button of a PushButonSurface.
    private const string powerButtonTag = "PowerButton";

    // Tag string that should be used to tag the game object(s) that 
    // represents the push button(s) of a PushButonSurface.
    private const string pushButtonTag = "PushButton";

    // The game object to be used as the power button for this 
    // PushButtonSurface.
    public PowerButton powerButton;

    // The game objects to be used as the push button(s) for this PushButton 
    // Surface.
    public List<PushButton> pushButtons = new List<PushButton>();

    // The game state. A refernce to the game state is needed for the 
    // convenience of having the game state passed to the push buttons on this 
    // PushButtonSurface so they don't have to be explicitly wired up in the 
    // Unity Editor. PushButtonSurface does not explicitly modify the game state.
    public GameState gameState;

    // Defines the mode that a PushButtonSurface can be in at any given time.
    public enum SurfaceMode
    {
        UNINITIALIZED,
        NOT_YET_PLAYED,
        READY_TO_PLAY,
        PLAYING,
        PLAYED
    }

    // The number of push buttons on this PushButtonSurface.
    public int PushButtonCount
    {
        get { return pushButtons.Count; }
    }

    // Indicates if the win pattern is currently being shown on this PushButtonSurface
    private bool isShowingWinPattern = false;

    public bool IsShowingWinPattern
    {
        get { return isShowingWinPattern; }
    }


    // Indicates the current mode the surface is in.
    private SurfaceMode mode = SurfaceMode.UNINITIALIZED;

    // Use this for initialization
    private void Start()
    {
        if (gameState != null)
        {
            // If the power button game object wasn't wired up in the Unity 
            // Editor try to find it by tag.
            if (powerButton == null)
            {
                FindPowerButton();
            }

            // If the push button game objects weren't wired up in the Unity 
            // Editor try to find them by tag.
            if (pushButtons.Count == 0)
            {
                FindPushButtons();
            }

            // Check if GameState references were wired up in Unity Editor for 
            // both the power button and the push buttons. If not then pass the
            // gameState reference along automatically.
            if (powerButton != null && powerButton.gameState == null)
            {
                powerButton.gameState = gameState;
                Debug.Log("Power button " + powerButton.gameObject.name + " gameState was assigned by parent Surface " + gameObject.name);
            }

            foreach (PushButton currentButton in pushButtons)
            {
                if (currentButton.gameState == null)
                {
                    currentButton.gameState = gameState;
                    Debug.Log("Push button " + currentButton.name + " gameState was assigned by parent Surface " + gameObject.name);
                }
            }

            // Set the initial mode for this PushButtonSurface.
            Mode = SurfaceMode.NOT_YET_PLAYED;

            // Create a queue to store player guesses.
            playerGuessQueue = new Queue<PushButton>();
        }
        else
        {
            Debug.LogError("GameState reference must be set for " + gameObject.name);
        }
    }

    private bool isCheckingPlayerGuesses;
    private bool isLastPlayerGuessCorrect;
    private bool isCorrectGuessCount;
    private bool hasCheckDelayEnded;
    public bool IsLastPlayerGuessCorrect { get { return isLastPlayerGuessCorrect; } }
    public bool IsCorrectGuessCount { get { return isCorrectGuessCount; } }
    public bool HasCheckDelayEnded {  get { return hasCheckDelayEnded; } }
    private int showPatternIdx;

    private float timeSinceLastActivation;
    private float pauseTimeBetweenButtonActivations = .5f;

    private void Update()
    {
        if (isShowingWinPattern)
        {
            int btnIdx = winPattern[showPatternIdx];
            Animator anim = pushButtons[btnIdx].GetComponent<Animator>();

            bool activated = anim.GetBool("HasBeenActivated");

            if (!activated)
            {
                anim.SetBool("Activate", true);
                timeSinceLastActivation = 0;
            }
            else if (timeSinceLastActivation > pauseTimeBetweenButtonActivations)
            {
                // Button was lit up reset to light next one (if more to light)
                anim.SetBool("HasBeenActivated", false);

                isShowingWinPattern = (++showPatternIdx) < winPattern.Count;
            }

            timeSinceLastActivation += Time.deltaTime;
        }

        if (isCheckingPlayerGuesses)
        {
            //Debug.Log("Checking Player Guesses");
            if (playerGuessQueue.Count > 0)
            {
                PushButton btn = playerGuessQueue.Peek();
                Animator anim = btn.gameObject.GetComponent<Animator>();

                bool activated = anim.GetBool("HasBeenActivated");

                if (!activated)
                {
                    anim.SetBool("Activate", true);
                }
                else
                {
                    // Button was lit up reset to light next one (if more to light)
                    anim.SetBool("HasBeenActivated", false);

                    // Remove the guess from the queue
                    btn = playerGuessQueue.Dequeue();

                    // Check the guess for correctness.
                    int btnIdx = winPattern[playerGuessIdx++];

                    if (pushButtons[btnIdx] == btn)
                    {
                        isLastPlayerGuessCorrect = true;
                        //Debug.Log("Correct guess, more? " + ((playerGuessIdx < winPattern.Count) ? "Yes." : "No."));
                    }
                    else
                    {
                        Debug.Log("Wrong guess");
                        isLastPlayerGuessCorrect = false;
                        isCheckingPlayerGuesses = false;
                    }

                    // Check the guess count against the number of buttons in the win pattern.
                    if (playerGuessIdx == winPattern.Count)
                    {
                        isCorrectGuessCount = true;
                    } else if (playerGuessIdx > winPattern.Count)
                    {
                        Debug.Log("Too many guesses");
                        isLastPlayerGuessCorrect = false;
                        isCheckingPlayerGuesses = false;
                    }

                    hasBegunCheckingGuesses = true;
                }
            }

            if (hasBegunCheckingGuesses)
            {
                hasCheckDelayEnded = Time.time - timeOfLastGuess > 1;
            }
        }
    }

    // Used to set the mode for this PushButtonSurface.
    public SurfaceMode Mode {
        get
        {
            return mode;
        }

        set
        {
            switch (value)
            {
                case SurfaceMode.NOT_YET_PLAYED:
                    powerButton.TurnOff();
                    mode = value;
                    break;

                case SurfaceMode.READY_TO_PLAY:
                    powerButton.StandBy();
                    mode = value;
                    break;

                case SurfaceMode.PLAYING:
                    powerButton.TurnOff();
                    mode = value;
                    break;

                case SurfaceMode.PLAYED:
                    mode = value;
                    break;

                default:
                    // If an unexpected mode is encountered this error message will shed light on it.
                    Debug.LogError("Unexpected mode (" + value + ") for surface " + gameObject.name);
                    break;
            }
        }
    }

    private void FindPowerButton()
    {
        // Find the power button on this surface.
        foreach (Component child in transform)
        {
            if (child.tag == powerButtonTag)
            {
                if (powerButton == null)
                {
                    powerButton = child.gameObject.GetComponent<PowerButton>();
                    Debug.Log("Surface " + gameObject.name + ": power button was found automatically.");
                }
                else
                {
                    Debug.LogWarning("More than one power button detected on surface " + gameObject.name + "; only the first will be used.");
                }
            }
        }

        if (powerButton == null)
        {
            Debug.LogError("Surface " + gameObject.name + " was expected to have a GameObject tagged as " + powerButtonTag);
        }
    }

    private void FindPushButtons()
    {
        // Find all push buttons on this surface.
        foreach (Transform child in transform)
        {
            // Store all buttons found in pushButtons list.
            if (child.tag == pushButtonTag)
            {
                pushButtons.Add(child.gameObject.GetComponent<PushButton>());
            }
        }

        if (pushButtons.Count > 0)
        {
            Debug.Log("Surface " + gameObject.name + ": " + PushButtonCount + " push buttons found: ");
        }
        else
        {
            Debug.LogError("Surface " + gameObject.name + ": No push buttons found and none were wired up in Unity Editor. This surface is unplayable.");
        }
    }

    //
    //
    //
    
    private List<int> winPattern = new List<int>();
    private int playerGuessIdx = 0;
    private Queue<PushButton> playerGuessQueue;
    private float timeOfLastGuess;

    private bool hasBegunCheckingGuesses;
    public bool HasBegunCheckingGuesses { get { return hasBegunCheckingGuesses; } }


    public void GenerateWinPattern(int numberOfPushesToWin)
    {
        Debug.Log("Generating pattern of " + numberOfPushesToWin + " pushes to win.");

        // Clear any previous pattern stored.
        winPattern.Clear();

        System.Random rand = new System.Random();

        // Generate the pattern of button pushes that will cause a win condition.
        for (int i = 0; i < numberOfPushesToWin; i++)
        {
            int randomIdx = rand.Next(0, pushButtons.Count);
            winPattern.Add(randomIdx);
        }

        playerGuessIdx = 0;
    }

    public void LogWinPattern()
    {
        StringBuilder dbg = new StringBuilder("This is the win pattern: ");
        for (int i = 0; i < winPattern.Count; i++)
        {
            int btnIdx = winPattern[i];
            dbg.Append(btnIdx);
            dbg.Append(") ");
            dbg.Append(pushButtons[btnIdx].name.Replace("_PushButton", ""));
            dbg.Append((i < winPattern.Count - 1) ? ", " : "");
        }
        Debug.Log(dbg);
    }

    public void ShowWinPattern()
    {
        isShowingWinPattern = true;
        showPatternIdx = 0;
    }

    public void StorePlayerGuess(PushButton button)
    {
        playerGuessQueue.Enqueue(button);
        timeOfLastGuess = Time.time;
        //Debug.Log("Guess queued: " + button.name);
        isCheckingPlayerGuesses = true;
    }

    public void ClearPlayerGuesses()
    {
        //Debug.Log("Guesses cleared!");
        playerGuessQueue.Clear();
        timeOfLastGuess = 0;
        hasBegunCheckingGuesses = false;
        isCheckingPlayerGuesses = false;
        isLastPlayerGuessCorrect = false;
        isCorrectGuessCount = false;
        hasCheckDelayEnded = false;
    }

    public void PlayStartSound()
    {
        gameObject.GetComponent<AudioSource>().Play();
    }

    public bool IsStartSoundPlaying { get { return gameObject.GetComponent<AudioSource>().isPlaying; } }
}
