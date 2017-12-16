using UnityEngine;
using UnityEngine.EventSystems;

public class PushButton : MonoBehaviour, IPointerClickHandler
{
    public GameState gameState;

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("In PushButton.OnPointerClick");
        gameState.OnButtonPush(this);
    }
}
