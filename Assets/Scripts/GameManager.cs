using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.InputSystem;

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
    public int totalFragmentsCollectedOverall
    {
        get { return levelFullyCollected.Count(x => x) * 4; }
    }

    public bool[] levelFullyCollected = new bool[3];      // 每个关卡是否全收集
    public const int TOTAL_FRAGMENTS_ALL_LEVELS = 12;
    public bool trueEndingUnlocked = false;

    public bool anyFragmentCollectedOverall = false;
    private const string ANY_FRAGMENT_KEY = "AnyFragmentCollected";

    [System.Serializable]
    public class FragmentStory
    {
        [TextArea(2, 4)] public string[] fragments;
        [TextArea(4, 8)] public string[] fullStory;
    }
    public FragmentStory story;

    public bool isGameFinished = false;

    // ==================== 极速模式 ====================
    [Header("Speedrun Mode")]
    public bool isSpeedrunModeUnlocked = false;
    public bool isSpeedrunModeActive = false;
    public float[] bestTimes = new float[3];   // 普通模式最佳时间

    // ==================== 挑战模式 ====================
    [Header("Challenge Mode")]
    public bool isAdvancedChallengeEnabled = false;
    public float[] challengeBestTimes = new float[3];   // 挑战模式最佳时间
    public bool[] challengeCompleted = new bool[3];   // 挑战模式是否完成

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

    // ==================== 无障碍选项 ====================
    [Header("Accessibility")]
    public bool disableScreenShake = false;
    private const string ACCESS_SCREEN_SHAKE_KEY = "DisableScreenShake";


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

        // 首次启动检测
        if (!PlayerPrefs.HasKey("FirstLaunch"))
        {
            Debug.Log("First launch: resetting all progress.");
            PlayerPrefs.DeleteAll();
            // 重新初始化默认值
            anyFragmentCollectedOverall = false; // 重置全局碎片收集状态
            highestUnlockedLevel = 1;
            
            for (int i = 0; i < levelFullyCollected.Length; i++) levelFullyCollected[i] = false;
            isSpeedrunModeUnlocked = false;
            trueEndingUnlocked = false;
            currentSkinIndex = 0;
            isAdvancedChallengeEnabled = false;
            for (int i = 0; i < bestTimes.Length; i++) bestTimes[i] = -1f;
            for (int i = 0; i < challengeBestTimes.Length; i++) challengeBestTimes[i] = -1f;
            if (challengeCompleted != null)
                for (int i = 0; i < challengeCompleted.Length; i++) challengeCompleted[i] = false;
            if (galleryItems != null)
            {
                // 默认皮肤（索引0）始终解锁，其他皮肤解锁状态重置
                for (int i = 0; i < galleryItems.Count; i++)
                    galleryItems[i].isUnlocked = (i == 0);
            }
            SaveAllData();
            PlayerPrefs.SetInt("FirstLaunch", 1);
            PlayerPrefs.Save();
        }
        else
        {
            LoadAllData();
        }
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
        
        for (int i = 0; i < levelFullyCollected.Length; i++)
        {
            levelFullyCollected[i] = PlayerPrefs.GetInt(LEVEL_FULLY_PREFIX + i, 0) == 1;
        }

        anyFragmentCollectedOverall = PlayerPrefs.GetInt(ANY_FRAGMENT_KEY, 0) == 1;

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

        // 加载挑战完成标志
        for (int i = 0; i < challengeCompleted.Length; i++)
        {
            challengeCompleted[i] = PlayerPrefs.GetInt("ChallengeCompleted_" + i, 0) == 1;
        }

        disableScreenShake = PlayerPrefs.GetInt(ACCESS_SCREEN_SHAKE_KEY, 0) == 1;

    }

    public void SaveAllData()
    {
        PlayerPrefs.SetInt(SAVE_KEY_HIGHEST_LEVEL, highestUnlockedLevel);
        PlayerPrefs.SetInt(TOTAL_FRAGMENTS_KEY, totalFragmentsCollectedOverall);
        PlayerPrefs.SetInt(ANY_FRAGMENT_KEY, anyFragmentCollectedOverall ? 1 : 0);
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

        // 保存挑战完成标志
        for (int i = 0; i < challengeCompleted.Length; i++)
        {
            PlayerPrefs.SetInt("ChallengeCompleted_" + i, challengeCompleted[i] ? 1 : 0);
        }

        PlayerPrefs.SetInt(ACCESS_SCREEN_SHAKE_KEY, disableScreenShake ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void SetDisableScreenShake(bool value)
    {
        disableScreenShake = value;
        SaveAllData();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.f1Key.wasPressedThisFrame) SetPlayerSkin(0);
        if (Keyboard.current.f2Key.wasPressedThisFrame) SetPlayerSkin(1);
        if (Keyboard.current.f3Key.wasPressedThisFrame) SetPlayerSkin(2);
        if (Keyboard.current.f4Key.wasPressedThisFrame) SetPlayerSkin(3);
    }

    // ==================== 全局碎片与全收集 ====================
    public void AddGlobalFragment(int levelIndex, bool levelCompletedFully)
    {
        // 不再累加 totalFragmentsCollectedOverall
        if (!anyFragmentCollectedOverall)
            anyFragmentCollectedOverall = true;
        int idx = levelIndex - 1;
        if (levelCompletedFully && idx >= 0 && idx < levelFullyCollected.Length)
        {
            levelFullyCollected[idx] = true;
            Debug.Log($"Level {levelIndex} fully collected flag set to true");
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
            // 皮肤类型不由碎片数量自动解锁
            if (item.type == GalleryItemType.Skin) continue;

            if(!item.isUnlocked && totalFragmentsCollectedOverall >= item.costFragments)
            {
                item.isUnlocked = true;
                anyChange = true;
                Debug.Log($"Auto-unlocked: {item.itemName}");
            }
        }
        if (anyChange) SaveAllData();
    }

    public void UnlockLevelReward(int levelIndex, bool fullyCollected)
    {
        if (!fullyCollected) return;

        int skinIndex = -1;
        string skinName = "";
        if (levelIndex == 1) { skinName = "Green"; skinIndex = 1; }
        else if (levelIndex == 2) { skinName = "Purple"; skinIndex = 2; }
        else if (levelIndex == 3)
        {
            // 第三关全收集不直接解锁金色皮肤，而是仅记录（如果有其他奖励可在这里加）
            // 金色皮肤由 UnlockTrueEnding 解锁
            Debug.Log("Level 3 fully collected, but Gold skin will be unlocked only when all levels are fully collected.");
            return;   // 直接返回，不执行后续皮肤解锁
        }
        else return;

        // 解锁绿色或紫色皮肤（索引1和2）
        if (galleryItems != null && galleryItems.Count > skinIndex && galleryItems[skinIndex].type == GalleryItemType.Skin)
        {
            var skinItem = galleryItems[skinIndex];
            if (!skinItem.isUnlocked)
            {
                skinItem.isUnlocked = true;
                SaveAllData();
                Debug.Log($"Unlocked {skinName} skin for Level {levelIndex}");
                SetPlayerSkin(skinIndex);
            }
        }
    }

    // ==================== 皮肤系统 ====================
    public void SetPlayerSkin(int index)
    {
        Debug.Log($"SetPlayerSkin called with index {index}");
        if (index < 0 || index >= playerSkins.Length)
        {
            Debug.LogError($"Invalid skin index {index}");
            return;
        }
        currentSkinIndex = index;
        SaveAllData();
        Debug.Log($"Skin changed to index {index} ({playerSkins[index].name})");

        var player = FindAnyObjectByType<player>();
        if (player != null)
        {
            player.UpdateSkin();
            Debug.Log("Player found, UpdateSkin called");
        }
        else
        {
            Debug.Log("No player in scene (main menu), skin saved for next level");
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
        if (!trueEndingUnlocked && levelFullyCollected.All(x => x))
        {
            trueEndingUnlocked = true;
            // 解锁金色皮肤（假设索引为3）
            if (galleryItems.Count > 3 && !galleryItems[3].isUnlocked)
            {
                galleryItems[3].isUnlocked = true;
                Debug.Log("True ending unlocked: Golden skin available.");
                SetPlayerSkin(3);
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

    public void ResetAllProgress()
    {
        // 重置所有内存变量为默认值
        anyFragmentCollectedOverall = false;
        highestUnlockedLevel = 1;
        for (int i = 0; i < levelFullyCollected.Length; i++) levelFullyCollected[i] = false;
        isSpeedrunModeUnlocked = false;
        trueEndingUnlocked = false;
        currentSkinIndex = 0;
        isAdvancedChallengeEnabled = false;
        for (int i = 0; i < bestTimes.Length; i++) bestTimes[i] = -1f;
        for (int i = 0; i < challengeBestTimes.Length; i++) challengeBestTimes[i] = -1f;
        if (challengeCompleted != null)
            for (int i = 0; i < challengeCompleted.Length; i++) challengeCompleted[i] = false;
        if (galleryItems != null)
        {
            // 默认皮肤（索引0）解锁，其他全部锁定
            for (int i = 0; i < galleryItems.Count; i++)
                galleryItems[i].isUnlocked = (i == 0);
        }

        // 不清除 FirstLaunch，只清除游戏进度数据
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("FirstLaunch", 1); // 重新设置标志
        PlayerPrefs.Save();

        // 保存这些默认值到 PlayerPrefs
        SaveAllData();
    }
}