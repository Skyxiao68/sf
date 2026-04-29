using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SpeedrunSelection : MonoBehaviour
{
    public Button[] levelButtons;          // 依次绑定 Level1, Level2, Level3 的按钮
    public TMP_Text[] bestTimeTexts;           // 对应每个按钮下方显示最佳时间的文本

    void Start()
    {
        UpdateBestTimesUI();
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
        }
    }

    void UpdateBestTimesUI()
    {
        if (GameManager.Instance == null) return;
        for (int i = 0; i < bestTimeTexts.Length; i++)
        {
            float best = GameManager.Instance.bestTimes[i];
            if (best > 0)
                bestTimeTexts[i].text = "Best: " + best.ToString("F2") + "s";
            else
                bestTimeTexts[i].text = "Best: --";
        }
    }

    void LoadLevel(int level)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isSpeedrunModeActive = true;
            // 可选：极速模式下强制使用 Normal 难度（可根据需要调整）
            // GameManager.Instance.currentDifficulty = GameManager.Difficulty.Normal;
        }
        SceneManager.LoadScene("Level" + level);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}