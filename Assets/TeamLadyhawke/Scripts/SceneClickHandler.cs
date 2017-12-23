using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneClickHandler : MonoBehaviour
{
    public void SceneClick(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));

    }
    public void SceneClick()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        switch (currentSceneName)
        {
            case "TitleScreen":
                StartCoroutine(LoadSceneAsync("Main"));
                break;

            default:
                Debug.LogError("Unhandled scene name:" + currentSceneName);
                break;
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // The Application loads the Scene in the background at the same time as the current Scene.
        //This is particularly good for creating loading screens. You could also load the Scene by build //number.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        //Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

}
