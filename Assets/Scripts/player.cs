using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; 
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;


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

    [Header("Particle Effects")]
    public GameObject pickupEffectPrefab;

    [Header("Story")]
    public GameObject storyPanel;
    public TMP_Text storyText;
    private bool allFragmentCollected = false; 

    [Header("Smooth Movement")]
    public float acceleration = 10f;
    public float deceleration = 10f;
    public float currentSpeed = 0f; 

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
    
    void Start()
    {  
      winText.gameObject.SetActive(false);
      
      if (fragmentPanel != null)
      {
          fragmentPanel.SetActive(false);
      }
       
      if (GameManager.Instance != null)
        {   
            Debug.Log("Current Difficulty: "+ GameManager.Instance.currentDifficulty);
            switch (GameManager.Instance.currentDifficulty)
            {
                case GameManager.Difficulty.Easy:
                    moveSpeed = 3f;
                    totalTokens =3;
                    totalFragments = 4;
                    break;
                case GameManager.Difficulty.Normal:
                    moveSpeed = 2f;
                    totalTokens = 5;
                    totalFragments = 4;
                    break;
            }
        }
        else
        {
            Debug.LogWarning("GameManager.Instance is null Using default values");
        }
    
        if (storyPanel != null)
        {
            storyPanel.SetActive(false);
        }

      Debug.Log($"Total Tokens set to: {totalTokens}, Total Fragments set to: {totalFragments}  ");  

      AdjustCollectibles();    

      UpdateTokenUI();

      UpdateFragmentUI();
      
    }

    void AdjustCollectibles()
    {
        GameObject[] tokens = GameObject.FindGameObjectsWithTag("Token");
        Debug.Log($"Found {tokens.Length} tokens in scene.");

        for (int i = 0; i < tokens.Length; i++)
        {
            bool active = i < totalTokens;
            tokens[i].SetActive(active);
            Debug.Log($"Token {i}: set active = {active}, name = {tokens[i].name}");
        }

        GameObject[] fragments = GameObject.FindGameObjectsWithTag("Fragment");
        Debug.Log($"Found {fragments.Length} fragments in scene.");

        for (int i = 0; i < fragments.Length; i++)
        {
            bool active = i < totalFragments;
            fragments[i].SetActive(active);
            Debug.Log($"Fragment {i}: set active = {active}, name = {fragments[i].name}");
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
        Vector3 targetDir = transform.forward * yMove + transform.right * xMove;
        targetDir.Normalize();
        if (targetDir.magnitude > 0.1f)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, deceleration * Time.deltaTime);
        }

        cc.Move(targetDir * currentSpeed * Time.deltaTime);

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
        Debug.Log($"Trigger entered with: {other.tag}");

        if (other.CompareTag("Token") || other.CompareTag("Fragment"))
        {
            if (pickupEffectPrefab != null)
            {
                GameObject effect = Instantiate(pickupEffectPrefab, other.transform.position, Quaternion.identity);
                Destroy(effect, 1f);
            }
        }

        if (other.CompareTag("Token"))
        {
            collectedTokens++;
            Destroy(other.gameObject);
            PlaySound(tokenClip);
            UpdateTokenUI();
            Debug.Log($"Collected token. Current: {collectedTokens}/{totalTokens}");
        }
        else if (other.CompareTag("Fragment"))
        {
            FragmentData data = other.GetComponent<FragmentData>();
            if (data != null)
            {
                collectedFragments++;
                UpdateFragmentUI();

                fragmentDisplayText.text = data.fragmentText;
                fragmentPanel.SetActive(true);
                displayTimer = displayDuration;
                PlaySound(fragmentClip);
                Destroy(other.gameObject);

                Debug.Log($"Collected fragment. Current: {collectedFragments}/{totalFragments}");

                if (collectedFragments == totalFragments)
                {
                    allFragmentCollected = true;
                 Debug.Log("All fragments collected!");
                }
            }
        }
        else if (other.CompareTag("Exit") && collectedTokens == totalTokens)
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

    // 集齐所有碎片时显示故事面板
        if (allFragmentCollected && GameManager.Instance != null && GameManager.Instance.story != null && GameManager.Instance.story.fullStory != null && GameManager.Instance.story.fullStory.Length > 0)
        {
        // 设置面板内的文本
            storyText.text = GameManager.Instance.story.fullStory[0];
        // 显示整个面板
            storyPanel.SetActive(true);
        }
    }
    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void CloseStoryPanel()
    {
        if (storyPanel != null)
        {
            storyPanel.SetActive(false);
        }
    }

}
