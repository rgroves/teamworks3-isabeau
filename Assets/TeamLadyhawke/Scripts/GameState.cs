﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private int voiceOverEndGameSoundIdx = 0; //VoiceOverEndGameSound
    private int teleportSoundIdx = 0; //TeleportSound

    // Time after destruction of earth 
    private float timeAfterDestruction = 0f;
    // Time to wait after destruction of earth on loss before going to credits
    private float timeToWaitAfterDestruction = 2.5f;


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
        WaitForInputSound,
        CollectPlayerGuesses,
        WinLevel,
        WaitForWinLevelSound,
        WinGame,
        SimulateTeleport,
        WaitForEndGameVoiceover,
        LostRound,
        LostGame,
        WaitForDestruction,
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
            else if (clip.name == "VoiceOverEndGameSound")
            {
                voiceOverEndGameSoundIdx = i;
            }
            else if (clip.name == "TeleportSound")
            {
                teleportSoundIdx = i;
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
                //numberOfButtonPushesToWin = (2 * currentPlaySurface.PushButtonCount) + (levelManager.CurrentRound - 2);
                numberOfButtonPushesToWin = (currentPlaySurface.PushButtonCount) + levelManager.CurrentRound;

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
                    currentState = State.WaitForInputSound;
                }

                if (currentState != State.ShowingWinPattern)
                {
                    audioSource.clip = soundEffects[readyForInputSoundIdx];
                    audioSource.Play();
                }
                break;

            case State.WaitForInputSound:
                if (!audioSource.isPlaying)
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
                GameObject.FindGameObjectWithTag("DestructoParticles").SetActive(false);
                GameObject portal = GameObject.FindGameObjectWithTag("Portal");

                foreach (ParticleSystem ps in portal.GetComponentsInChildren<ParticleSystem>())
                {
                    ParticleSystem.EmissionModule e = ps.emission;
                    e.enabled = false;
                    e.enabled = true;
                }

                Vector3 cameraPosition = GameObject.FindGameObjectWithTag("MainCamera").transform.parent.transform.position;
                cameraPosition.y -=2.5f;
                portal.transform.SetPositionAndRotation(cameraPosition, Quaternion.identity);
                EnablePortale(true);
                audioSource.clip = soundEffects[teleportSoundIdx];
                audioSource.Play();
                currentState = State.SimulateTeleport;
                break;

            case State.SimulateTeleport:
                if (!audioSource.isPlaying)
                {
                    GameObject.FindGameObjectWithTag("DoomsdayDevice").SetActive(false);
                    EnablePortale(false);
                    audioSource.clip = soundEffects[voiceOverEndGameSoundIdx];
                    audioSource.Play();
                    currentState = State.WaitForEndGameVoiceover;
                }
                break;

            case State.WaitForEndGameVoiceover:
                if (!audioSource.isPlaying)
                {
                    currentState = State.EndGame;
                }
                break;

            case State.LostRound:
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = soundEffects[lostRoundSoundIdx];
                    audioSource.Play();
                    GameObject.FindGameObjectWithTag("MainCamera").transform.parent.GetComponent<Animator>().SetTrigger("FloatUp");
                    currentState = State.LostGame;
                }
                break;

            case State.LostGame:
                GameObject.FindGameObjectWithTag("DestructoParticles").SetActive(true);
                if (!audioSource.isPlaying)
                {
                    GameObject.FindGameObjectWithTag("DestructoRay").GetComponent<Animator>().SetTrigger("Fire");
                    currentState = State.WaitForDestruction;
                }
                break;

            case State.WaitForDestruction:
                if (GameObject.FindGameObjectWithTag("Earth").GetComponent<Animator>().GetBool("Done"))
                {
                    if (timeAfterDestruction > timeToWaitAfterDestruction)
                    {
                        GameObject.FindGameObjectWithTag("MainCamera").transform.parent.GetComponent<Animator>().SetTrigger("FloatDown");
                        currentState = State.EndGame;
                    }
                    timeAfterDestruction += Time.deltaTime;
                }
                break;

            case State.EndGame:
                SceneManager.LoadScene("GameOverCredits", LoadSceneMode.Single);
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
        //Debug.Log("Push Button Pushed");

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
        //Debug.Log("Power Button Pushed");

        if (currentState != State.WaitForPlayerStartAction)
        {
            return;
        }

        if (currentPlaySurface.gameObject.transform != buttonPushed.gameObject.transform.parent)
        {
            // this is a different wall's power button press, ignore it
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
}
