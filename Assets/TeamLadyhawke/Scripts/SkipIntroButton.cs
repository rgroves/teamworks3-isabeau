using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SkipIntroButton : MonoBehaviour, IPointerClickHandler
{
    public GameState gameState;

    public void OnPointerClick(PointerEventData eventData)
    {
        gameState.OnSkipIntro();
    }
}
