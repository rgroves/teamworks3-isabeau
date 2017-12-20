using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {
    public GameObject SkipButton;
    public GameObject PlayAgainButton;
    public GameObject ExitButton;

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

    public AudioClip[] soundEffects;
    private AudioSource audioSource;
    private int readyForInputSoundIdx = 0;
    private int wonRoundSoundIdx = 0;
    private int lostRoundSoundIdx = 0;
    private int lostGameSoundIdx = 0; 
    private int easyPeasySoundIdx = 0; //WinEasyPeasySound
    private int pieceOfCakeSoundIdx = 0; //WinPieceOfCake
    private int allDayLongSoundIdx = 0; //WinAllDayLongSound
    private int worriedForAMinuteSoundIdx = 0; //WinWorriedForAMinute
    private int voiceOverPt1SoundIdx = 0; //VoiceOverPart1Sound
    private int voiceOverPt2SoundIdx = 0; //VoiceOverPart2Sound

    // The current level's play surface.
    private PushButtonSurface currentPlaySurface;

    // Number of button pushes needed to win for this round.
    private int numberOfButtonPushesToWin;

    // Game States
    private enum State
    {
        Intro1,
        WaitIntro1,
        Intro2,
        WaitIntro2,
        NewGame,
        NewLevel,
        NewRound,
        NewRoundWait,
        WaitForPlayerStartAction,
        BeginRound,
        WaitForStartSound,
        GenerateWinPattern,
        ShowWinPattern,
        ShowingWinPattern,
        CollectPlayerGuesses,
        WinLevel,
        WaitForWinLevelSound,
        WinGame,
        LostRound,
        LostGame,
        ShowEndGameUI,
        EndGame,
        ExitGame
    }

    // The current game state.
    private State currentState;

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        for (int i = 0; i < soundEffects.Length; i++)
        {
            AudioClip clip = soundEffects[i];

            if (clip.name == "LoseRoundSound")
            {
                lostRoundSoundIdx = i;
            }
            else if (clip.name == "WinRoundSound")
            {
                wonRoundSoundIdx = i;
            }
            else if (clip.name == "ReadyForInputSound")
            {
                readyForInputSoundIdx = i;
            }
            else if (clip.name == "LostGameSound")
            {
                lostGameSoundIdx = i;
            }
            else if (clip.name == "WinEasyPeasySound")
            {
                easyPeasySoundIdx = i;
            }
            else if (clip.name == "WinPieceOfCake")
            {
                pieceOfCakeSoundIdx = i;
            }
            else if (clip.name == "WinAllDayLongSound")
            {
                allDayLongSoundIdx = i;
            }
            else if (clip.name == "WinWorriedForAMinute")
            {
                worriedForAMinuteSoundIdx = i;
            }
            else if (clip.name == "VoiceOverPart1Sound")
            {
                voiceOverPt1SoundIdx = i;
            }
            else if (clip.name == "VoiceOverPart2Sound")
            {
                voiceOverPt2SoundIdx = i;
            }
        }
    }
    // Use this for initialization
    private void Start () {
        currentState = State.NewGame;
    }

    // Update is called once per frame
    private void Update () {
        switch (currentState)
        {
            case State.NewGame:
                Debug.Log("in NewGame State!");
                levelManager.NewGame(NumberOfLevels, NumberOfRounds);
                currentState = State.Intro1;
                break;

            case State.Intro1:
                EnablePortale(true);
                GameStartUIEnabled(true);
                EndGameUIEnabled(false);
                audioSource.clip = soundEffects[voiceOverPt1SoundIdx];
                audioSource.Play();
                currentState = State.WaitIntro1;
                break;

            case State.WaitIntro1:
                if (!audioSource.isPlaying)
                {
                    currentState = State.Intro2;
                }
                break;

            case State.Intro2:
                EnablePortale(false);

                audioSource.clip = soundEffects[voiceOverPt2SoundIdx];
                audioSource.Play();
                currentState = State.WaitIntro2;
                break;

            case State.WaitIntro2:
                if (!audioSource.isPlaying)
                {
                    GameStartUIEnabled(false);
                    currentState = State.NewLevel;
                }
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
                    currentState = State.WinGame;
                }

                break;

            case State.NewRound:
                levelManager.NewRound();
                currentPlaySurface.ClearPlayerGuesses();

                // If there are more rounds in the current level then continue play otherwise transition to next level.
                currentState = levelManager.HasMoreRounds ? State.GenerateWinPattern : State.WinLevel;
                break;

            case State.WinLevel:
                switch (levelManager.CurrentLevel)
                {
                    case 1:
                        audioSource.clip = soundEffects[easyPeasySoundIdx];
                        break;
                    case 2:
                        audioSource.clip = soundEffects[pieceOfCakeSoundIdx];
                        break;
                    case 3:
                        audioSource.clip = soundEffects[allDayLongSoundIdx];
                        break;
                    case 4:
                        audioSource.clip = soundEffects[worriedForAMinuteSoundIdx];
                        break;

                }

                audioSource.Play();
                currentState = State.WaitForWinLevelSound;
                break;

            case State.WaitForWinLevelSound:
                if (!audioSource.isPlaying)
                {
                    currentState = State.NewLevel;
                }
                break;

            case State.WaitForPlayerStartAction:
                // Nothing to do here. Wating for player to click the power 
                // button which will trigger a call to OnPowerButtonPush() 
                // which will transition the state.
                break;

            case State.BeginRound:
                currentPlaySurface.PlayStartSound();
                currentState = State.WaitForStartSound;
                break;

            case State.WaitForStartSound:
                currentState = currentPlaySurface.IsStartSoundPlaying ? State.WaitForStartSound : State.GenerateWinPattern;
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

                if (currentState != State.ShowingWinPattern)
                {
                    audioSource.clip = soundEffects[readyForInputSoundIdx];
                    audioSource.Play();
                }
                break;

            case State.CollectPlayerGuesses:
                if (currentPlaySurface.HasBegunCheckingGuesses)
                {
                    if (currentPlaySurface.IsCorrectGuessCount && currentPlaySurface.IsLastPlayerGuessCorrect && currentPlaySurface.HasCheckDelayEnded)
                    {
                        currentPlaySurface.Mode = PushButtonSurface.SurfaceMode.PLAYED;
                        audioSource.clip = soundEffects[wonRoundSoundIdx];
                        audioSource.Play();
                        currentState = State.NewRoundWait;
                    }
                    else if (!currentPlaySurface.IsLastPlayerGuessCorrect)
                    {
                        audioSource.clip = soundEffects[lostGameSoundIdx];
                        audioSource.Play();
                        currentState = State.LostRound;
                    }
                }
                break;

            case State.NewRoundWait:
                if (!audioSource.isPlaying)
                {
                    currentState = State.NewRound;
                }
                break;

            case State.WinGame:
                currentState = State.ShowEndGameUI;
                break;

            case State.LostRound:
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = soundEffects[lostRoundSoundIdx];
                    audioSource.Play();
                    currentState = State.LostGame;
                }
                break;

            case State.LostGame:
                if (!audioSource.isPlaying)
                {
                    currentState = State.ShowEndGameUI;
                }

                GameObject.FindGameObjectWithTag("MainCamera").transform.parent.GetComponent<Animator>().SetBool("FloatUp", true);
                break;

            case State.ShowEndGameUI:
                EndGameUIEnabled(true);
                currentState = State.EndGame;
                break;

            case State.EndGame:
                break;

            case State.ExitGame:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;

            default:
                Debug.LogError("Unhandled game state: " + currentState.ToString());
                break;
        }
    }

    private void EndGameUIEnabled(bool enabled)
    {
        if (enabled)
        {
            PlayAgainButton.SetActive(true);
            ExitButton.SetActive(true);
        }
        else
        {
            PlayAgainButton.SetActive(false);
            ExitButton.SetActive(false);
        }
    }

    public void EnablePortale(bool enabled)
    {
        GameObject portal = GameObject.FindGameObjectWithTag("Portal");

        if (enabled)
        {
            portal.gameObject.layer = 0;

            foreach (Transform child in portal.gameObject.GetComponentInChildren<Transform>())
            {
                child.gameObject.layer = 0;
            }
        }
        else
        {
            portal.gameObject.layer = 8;

            foreach (Transform child in portal.gameObject.GetComponentInChildren<Transform>())
            {
                child.gameObject.layer = 8;
            }
        }
    }

    public void GameStartUIEnabled(bool enabled)
    {
        if (enabled)
        {
            SkipButton.SetActive(true);
        }
        else
        {
            SkipButton.SetActive(false);
        }
    }

public void OnButtonPush(PushButton buttonPushed)
    {
        Debug.Log("Push Button Pushed");

        if (currentState != State.CollectPlayerGuesses)
        {
            buttonPushed.PlayInvalidActionSound();
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
        currentState = State.BeginRound;
    }

    public void OnSkipIntro()
    {
        audioSource.Stop();
        GameStartUIEnabled(false);
        EnablePortale(false);
        currentState = State.NewLevel;
    }

    public void OnPlayAgain()
    {
        EndGameUIEnabled(false);
        currentState = State.NewGame;
    }

    public void OnExitGame()
    {
        EndGameUIEnabled(false);
        currentState = State.ExitGame;
    }
}
