using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;



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

    public TMP_Text timerText;

    private float levelStartTime;



    [Header("Tokens and Pass")]
    public int totalTokens = 5;
    public GameObject tokenPrefab;
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

    [Header("Fragments")]
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
    public float acceleration = 8f;
    public float deceleration = 12f;
    public float currentSpeed = 0f;

    [Header("Smooth Camera")]
    public float rotateSmoothTime = 0.1f;
    private float currentRotateVel = 0f;

    [Header("Exit Hint")]
    public float hintDistance = 5f;
    public GameObject exitHintPanel;
    public TMP_Text exitHintText;

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
        Debug.Log("player Start() executed");

        winText.gameObject.SetActive(false);

        if (fragmentPanel != null)
        {
            fragmentPanel.SetActive(false);
        }

        if (GameManager.Instance != null)
        {
            Debug.Log("Current Difficulty: " + GameManager.Instance.currentDifficulty);
            switch (GameManager.Instance.currentDifficulty)
            {
                case GameManager.Difficulty.Easy:
                    moveSpeed = 3f;
                    totalTokens = 3;
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

        

        

        if (GameManager.Instance != null && GameManager.Instance.isSpeedrunModeActive && timerText != null)
        {
            timerText.gameObject.SetActive(true);
            levelStartTime = Time.time;
            timerText.text = "Time: 0.00";   // 初始化显示
        }

        if (GameManager.Instance != null && GameManager.Instance.isSpeedrunModeActive)
        {
            // 禁用场景中所有碎片物体
            GameObject[] fragments = GameObject.FindGameObjectsWithTag("Fragment");
            foreach (GameObject frag in fragments)
            {
                if (frag != null) frag.SetActive(false);
            }

            // 隐藏碎片计数文本和侧边栏面板
            if (fragmentCountText != null) fragmentCountText.gameObject.SetActive(false);
            if (fragmentPanel != null) fragmentPanel.SetActive(false);
        }

        AdjustCollectibles();
        
        UpdateSkin();
        
        RandomExit(); // 在开始时随机移动出口

        UpdateTokenUI();

        UpdateFragmentUI();

    }

    public void UpdateSkin()
    {
        Debug.Log("UpdateSkin() called");

        Material mat = GameManager.Instance?.GetCurrentSkinMaterial();
        Debug.Log($"GetCurrentSkinMaterial returned: {(mat != null ? mat.name : "Null")}");

        if (mat != null)
        {
            GetComponent<Renderer>().material = mat;
            Debug.Log("Player skin updated to " + mat.name);
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null in UpdateSkin() method");
            return;
        }



    }

    void AdjustCollectibles()
    {
        // 处理挑战模式增加需求（必须在动态生成前调整 totalTokens）
        if (GameManager.Instance != null && GameManager.Instance.isAdvancedChallengeEnabled)
        {
            totalTokens += 2;
        }

        // 动态生成随机硬币
        SpawnRandomTokens();

        // 调整碎片（保持不变）
        GameObject[] fragments = GameObject.FindGameObjectsWithTag("Fragment");
        for (int i = 0; i < fragments.Length; i++)
        {
            fragments[i].SetActive(i < totalFragments);
        }
    }

    void SpawnRandomTokens()
    {
        {
            // 查找所有硬币生成点
            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("TokenSpawnPoint");
            if (spawnPoints.Length == 0)
            {
                Debug.LogWarning("No TokenSpawnPoint found in scene. Falling back to static coins.");
                // 如果没有生成点，则回退到原有的静态硬币激活方式
                AdjustStaticCoins();
                return;
            }

            // 删除场景中已有的静态硬币（如果存在）
            GameObject[] existingCoins = GameObject.FindGameObjectsWithTag("Token");
            foreach (GameObject coin in existingCoins)
            {
                Destroy(coin);
            }

            // 随机打乱生成点顺序
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                int rand = Random.Range(i, spawnPoints.Length);
                GameObject temp = spawnPoints[i];
                spawnPoints[i] = spawnPoints[rand];
                spawnPoints[rand] = temp;
            }

            // 实例化前 totalTokens 个硬币
            for (int i = 0; i < totalTokens && i < spawnPoints.Length; i++)
            {
                Instantiate(tokenPrefab, spawnPoints[i].transform.position, Quaternion.identity);
            }
        }
    }

    void AdjustStaticCoins()
    {
        GameObject[] tokens = GameObject.FindGameObjectsWithTag("Token");
        for (int i = 0; i < tokens.Length; i++)
        {
            tokens[i].SetActive(i < totalTokens);
        }
    }

    void RandomExit()
    {
        if (GameManager.Instance != null && GameManager.Instance.isAdvancedChallengeEnabled)
        {
            GameObject[] exitPoints = GameObject.FindGameObjectsWithTag("ExitSpawnPoint");
            if (exitPoints.Length > 0)
            {
                Vector3 newPos = exitPoints[Random.Range(0, exitPoints.Length)].transform.position;
                exit.transform.position = newPos;
                Debug.Log($"Exit move to {newPos} in challenge mode.");

            }
        }
    }



    void Update()
    {
        if (isGameWin) return;

        Vector2 inputLook = inputControl.Player.Look.ReadValue<Vector2>();
        float targetRotate = inputLook.x * rotateSpeed * Time.deltaTime;
        float smoothRotate = Mathf.SmoothDamp(0, targetRotate, ref currentRotateVel, rotateSmoothTime);
        transform.Rotate(0, smoothRotate, 0);


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

        if (exit != null && collectedTokens < totalTokens)
        {
            float disToExit = Vector3.Distance(transform.position, exit.transform.position);
            if (disToExit < hintDistance)
            {
                exitHintPanel.SetActive(true);
                exitHintText.text = $"Need {totalTokens - collectedTokens} more tokens to unlock exit!";
            }
            else
            {
                exitHintPanel.SetActive(false);
            }
        }
        else
        {
            exitHintPanel.SetActive(false);
        }

        if (!isGameWin && timerText != null && timerText.gameObject.activeSelf)
        {
            float elapsed = Time.time - levelStartTime;
            timerText.text = "Time: " + elapsed.ToString("F2");
        }
    }


    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered with: {other.tag}");

        if (other.CompareTag("Token") || other.CompareTag("Fragment"))
        {
            CameraShake.instance?.Shake(0.1f, 0.5f);

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

                bool nowFull = (collectedFragments == totalFragments);
                GameManager.Instance.AddGlobalFragment(GetCurrentLevelIndex(), nowFull);

                Debug.Log($"Collected fragment. Current: {collectedFragments}/{totalFragments}");

                if (nowFull)
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
        if (GameManager.Instance != null && GameManager.Instance.isSpeedrunModeActive) return;

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
        float finishTime = Time.time - levelStartTime;

        winText.gameObject.SetActive(true);
        winText.text = "You Win! Time: " + finishTime.ToString("F2") + "s";
        PlaySound(winClip);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        int currentLevel = GetCurrentLevelIndex();

        

        // 获取全局全收集状态
        bool anyLevelFull = false;
        bool allLevelsFull = false;
        if (GameManager.Instance != null)
        {
            anyLevelFull = System.Linq.Enumerable.Any(GameManager.Instance.levelFullyCollected, x => x);
            allLevelsFull = System.Linq.Enumerable.All(GameManager.Instance.levelFullyCollected, x => x);
        }

        // 显示结局或故事
        if (currentLevel == 3)
        {
            string endingText = "";
            if (GameManager.Instance != null)
            {
                if (allLevelsFull && GameManager.Instance.totalFragmentsCollectedOverall == GameManager.TOTAL_FRAGMENTS_ALL_LEVELS)
                {
                    endingText = "You have pieced together the complete story of the maze. The maze is no longer a cage, but a memory cherished.";
                    GameManager.Instance.UnlockTrueEnding();
                }
                else if (anyLevelFull)
                {
                    endingText = "You have gathered the full memory of one part of the maze. A fragment of the past emerges clearly.";
                }
                else
                {
                    endingText = "You found the exit. The memory of the maze slowly fades.";
                }
            }
            else
            {
                endingText = "You found the exit.";
            }
            storyText.text = endingText;
            storyPanel.SetActive(true);
        }
        else
        {
            // 非第三关：仅当本关卡全收集时显示故事
            if (allFragmentCollected)
            {
                string story = GetLevelStory(currentLevel);
                if (!string.IsNullOrEmpty(story))
                {
                    storyText.text = story;
                    storyPanel.SetActive(true);
                }
            }
        }

        // 通关进度与时间记录
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteLevel(currentLevel);

            if (GameManager.Instance.isSpeedrunModeActive)
            {
                GameManager.Instance.RegisterBestTime(currentLevel, finishTime);
                GameManager.Instance.isSpeedrunModeActive = false;
            }

            if (GameManager.Instance.isAdvancedChallengeEnabled)
            {
                GameManager.Instance.RegisterChallengeBestTime(currentLevel, finishTime);
            }
        }

        if (allFragmentCollected)
        {
            GameManager.Instance?.UnlockLevelReward(currentLevel, true);
        }

        StartCoroutine(ReturnToLevelSelect());
    }

    string GetLevelStory(int level)
    {
        switch (level)
        {
            case 1:
                return "You stand before the first exit, looking back at the winding path behind you. Memories flood back: this was the garden where you played as a child. Those walls are not walls – they are trimmed hedges. Those coins are your lost marbles. You begin to sense that this maze hides a past you had forgotten.";
            case 2:
                return "The wind at the crossroads clears your hesitation. You remember that summer when you faced two choices: leave home to chase your dreams, or stay to protect your family. You chose the former, and never looked back. Every left or right turn in this maze echoes that decision. Now you understand – there is no right or wrong, only the journey.";
            case 3:
                return "In the heart of the hall, you place all the coins and fragments you collected. They melt into an image: you standing at the maze's entrance, but this time with a smile on your face. You realize that the entire maze was a training ground you built for yourself – to prepare you for the real world outside. The exit light is not an escape, but an embrace. You push the door open and step into a new adventure.";
            default:
                return "";
        }
    }

    IEnumerator ReturnToLevelSelect()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("LevelSelection");
    }

    int GetCurrentLevelIndex()
    {
        string name = SceneManager.GetActiveScene().name;
        if (name == "Level1") return 1;
        if (name == "Level2") return 2;
        if (name == "Level3") return 3;
        return 1;
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

    void OnDestroy()
    {
        inputControl?.Dispose();
    }
}
