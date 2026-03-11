using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEditor;

public class Manager : MonoBehaviour
{
    
    public GameObject pauseMenuUI;
    public GameObject gametut; 
  
    private bool isPaused = false;
    private Player_Input inputControl;

    void Awake()
    {
        
        inputControl = new Player_Input();
        
        

    
    }

    void OnEnable()
    {
        inputControl.Enable();
        
       
    }

    void OnDisable()
    {
        inputControl.Disable();
        
    }

    public void Start()
    {
        Time.timeScale = 0f;
        gametut.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (inputControl.Player.Submit.WasPressedThisFrame())
        {
            gametut.SetActive(false);
            Time.timeScale = 1f;   

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        
        if (inputControl.Player.Pause.WasPressedThisFrame())
        {
            if (isPaused == true)
            {
                ResumeGame();
            }
            else
            {
                GamePause();
            }
        }

        if (inputControl.Player.Restart.WasPressedThisFrame())
        {
            RestartGame();
        }
    }

   
    public void GamePause()
    {
            Debug.Log("pause game");
        
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isPaused = true;
        
    }

    
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Debug.Log("Game Resumed");

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
       
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}