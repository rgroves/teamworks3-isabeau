using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public int CurrentLevel { get { return level; } }
    public int CurrentRound { get { return round; } }
    public string CurrentLevelRound { get { return "Level: " + level + ", Round:" + round; } }
    public bool HasMoreLevels { get { return hasMoreLevels; } }
    public bool HasMoreRounds { get { return hasMoreRounds; } }

    private int level;
    private int round;
    private bool hasMoreLevels;
    private bool hasMoreRounds;
    private int MaxLevels { get; set; }
    private int MaxRounds { get; set; }

    public void NewGame(int levels, int rounds)
    {
        Debug.Log(">>> New Game <<<");

        // Initialize maximum number of levels & rounds.
        MaxLevels = levels;
        MaxRounds = rounds;

        // Initialize the starting level and round.
        level = 0;
        round = 0;
    }

    public void NewLevel()
    {
        // Determine if there are more levels to play.
        hasMoreLevels = level < MaxLevels;

        if (hasMoreLevels)
        {
            // Increment the level number.
            level++;

            Debug.Log(">>> Level " + level + " <<<");

            // Reset rounds for this level.
            round = 0;

            // Initialize Round 1
            NewRound();
        }
        else
        {
            Debug.Log(">>> No More Levels <<<");
        }

    }

    public void NewRound()
    {
        // Determine if there are still rounds to be played.
        hasMoreRounds = round < MaxRounds;

        if (hasMoreRounds)
        {
            // Increment the round number.
            round++;

            Debug.Log(">>> Round " + round + " <<<");
        }
        else
        {
            Debug.Log(">>> No More Rounds <<<");
        }
    }
}
