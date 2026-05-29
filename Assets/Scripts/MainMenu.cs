using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Settings")]
    public string difficultySceneName = "DifficultySelection"; 
    public string gameSceneName = "Level1";     
    public Toggle shakeToggle; 


    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        shakeToggle.isOn = GameManager.Instance.disableScreenShake;
        shakeToggle.onValueChanged.AddListener(val => GameManager.Instance.SetDisableScreenShake(val));
    }

   
    public void GoToDifficultySelection()
    {
        if(SceneFadeManager.Instance!= null)
        {
            SceneFadeManager.Instance.LoadScene(difficultySceneName);
        }
        else
        {
            SceneManager.LoadScene(difficultySceneName);
        }
    }

    public void StartGameDirectly()
    {
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetDifficulty(GameManager.Difficulty.Normal);

            if(SceneFadeManager.Instance!= null)
            {
                SceneFadeManager.Instance.LoadScene(gameSceneName);
            }
        }
        else
        {
            if (SceneFadeManager.Instance!= null)
            {
                SceneFadeManager.Instance.LoadScene(gameSceneName);
            }
            else
            {
                SceneManager.LoadScene(gameSceneName);
            }
        }
    }

    public void GoToMemoryGallary()
    {
        SceneManager.LoadScene("MemoryGallary");  
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