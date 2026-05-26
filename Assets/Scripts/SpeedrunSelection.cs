using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SpeedrunSelection : MonoBehaviour
{
    public Button[] levelButtons;
    public TMP_Text[] normalBestTimeTexts;
    public TMP_Text[] challengeBestTimeTexts;
    public Toggle advancedChallengeToggle;   // 新增：挑战模式开关

    void Start()
    {
        UpdateBestTimes();

        // 绑定关卡按钮
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            levelButtons[i].onClick.AddListener(() => LoadNormalSpeedrun(levelIndex));
        }

        // 初始化挑战模式开关（与 GameManager 同步）
        if (advancedChallengeToggle != null && GameManager.Instance != null)
        {
            advancedChallengeToggle.isOn = GameManager.Instance.isAdvancedChallengeEnabled;
            advancedChallengeToggle.onValueChanged.AddListener(OnAdvancedChallengeToggled);
        }
    }

    void OnEnable()
    {
        UpdateBestTimes();
        // 同步开关状态（以防其他场景修改过）
        if (advancedChallengeToggle != null && GameManager.Instance != null)
        {
            advancedChallengeToggle.isOn = GameManager.Instance.isAdvancedChallengeEnabled;
        }
    }

    void UpdateBestTimes()
    {
        if (GameManager.Instance == null) return;

        for (int i = 0; i < normalBestTimeTexts.Length && i < GameManager.Instance.bestTimes.Length; i++)
        {
            float t = GameManager.Instance.bestTimes[i];
            normalBestTimeTexts[i].text = t > 0 ? $"Best: {t:F2}s" : "Best: --";
        }

        for (int i = 0; i < challengeBestTimeTexts.Length && i < GameManager.Instance.challengeBestTimes.Length; i++)
        {
            float t = GameManager.Instance.challengeBestTimes[i];
            challengeBestTimeTexts[i].text = t > 0 ? $"Challenge Best: {t:F2}s" : "Chal. Best: --";
        }
    }

    void OnAdvancedChallengeToggled(bool isOn)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SetAdvancedChallenge(isOn);
    }

    void LoadNormalSpeedrun(int level)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isSpeedrunModeActive = true;
            // 注意：挑战模式和极速模式是独立的，这里只设置极速模式标志
        }
        SceneManager.LoadScene("Level" + level);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}