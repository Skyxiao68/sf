using UnityEngine;
using UnityEngine.UI;


public class MiniMap : MonoBehaviour
{
    [Header("Mini Map settings")]
    public Transform player; 
    public GameObject[] Tokens;
    public Transform exit;
    public Image mapTokenPrefab; 
    public Image mapPlayer; 
    public Image mapExit;
    public RectTransform mapPanel; 

    private Image[] mapTokens;
    private Vector3 mapOffset; 

    void Start()
    {
       
        mapTokens = new Image[Tokens.Length];
        for (int i = 0; i < Tokens.Length; i++)
        {
            mapTokens[i] = Instantiate(mapTokenPrefab, mapPanel);
            mapTokens[i].rectTransform.localScale = Vector3.one;
        }
        
        mapOffset = new Vector3(5, 0, 5);
    }

    void Update()
    {
        
        UpdateMapPos(player.position, mapPlayer.rectTransform);
        
        UpdateMapPos(exit.position, mapExit.rectTransform);
      
        for (int i = 0; i < Tokens.Length; i++)
        {
            if (Tokens[i] != null)
            {
                UpdateMapPos(Tokens[i].transform.position, mapTokens[i].rectTransform);
            }
            else
            {
                mapTokens[i].gameObject.SetActive(false); 
        }
    }

    
    void UpdateMapPos(Vector3 worldPos, RectTransform uiPos)
    {
      
        float scale = 20f;
        Vector3 localPos = new Vector3(worldPos.x - mapOffset.x, 0, worldPos.z - mapOffset.z);
        uiPos.anchoredPosition = new Vector2(localPos.x * scale, localPos.z * scale);
    }

}
}
