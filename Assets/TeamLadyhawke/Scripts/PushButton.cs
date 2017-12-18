using UnityEngine;
using UnityEngine.EventSystems;

public class PushButton : MonoBehaviour, IPointerClickHandler
{
    public GameState gameState;
    public AudioClip[] soundEffects;
    private int invalidSoundIdx = 0;
    private int buttonSoundIdx = 0;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        for(int i = 0; i < soundEffects.Length; i++)
        {
            AudioClip clip = soundEffects[i];

            if (clip.name == "InvalidActionSound")
            {
                invalidSoundIdx = i;
            }
            else
            {
                buttonSoundIdx = i;
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("In PushButton.OnPointerClick");
        gameState.OnButtonPush(this);
    }

    public void PlayInvalidActionSound()
    {
        audioSource.clip = soundEffects[invalidSoundIdx];

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.Play();
    }

    public void PlayButtonSound()
    {
        audioSource.clip = soundEffects[buttonSoundIdx];
        
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
