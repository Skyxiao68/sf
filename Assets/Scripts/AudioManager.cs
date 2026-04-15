using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;
    private AudioSource audioSource;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClick()
    {
        if (buttonClickSound != null)
            audioSource.PlayOneShot(buttonClickSound);
    }

    public void PlayHover()
    {
        if (buttonHoverSound != null)
            audioSource.PlayOneShot(buttonHoverSound);
    }
}
