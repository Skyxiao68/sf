using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DifficultySelection : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameSceneName = "Level1";     // 游戏场景名称
    public string mainMenuSceneName = "Main Menu"; // 返回主菜单的场景名

    public Button easyButton;
    public Button normalButton;
    public Button backButton;                  

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (easyButton != null)
            easyButton.onClick.AddListener(() => StartGameWithDifficulty(GameManager.Difficulty.Easy));

        if (normalButton != null)
            normalButton.onClick.AddListener(() => StartGameWithDifficulty(GameManager.Difficulty.Normal));

        if (backButton != null)
            backButton.onClick.AddListener(BackToMainMenu);
    }

    void StartGameWithDifficulty(GameManager.Difficulty difficulty)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetDifficulty(difficulty);
        }
        else
        {
           
            Debug.LogWarning("GameManager not found! Loading game with default difficulty.");
            SceneManager.LoadScene(gameSceneName);
        }
    }

    void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}