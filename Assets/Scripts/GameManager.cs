using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Speedrun Mode")]
    public bool isSpeedrunModeUnlocked = false;
    public bool isSpeedrunModeActive = false;
    public float[] bestTimes = new float[3];

    private const string SPEEDRUN_UNLOCK_KEY = "SpeedrunUnlocked";
    private const string BEST_TIME_PREFIX = "BestTime_";

    public static GameManager Instance { get; private set; }
    public int highestUnlockedLevel = 1;
    private const string SAVE_KEY = "HighestUnlockedLevel";
    public enum Difficulty { Easy, Normal }
    public Difficulty currentDifficulty = Difficulty.Normal;

    public int maxLevel = 3;

    [System.Serializable]
    public class FragmentStory
    {
        [TextArea(2,4)]
        public string[] fragments;

        [TextArea(4,8)]
        public string[] fullStory;
    }
    public FragmentStory story;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadProgress();
        LoadSpeedrunData(); 

    }
    
    void LoadSpeedrunData()
    {
        isSpeedrunModeUnlocked = PlayerPrefs.GetInt(SPEEDRUN_UNLOCK_KEY, 0) == 1;
        for (int i = 0; i < bestTimes.Length; i++)
        {
            bestTimes[i] = PlayerPrefs.GetFloat(BEST_TIME_PREFIX + (i+1), -1f);
        }
    }

    void SaveSpeedrunData()
    {
        PlayerPrefs.SetInt(SPEEDRUN_UNLOCK_KEY, isSpeedrunModeUnlocked ? 1 : 0);
        for (int i = 0; i < bestTimes.Length; i++)
        {
            if (bestTimes[i] > 0)
                PlayerPrefs.SetFloat(BEST_TIME_PREFIX + (i+1), bestTimes[i]);
        }
        PlayerPrefs.Save();
    }

    public void UnlockSpeedrunMode()
    {
        if (!isSpeedrunModeUnlocked)
        {
          isSpeedrunModeUnlocked = true;
         SaveSpeedrunData();
         Debug.Log("Speedrun mode unlocked!");
        }
    }

    public void RegisterBestTime(int levelIndex, float timeInSeconds)
{
    if (levelIndex < 1 || levelIndex > 3) return;
    int idx = levelIndex - 1;
    if (bestTimes[idx] < 0 || timeInSeconds < bestTimes[idx])
    {
        bestTimes[idx] = timeInSeconds;
        SaveSpeedrunData();
        Debug.Log($"New best time for Level{levelIndex}: {timeInSeconds:F2}s");
    }
}


    void LoadProgress()
    {
        highestUnlockedLevel = PlayerPrefs.GetInt(SAVE_KEY, 1);
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt(SAVE_KEY, highestUnlockedLevel);
        PlayerPrefs.Save();
    }

    public void CompleteLevel(int levelIndex)
    {   
        if (levelIndex == highestUnlockedLevel)
        {
            if (levelIndex < maxLevel)
                {
                    highestUnlockedLevel = levelIndex + 1;
                }
            else if (levelIndex == maxLevel)
                {
                    UnlockSpeedrunMode();
                }
            SaveProgress();
    }
    }

    public void LoadLevel(int levelIndex)
    {
        string sceneName = "Level" + levelIndex;
        SceneManager.LoadScene(sceneName);
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        Debug.Log("Difficulty set to : " + currentDifficulty);
        SceneManager.LoadScene("Level1"); 
    }

    public void SetDifficultyWithoutLoading(Difficulty difficulty)
{
    currentDifficulty = difficulty;
    Debug.Log("Difficulty set to: " + currentDifficulty + " (no scene load)");
    // 不加载任何场景，等待后续手动跳转
}
    
}
