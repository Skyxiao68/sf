using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum Difficulty { Easy, Normal }
    public Difficulty currentDifficulty = Difficulty.Normal;

    [System.Serializable]
    public class FragmentStory
    {
        [TextArea(2,4)]
        public string[] fragments;

        [TextArea(4,8)]
        public string[] fullStory;
    }
    public FragmentStory story;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        Debug.Log("Difficulty set to : " + currentDifficulty);
        SceneManager.LoadScene("Level1"); 
    }
    
}
