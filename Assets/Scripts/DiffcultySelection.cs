using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DifficultySelection : MonoBehaviour
{
    [Header("Scene Settings")]
    public string levelSelectionSceneName = "LevelSelection"; // 新增：关卡选择场景名
    public string mainMenuSceneName = "Main Menu";            // 返回主菜单的场景名

    // 可选：保留原游戏场景名作为备用（如果不需要可直接删除）
    // public string gameSceneName = "Level1";

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
            // 设置难度（注意：此处不能调用 GameManager.Instance.SetDifficulty 自动加载场景的版本，
            // 而是仅保存难度，然后手动加载关卡选择场景）
            GameManager.Instance.SetDifficultyWithoutLoading(difficulty);
            // 手动加载关卡选择场景
            SceneManager.LoadScene(levelSelectionSceneName);
        }
        else
        {
            Debug.LogWarning("GameManager not found! Loading level selection with default difficulty.");
            SceneManager.LoadScene(levelSelectionSceneName);
        }
    }

    void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}