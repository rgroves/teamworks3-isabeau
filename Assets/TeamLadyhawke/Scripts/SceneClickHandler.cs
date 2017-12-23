using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneClickHandler : MonoBehaviour
{
    public void SceneClick()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        switch (currentSceneName)
        {
            case "TitleScreen":
                SceneManager.LoadScene("Main", LoadSceneMode.Single);
                break;

            case "GameOverCredits":
                SceneManager.LoadScene("TitleScreen", LoadSceneMode.Single);
                break;

            default:
                Debug.LogError("Unhandled scene name:" + currentSceneName);
                break;
        }
    }

}
