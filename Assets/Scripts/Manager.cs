using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Manager : MonoBehaviour
{
    public static Manager instance;
    private string currentScene;
    private Player_Input inputControl; 


    void Awake()
    {
        inputControl = new Player_Input();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        currentScene = SceneManager.GetActiveScene().name;
    }
    
   

    // Update is called once per frame
    void Update()
    {
        if (inputControl.Player.Restart.IsPressed() == true)
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(currentScene);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
