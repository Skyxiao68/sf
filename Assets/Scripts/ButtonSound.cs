
using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip hoverSound; 
    public AudioClip clickSound; 

    private AudioSource audioSource;
    private Button button; 
    
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false; 

        button = GetComponent<Button>();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Hover detected on" + gameObject.name);
        if (button != null && button.interactable && hoverSound!= null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click detected on" + gameObject.name);
        if (button != null && button.interactable && clickSound!= null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
    

    
}
