using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayAgainButton : MonoBehaviour, IPointerClickHandler
{
    public GameState gameState;

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Play Again Button Clicked");
        gameState.OnPlayAgain();
    }
}