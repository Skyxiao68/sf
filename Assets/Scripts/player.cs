
using UnityEngine;
using UnityEngine.UI;

public class player : MonoBehaviour
{   
    [Header("Player Settings")]
    public float movespeed = 5f;
    public float rotatespeed = 100f;
    private CharacterController cc;
    private Vector3 moveDir; 

    [Header("Tokens and Pass")]
    public int totalTokens = 5;
    private int collectedTokens = 0;
    public GameObject exit; 
    private Material matExitUnlocked; 

    [Header("UI")]
    public Text tokenText;
    public Text winText;

    [Header("Sound")]
    public AudioClip tokenClip;
    public AudioClip winClip;
    private AudioSource audioSource; 

    private bool isGameWin; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      cc = GetComponent<CharacterController>(); 
      audioSource = GetComponent<AudioSource>();

      winText.gameObject.SetActive(false);

      UpdateTokenUI();

      matExitUnlocked = exit.GetComponent<Renderer>().materials[1];   
    }

    // Update is called once per frame
    void Update()
    {
        if(isGameWin) return;

        float mouseX = Input.GetAxis("Mouse X") * rotatespeed * Time.deltaTime;
        transform.Rotate(0, mouseX, 0); 

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        moveDir =  transform.forward * vertical + transform.right * horizontal;
        moveDir.y = 0;
        moveDir.Normalize();
        cc.Move(moveDir * movespeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }
    }

    void OTriggerEnter(Collider other)
    {
        if (other.CompareTag("Token"))
        {
            collectedTokens++;
            Destroy(other.gameObject);
            PlaySound(tokenClip); 
            UpdateTokenUI();

            if (collectedTokens == totalTokens);
            {
                UnlockExit();

            }
        }   

        if (other.CompareTag("Exit") && collectedTokens == totalTokens)
        {
            GameWin();
        }

    }

    void UpdateTokenUI()
    {
        tokenText.text = "Tokens: " + collectedTokens + "/" + totalTokens;
    }

    void UnlockExit()
    {
        exit.GetComponent<Renderer>().material = matExitUnlocked; 
    }

    void GameWin()
    {
        isGameWin = true;
        winText.gameObject.SetActive(true);
        winText.text = "You Win! Time: " + Mathf.Round(Time.time) + "s";
        PlaySound(winClip);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }
    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

}
