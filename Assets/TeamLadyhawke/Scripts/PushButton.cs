using UnityEngine;
using UnityEngine.EventSystems;

public class PushButton : MonoBehaviour, IPointerClickHandler
{
    public GameState gameState;

    public void OnPointerClick(PointerEventData eventData)
    {
        gameState.OnButtonPush(gameObject);
    }
}
