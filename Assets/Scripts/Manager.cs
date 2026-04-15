using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEditor;
using System.Collections;

public class Manager : MonoBehaviour
{
    
    public GameObject pauseMenuUI;
    public GameObject gametut; 
  
    private bool isPaused = false;
    private Player_Input inputControl;
    private CanvasGroup pauseCanvasGroup;

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
        if (pauseMenuUI != null)
        {
            pauseCanvasGroup = pauseMenuUI.GetComponent<CanvasGroup>();
        }
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
            StartCoroutine(FadeUI(pauseCanvasGroup, 1f, 0.2f, true));
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isPaused = true;
        
    }

    
    public void ResumeGame()
    {
        StartCoroutine(FadeUI(pauseCanvasGroup, 0f, 0.2f, false, () =>
        {   
            pauseMenuUI.SetActive(false);
            Debug.Log("Game Resumed");

            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }));

        isPaused = false;
    }

    private IEnumerator FadeUI(CanvasGroup cg, float targetAlpha, float duration, bool interactable, System.Action onCompelte = null)
    {
        if (cg == null) yield break;

        float startAlpha = cg.alpha;
        float elapsed = 0f; 
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        cg.alpha = targetAlpha;
        cg.interactable = interactable;
        cg.blocksRaycasts = interactable;
        onCompelte?.Invoke();
        
    }

    public void LoadMainMenu()
    {
        if(SceneFadeManager.Instance != null)
        {
            Time.timeScale = 1f;
            SceneFadeManager.Instance.LoadScene("Main Menu");
        }
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Main Menu");
        }
        
    }

    public void RestartGame()
    {
       if(SceneFadeManager.Instance != null)
        {
            Time.timeScale = 1f;
            SceneFadeManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
        
        }
        else
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
    }

    public void QuitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    void OnDestroy()
    {
        inputControl?.Dispose();
    }
}