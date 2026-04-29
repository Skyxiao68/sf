using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelection : MonoBehaviour
{
    public Button speedrunButton;
    public Button[] levelButtons;      
    public TMP_Text[] statusTexts;           
    public GameObject[] lockIcons;       

    void Start()
    {
        UpdateUI();
        if (speedrunButton != null)
        {   
            bool unlocked = GameManager.Instance != null && GameManager.Instance.isSpeedrunModeUnlocked;
            speedrunButton.gameObject.SetActive(unlocked);
            if (unlocked)
            {
                speedrunButton.onClick.AddListener(GoToSpeedrunMode);
            }
        }
    }

    public void GoToSpeedrunMode()
    {
        SceneManager.LoadScene("SpeedrunSelection");
    }

    void UpdateUI()
    {
        int highestUnlocked = GameManager.Instance != null ? GameManager.Instance.highestUnlockedLevel : 1;
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNum = i + 1;
            bool unlocked = levelNum <= highestUnlocked;
            levelButtons[i].interactable = unlocked;
            if (lockIcons != null && lockIcons.Length > i)
                lockIcons[i].SetActive(!unlocked);
            if (statusTexts != null && statusTexts.Length > i)
                statusTexts[i].text = unlocked ? "Available" : "Locked";
        }
    }

    public void OnLevelButtonClick(int levelIndex)
    {
        if (GameManager.Instance != null && levelIndex + 1 <= GameManager.Instance.highestUnlockedLevel)
        {
            GameManager.Instance.LoadLevel(levelIndex + 1);
        }
        else
        {
            Debug.Log("Level not unlocked yet");
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}