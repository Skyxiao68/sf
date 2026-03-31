using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; 
using TMPro;


public class player : MonoBehaviour
{   
    [Header("Debug")]
    public Vector2 inputDir;
    public int collectedTokens = 0;


    [Header("Input")]
    public Player_Input inputControl; 

    [Header("Player Settings")]
    public float moveSpeed = 5f;
    public float rotateSpeed = 100f;
    private CharacterController cc;
    private Vector3 moveDir; 
    
    

    [Header("Tokens and Pass")]
    public int totalTokens = 5;
    
    public GameObject exit; 
    public Material matExitUnlocked; 

    [Header("UI")]
    public TMP_Text tokenText;
    public TMP_Text winText;

    [Header("Sound")]
    public AudioClip tokenClip;
    public AudioClip winClip;
    public AudioClip fragmentClip; 
    private AudioSource audioSource; 

    [Header ("Fragments")]
    public int totalFragments = 4;
    private int collectedFragments = 0;
    public TMP_Text fragmentCountText;
    public GameObject fragmentPanel;
    public TMP_Text fragmentDisplayText;
    public float displayDuration = 2f;
    private float displayTimer = 0f;

    private bool isGameWin;

    private void Awake()
    {
        inputControl = new Player_Input();
        cc = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();     
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {  
      winText.gameObject.SetActive(false);

      UpdateTokenUI();

      UpdateFragmentUI();

      if (fragmentPanel != null)
      {
          fragmentPanel.SetActive(false);
      }
       
      
    }

    // Update is called once per frame
    void Update()
    {
        if(isGameWin) return;

        Vector2 inputLook = inputControl.Player.Look.ReadValue<Vector2>(); 
        float mouseX = inputLook.x * rotateSpeed * Time.deltaTime;
        
        transform.Rotate(0, mouseX, 0); 
        

        inputDir = inputControl.Player.Move.ReadValue<Vector2>();
        float xMove = inputDir.x;
        float yMove = inputDir.y;
       
        moveDir =  transform.forward * yMove  + transform.right * xMove;
        moveDir.y = 0;
        moveDir.Normalize();
        cc.Move(moveDir * moveSpeed * Time.deltaTime);

        if (displayTimer > 0)
        {
            displayTimer -= Time.deltaTime;
            if (displayTimer <= 0 && fragmentPanel != null)
            {
                fragmentPanel.SetActive(false);
            }
        }

        if (collectedTokens == totalTokens)
            {
                UnlockExit();

            }

    }
    

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Token"))
        {
            collectedTokens++;
            Destroy(other.gameObject);
            PlaySound(tokenClip); 
            UpdateTokenUI();
        }   
        else if (other.CompareTag("Fragment"))
        {
            FragmentData data = other.GetComponent<FragmentData>();
            if (data != null)
            {
                collectedFragments++;
                UpdateFragmentUI();

                if (fragmentDisplayText != null && fragmentPanel != null)
                {
                    fragmentDisplayText.text = data.fragmentText;
                    fragmentPanel.SetActive(true);
                    displayTimer = displayDuration;
                }

                PlaySound(fragmentClip);
                Destroy(other.gameObject);
            }
        }
        else  if (other.CompareTag("Exit") && collectedTokens == totalTokens)
        {
            GameWin();
        }
        
       

    }

    void UpdateTokenUI()
    {
        tokenText.text = "Tokens: " + collectedTokens + "/" + totalTokens;
    }

    void UpdateFragmentUI()
    {
        if (fragmentCountText != null)
        {
            fragmentCountText.text = "Fragments: " + collectedFragments + "/" + totalFragments;
        }
    }

    void UnlockExit()
    {
        

       if (matExitUnlocked == null)
        {
            Debug.LogError("matExitUnlocked is null");
            return;
        }

        exit.GetComponent<Renderer>().material = matExitUnlocked;
        Debug.Log("Exit Unlocked");
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
