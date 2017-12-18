using UnityEngine;
using UnityEngine.EventSystems;

public class PowerButton : MonoBehaviour, IPointerClickHandler
{
    // TOOD should remove gamestate and set up an Observer pattern for the click event
    public GameState gameState;
    private Animator animator;
    private AudioSource audioSource;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator could not be found for " + gameObject.name);
        }

        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource could not be found for " + gameObject.name);
        }
    }

    public void StandBy()
    {
        // When the power button is in standby it's "glow" should be enabled.
        animator.SetBool("Activate", true);
        audioSource.Play();
    }

    public void TurnOn()
    {
        // When the power button is turned on it's "glow" and sound should be disabled.
        animator.SetBool("Activate", false);
        audioSource.Stop();
    }

    public void TurnOff()
    {
        // When the power button is turned off it's "glow" and sound should be disabled.
        animator.SetBool("Activate", false);
        audioSource.Stop();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        gameState.OnPowerButtonPush(gameObject);
    }
}