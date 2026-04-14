
using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioClip hoverSound; 
    public AudioClip clickSound; 
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound!= null)
        {
            audioSource.PlayOneShot(hoverSound);
        }   
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound!= null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
    

    
}
