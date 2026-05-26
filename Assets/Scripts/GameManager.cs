using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ==================== 基础设置 ====================
    public enum Difficulty { Easy, Normal }
    public Difficulty currentDifficulty = Difficulty.Normal;
    public int highestUnlockedLevel = 1;
    public int maxLevel = 3;

    // ==================== 故事与全局进度 ====================
    [Header("Story")]
    public int totalFragmentsCollectedOverall = 0;
    public bool[] levelFullyCollected = new bool[3];      // 每个关卡是否全收集
    public const int TOTAL_FRAGMENTS_ALL_LEVELS = 12;
    public bool trueEndingUnlocked = false;

    [System.Serializable]
    public class FragmentStory
    {
        [TextArea(2, 4)] public string[] fragments;
        [TextArea(4, 8)] public string[] fullStory;
    }
    public FragmentStory story;

    // ==================== 极速模式 ====================
    [Header("Speedrun Mode")]
    public bool isSpeedrunModeUnlocked = false;
    public bool isSpeedrunModeActive = false;
    public float[] bestTimes = new float[3];   // 普通模式最佳时间

    // ==================== 挑战模式 ====================
    [Header("Challenge Mode")]
    public bool isAdvancedChallengeEnabled = false;
    public float[] challengeBestTimes = new float[3];   // 挑战模式最佳时间

    // ==================== 皮肤系统 ====================
    [Header("Skins")]
    public Material[] playerSkins;   // 顺序：0默认,1绿色,2红色,3金色
    public int currentSkinIndex = 0;

    // ==================== 画廊系统 ====================
    
    public enum GalleryItemType { Skin, ConceptArt, DevMessage, EasterEgg }
    [System.Serializable]
    public class GalleryItem
    {
        public string itemName;
        public GalleryItemType type;
        public int costFragments;
        public bool isUnlocked;
        public string previewImagePath;
        // 可选：皮肤材质关联（如果类型为Skin，可直接指向材质）
        public Material skinMaterial;
    }
    public List<GalleryItem> galleryItems = new List<GalleryItem>();

    // ==================== PlayerPrefs 键名常量 ====================
    private const string SAVE_KEY_HIGHEST_LEVEL = "HighestUnlockedLevel";
    private const string SPEEDRUN_UNLOCK_KEY = "SpeedrunUnlocked";
    private const string BEST_TIME_PREFIX = "BestTime_";
    private const string TRUE_ENDING_KEY = "TrueEndingUnlocked";
    private const string TOTAL_FRAGMENTS_KEY = "TotalFragments";
    private const string LEVEL_FULLY_PREFIX = "LevelFully_";
    private const string CURRENT_SKIN_KEY = "CurrentSkin";
    private const string GALLERY_ITEM_PREFIX = "GalleryItem_";
    private const string ADVANCED_CHALLENGE_KEY = "AdvancedChallenge";
    private const string CHALLENGE_TIME_PREFIX = "ChallengeTime_";

    // ==================== Unity 生命周期 ====================
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
            return;
        }

        LoadAllData();
    }

    void Start()
    {
        // 如果有挑战模式开关引用，绑定事件（建议在关卡选择场景单独处理，此处仅示例）
        // 实际使用中，你应该在 LevelSelection 场景中控制 Toggle
    }

    // ==================== 统一数据加载/保存 ====================
    void LoadAllData()
    {
        // 基础进度
        highestUnlockedLevel = PlayerPrefs.GetInt(SAVE_KEY_HIGHEST_LEVEL, 1);

        // 全局碎片 & 关卡全收集
        totalFragmentsCollectedOverall = PlayerPrefs.GetInt(TOTAL_FRAGMENTS_KEY, 0);
        for (int i = 0; i < levelFullyCollected.Length; i++)
        {
            levelFullyCollected[i] = PlayerPrefs.GetInt(LEVEL_FULLY_PREFIX + i, 0) == 1;
        }

        // 极速模式
        isSpeedrunModeUnlocked = PlayerPrefs.GetInt(SPEEDRUN_UNLOCK_KEY, 0) == 1;
        for (int i = 0; i < bestTimes.Length; i++)
        {
            bestTimes[i] = PlayerPrefs.GetFloat(BEST_TIME_PREFIX + (i + 1), -1f);
        }

        // 真结局
        trueEndingUnlocked = PlayerPrefs.GetInt(TRUE_ENDING_KEY, 0) == 1;

        // 皮肤
        currentSkinIndex = PlayerPrefs.GetInt(CURRENT_SKIN_KEY, 0);

        // 挑战模式开关与最佳时间
        isAdvancedChallengeEnabled = PlayerPrefs.GetInt(ADVANCED_CHALLENGE_KEY, 0) == 1;
        for (int i = 0; i < challengeBestTimes.Length; i++)
        {
            challengeBestTimes[i] = PlayerPrefs.GetFloat(CHALLENGE_TIME_PREFIX + i, -1f);
        }

        // 画廊解锁状态
        for (int i = 0; i < galleryItems.Count; i++)
        {
            galleryItems[i].isUnlocked = PlayerPrefs.GetInt(GALLERY_ITEM_PREFIX + i, 0) == 1;
        }
    }

    void SaveAllData()
    {
        PlayerPrefs.SetInt(SAVE_KEY_HIGHEST_LEVEL, highestUnlockedLevel);
        PlayerPrefs.SetInt(TOTAL_FRAGMENTS_KEY, totalFragmentsCollectedOverall);
        for (int i = 0; i < levelFullyCollected.Length; i++)
        {
            PlayerPrefs.SetInt(LEVEL_FULLY_PREFIX + i, levelFullyCollected[i] ? 1 : 0);
        }

        PlayerPrefs.SetInt(SPEEDRUN_UNLOCK_KEY, isSpeedrunModeUnlocked ? 1 : 0);
        for (int i = 0; i < bestTimes.Length; i++)
        {
            if (bestTimes[i] > 0)
                PlayerPrefs.SetFloat(BEST_TIME_PREFIX + (i + 1), bestTimes[i]);
        }

        PlayerPrefs.SetInt(TRUE_ENDING_KEY, trueEndingUnlocked ? 1 : 0);
        PlayerPrefs.SetInt(CURRENT_SKIN_KEY, currentSkinIndex);

        PlayerPrefs.SetInt(ADVANCED_CHALLENGE_KEY, isAdvancedChallengeEnabled ? 1 : 0);
        for (int i = 0; i < challengeBestTimes.Length; i++)
        {
            if (challengeBestTimes[i] > 0)
                PlayerPrefs.SetFloat(CHALLENGE_TIME_PREFIX + i, challengeBestTimes[i]);
        }

        for (int i = 0; i < galleryItems.Count; i++)
        {
            PlayerPrefs.SetInt(GALLERY_ITEM_PREFIX + i, galleryItems[i].isUnlocked ? 1 : 0);
        }

        PlayerPrefs.Save();
    }

    // ==================== 全局碎片与全收集 ====================
    public void AddGlobalFragment(int levelIndex, bool levelCompletedFully)
    {
        totalFragmentsCollectedOverall++;
        if (levelCompletedFully && levelIndex >= 0 && levelIndex < levelFullyCollected.Length)
        {
            levelFullyCollected[levelIndex] = true;
        }
        SaveAllData();
        AutoUnlockGalleryItems();
    }

    // ==================== 画廊自动解锁 ====================
    void AutoUnlockGalleryItems()
    {
        bool anyChange = false;
        foreach (var item in galleryItems)
        {
            if (!item.isUnlocked && totalFragmentsCollectedOverall >= item.costFragments)
            {
                item.isUnlocked = true;
                anyChange = true;
                Debug.Log($"Auto-unlocked: {item.itemName}");
            }
        }
        if (anyChange) SaveAllData();
    }

    // ==================== 皮肤系统 ====================
    public void SetPlayerSkin(int index)
    {
        if (index < 0 || index >= playerSkins.Length) return;
        currentSkinIndex = index;
        SaveAllData();
        // 通知当前场景的玩家更新材质（需要在玩家脚本中实现 UpdateSkin 方法）
        var player = FindAnyObjectByType<player>();
        if (player != null) 
        {
            player.UpdateSkin();
        }
    }

    // 供玩家脚本在 Start 中调用
    public Material GetCurrentSkinMaterial()
    {
        if (playerSkins != null && currentSkinIndex >= 0 && currentSkinIndex < playerSkins.Length)
            return playerSkins[currentSkinIndex];
        return null;
    }

    // ==================== 真结局 ====================
    public void UnlockTrueEnding()
    {
        if (!trueEndingUnlocked)
        {
            trueEndingUnlocked = true;
            // 自动解锁金色皮肤（假设金色皮肤是 gallery 中第4个，cost=12且类型为Skin）
            foreach (var item in galleryItems)
            {
                if (item.type == GalleryItemType.Skin && item.itemName.Contains("Gold"))
                {
                    if (!item.isUnlocked)
                    {
                        item.isUnlocked = true;
                        Debug.Log("True ending unlocked: Golden skin available.");
                    }
                    break;
                }
            }
            SaveAllData();
            Debug.Log("True ending unlocked!");
        }
    }

    // ==================== 极速模式 ====================
    public void UnlockSpeedrunMode()
    {
        if (!isSpeedrunModeUnlocked)
        {
            isSpeedrunModeUnlocked = true;
            SaveAllData();
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
            SaveAllData();
            Debug.Log($"New best time for Level{levelIndex}: {timeInSeconds:F2}s");
        }
    }

    // ==================== 挑战模式 ====================
    public void SetAdvancedChallenge(bool enabled)
    {
        isAdvancedChallengeEnabled = enabled;
        SaveAllData();
    }

    public void RegisterChallengeBestTime(int levelIndex, float timeInSeconds)
    {
        if (levelIndex < 1 || levelIndex > 3) return;
        int idx = levelIndex - 1;
        if (challengeBestTimes[idx] < 0 || timeInSeconds < challengeBestTimes[idx])
        {
            challengeBestTimes[idx] = timeInSeconds;
            SaveAllData();
            Debug.Log($"New challenge best time for Level{levelIndex}: {timeInSeconds:F2}s");
        }
    }

    // ==================== 关卡解锁与加载 ====================
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
            SaveAllData();
        }
    }

    public void LoadLevel(int levelIndex)
    {
        string sceneName = "Level" + levelIndex;
        SceneManager.LoadScene(sceneName);
    }

    // ==================== 难度选择 ====================
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
    }

    // ==================== 公共查询方法（供画廊 UI 使用） ====================
    public bool IsGalleryItemUnlocked(int index)
    {
        if (index < 0 || index >= galleryItems.Count) return false;
        return galleryItems[index].isUnlocked;
    }

    public GalleryItem GetGalleryItem(int index)
    {
        if (index < 0 || index >= galleryItems.Count) return null;
        return galleryItems[index];
    }
}