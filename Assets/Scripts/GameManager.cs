using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public int totalFragmentsCollectedOverall = 0;
    public const int TOTAL_FRAGMENTS_ALL_LEVELS = 12;

    public bool trueEndingUnlocked = false;
    const string TRUE_ENDING_KEY = "TrueEndingUnlocked";

    public Toggle advancedChallengeToggle;

    public bool isAdvancedChallengeEnabled = false;

    [System.Serializable]
    public class FragmentStory
    {
        [TextArea(2, 4)]
        public string[] fragments;

        [TextArea(4, 8)]
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

        LoadOverallProgress();
        LoadProgress();
        LoadSpeedrunData();
        LoadTrueEndingStatus();
        LoadGallaryData();

    }

    void Start()
    {
        if (GameManager.Instance != null)
            {
                advancedChallengeToggle.isOn = GameManager.Instance.isAdvancedChallengeEnabled;
            }
        advancedChallengeToggle.onValueChanged.AddListener((val) =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetAdvancedChallenge(val);
        });
    }

    void LoadSpeedrunData()
    {
        isSpeedrunModeUnlocked = PlayerPrefs.GetInt(SPEEDRUN_UNLOCK_KEY, 0) == 1;
        for (int i = 0; i < bestTimes.Length; i++)
        {
            bestTimes[i] = PlayerPrefs.GetFloat(BEST_TIME_PREFIX + (i + 1), -1f);
        }
    }

    void SaveSpeedrunData()
    {
        PlayerPrefs.SetInt(SPEEDRUN_UNLOCK_KEY, isSpeedrunModeUnlocked ? 1 : 0);
        for (int i = 0; i < bestTimes.Length; i++)
        {
            if (bestTimes[i] > 0)
                PlayerPrefs.SetFloat(BEST_TIME_PREFIX + (i + 1), bestTimes[i]);
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

    public void AddGlobalFragment(int levelIndex)
    {
        totalFragmentsCollectedOverall++;
        SaveOverallProgress(); 
    }

    void SaveOverallProgress()
    {
        PlayerPrefs.SetInt("TotalFragments", totalFragmentsCollectedOverall);
        PlayerPrefs.Save();
    }

    void LoadOverallProgress()
    {
        totalFragmentsCollectedOverall = PlayerPrefs.GetInt("TotalFragments", 0);
    }

    public void UnlockTrueEnding()
    {
        if (!trueEndingUnlocked)
        {
            trueEndingUnlocked = true; 
            PlayerPrefs.SetInt(TRUE_ENDING_KEY, 1);
            PlayerPrefs.Save(); 
            Debug.Log("True ending unlocked!"); 
        }
    }

    void LoadTrueEndingStatus()
    {
        trueEndingUnlocked = PlayerPrefs.GetInt(TRUE_ENDING_KEY, 0) == 1;
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

    public void SetAdvancedChallenge(bool enabled)
    {
        isAdvancedChallengeEnabled = enabled;
        PlayerPrefs.SetInt("AdvancedChallenge", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetDifficultyWithoutLoading(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        Debug.Log("Difficulty set to: " + currentDifficulty + " (no scene load)");
        // 不加载任何场景，等待后续手动跳转
    }

    public enum GallaryItemType { Skin, ConceptArt, DevMessage, EasterEgg}
    [System.Serializable]
    public class GallaryItem
    {
        public string itemName;
        public GallaryItemType type;
        public int costFragments;
        public bool isUnlocked;
        public string previewImagePath; 
    }

    public List<GallaryItem> gallaryItems = new List<GallaryItem>();

    void LoadGallaryData()
    {
        for (int i = 0; i < gallaryItems.Count; i++)
        {
            bool unlocked = PlayerPrefs.GetInt("GalleryItem_" + i, 0) ==1; 
            gallaryItems[i].isUnlocked = unlocked;
        }
    }

    void SaveGalleryData()
    {
        for (int i = 0; i < gallaryItems.Count; i++)
        {
            PlayerPrefs.SetInt("GalleryItem_" + i, gallaryItems[i].isUnlocked ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    public void UnlockGalleryItem(int index)
    {
        if (index < 0 || index >= gallaryItems.Count) return;
        if (!gallaryItems[index].isUnlocked && totalFragmentsCollectedOverall >= gallaryItems[index].costFragments)
        {
            gallaryItems[index].isUnlocked = true;
            SaveGalleryData();
            Debug.Log($"Unlocked {gallaryItems[index].itemName}");
        }
    }

    public bool IsGalleryItemUnlocked(int index)
    {
        return gallaryItems[index].isUnlocked;
    }

}
