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

    [Header("World Bounds (Fixed)")]
    public Vector2 worldMin = new Vector2(-5f, -5f);
    public Vector2 worldMax = new Vector2(5f, 5f);

    private List<GameObject> currentTokens = new List<GameObject>();
    private List<Image> tokenIcons = new List<Image>();

    void Start()
    {
        RefreshTokenList();
    }

    void Update()
    {
        // 检测硬币数量变化
        GameObject[] tokensInScene = GameObject.FindGameObjectsWithTag("Token");
        if (tokensInScene.Length != currentTokens.Count)
        {
            RefreshTokenList();
        }
        else
        {
            bool needRefresh = false;
            foreach (var t in currentTokens)
                if (t == null) { needRefresh = true; break; }
            if (needRefresh) RefreshTokenList();
        }

        UpdateMapPos(player.position, mapPlayer.rectTransform);
        UpdateMapPos(exit.position, mapExit.rectTransform);

        for (int i = 0; i < currentTokens.Count; i++)
        {
            if (currentTokens[i] != null && currentTokens[i].activeInHierarchy)
            {
                tokenIcons[i].gameObject.SetActive(true);
                UpdateMapPos(currentTokens[i].transform.position, tokenIcons[i].rectTransform);
            }
            else
            {
                tokenIcons[i].gameObject.SetActive(false);
            }
        }
    }

    void RefreshTokenList()
    {
        foreach (var icon in tokenIcons)
            if (icon != null) Destroy(icon.gameObject);
        tokenIcons.Clear();
        currentTokens.Clear();

        GameObject[] allTokens = GameObject.FindGameObjectsWithTag("Token");
        currentTokens.AddRange(allTokens);

        foreach (var token in currentTokens)
        {
            Image icon = Instantiate(mapTokenPrefab, mapPanel);
            icon.rectTransform.localScale = Vector3.one;
            tokenIcons.Add(icon);
        }
    }

    void UpdateMapPos(Vector3 worldPos, RectTransform uiElement)
    {
        float tX = (worldPos.x - worldMin.x) / (worldMax.x - worldMin.x);
        float tZ = (worldPos.z - worldMin.y) / (worldMax.y - worldMin.y);
        tX = Mathf.Clamp01(tX);
        tZ = Mathf.Clamp01(tZ);

        Vector2 panelSize = mapPanel.rect.size;
        float localX = (tX - 0.5f) * panelSize.x;
        float localY = (tZ - 0.5f) * panelSize.y;

        uiElement.anchoredPosition = new Vector2(localX, localY);
    }
}