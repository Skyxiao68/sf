using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Settings")]
    public string difficultySceneName = "DifficultySelection"; // 难度选择场景名
    public string gameSceneName = "Level1";                      // 可选：直接开始游戏时用

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

   
    public void GoToDifficultySelection()
    {
        SceneManager.LoadScene(difficultySceneName);
    }

        public void StartGameDirectly()
    {
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetDifficulty(GameManager.Difficulty.Normal);
        }
        else
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game.");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}