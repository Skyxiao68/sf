using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform exit;
    public Image mapTokenPrefab;
    public Image mapPlayer;
    public Image mapExit;
    public RectTransform mapPanel;          

    private List<Image> tokenIcons = new List<Image>();
    private List<GameObject> tokens = new List<GameObject>();

   
    private Bounds mazeBounds = new Bounds(Vector3.zero, new Vector3(10, 0, 10)); // 中心0,0 范围-5..5

    void Start()
    {
        RefreshTokenList();
       
    }

    void Update()
    {
        RefreshTokenListIfNeeded();

        UpdateMapPos(player.position, mapPlayer.rectTransform);
        UpdateMapPos(exit.position, mapExit.rectTransform);

        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i] != null && tokens[i].activeInHierarchy)
            {
                tokenIcons[i].gameObject.SetActive(true);
                UpdateMapPos(tokens[i].transform.position, tokenIcons[i].rectTransform);
            }
            else
            {
                tokenIcons[i].gameObject.SetActive(false);
            }
        }
    }

    void RefreshTokenListIfNeeded()
    {
        GameObject[] currentTokens = GameObject.FindGameObjectsWithTag("Token");
        if (currentTokens.Length != tokens.Count)
            RefreshTokenList();
    }

    void RefreshTokenList()
    {
        foreach (var icon in tokenIcons)
            if (icon != null) Destroy(icon.gameObject);
        tokenIcons.Clear();
        tokens.Clear();

        GameObject[] allTokens = GameObject.FindGameObjectsWithTag("Token");
        tokens.AddRange(allTokens);

        foreach (var token in tokens)
        {
            Image icon = Instantiate(mapTokenPrefab, mapPanel);
            icon.rectTransform.localScale = Vector3.one;
            tokenIcons.Add(icon);
        }
    }

    
    void UpdateMapPos(Vector3 worldPos, RectTransform uiElement)
    {
        
        float tX = (worldPos.x - mazeBounds.min.x) / mazeBounds.size.x;
        float tZ = (worldPos.z - mazeBounds.min.z) / mazeBounds.size.z;

       
        Vector2 panelSize = mapPanel.rect.size;
        float localX = (tX - 0.5f) * panelSize.x;
        float localY = (tZ - 0.5f) * panelSize.y;

       
        uiElement.anchoredPosition = new Vector2(localX, localY);
    }
}