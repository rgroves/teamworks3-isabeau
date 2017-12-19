using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExitButton : MonoBehaviour, IPointerClickHandler
{ 
    public GameState gameState;

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Exit Button Clicked");
        gameState.OnExitGame();
    }
}
