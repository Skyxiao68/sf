using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour
{
    [Header("Mini Map Settings")]
    public Transform player;
    public Transform exit;
    public Image mapTokenPrefab;
    public Image mapPlayer;
    public Image mapExit;
    public RectTransform mapPanel;

    
    private List<GameObject> tokens = new List<GameObject>();
    private List<Image> mapTokens = new List<Image>();

    private Vector3 mapOffset;

    void Start()
    {
        
        GameObject[] allTokens = GameObject.FindGameObjectsWithTag("Token");
        tokens.Clear();
        mapTokens.Clear();

        foreach (GameObject token in allTokens)
        {
            tokens.Add(token);
            Image newIcon = Instantiate(mapTokenPrefab, mapPanel);
            newIcon.rectTransform.localScale = Vector3.one;
            mapTokens.Add(newIcon);
        }

        mapOffset = new Vector3(5, 0, 5);
    }

    void Update()
    {
        
        UpdateMapPos(player.position, mapPlayer.rectTransform);
        UpdateMapPos(exit.position, mapExit.rectTransform);

        
        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i] != null && tokens[i].activeInHierarchy)
            {
                
                mapTokens[i].gameObject.SetActive(true);
                UpdateMapPos(tokens[i].transform.position, mapTokens[i].rectTransform);
            }
            else
            {
                
                mapTokens[i].gameObject.SetActive(false);
            }
        }
    }

    void UpdateMapPos(Vector3 worldPos, RectTransform uiPos)
    {
        float scale = 20f;
        Vector3 localPos = new Vector3(worldPos.x - mapOffset.x, 0, worldPos.z - mapOffset.z);
        uiPos.anchoredPosition = new Vector2(localPos.x * scale, localPos.z * scale);
    }
}