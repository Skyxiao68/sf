using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GalleryUI : MonoBehaviour
{
    public Transform contentParent;          // ScrollView 的 Content 物体
    public GameObject itemButtonPrefab;      // 预制体：包含 Button 和 Text 组件
    public GameObject detailPanel;           // 详情弹窗面板（可选）
    public TMP_Text detailText;              // 详情文本
    public Image detailImage;                // 详情图片（可选）

    void Start()
    {
        RefreshUI();
    }

    void RefreshUI()
    {
        // 清空原有内容
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        List<GameManager.GalleryItem> items = GameManager.Instance.galleryItems;
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            GameObject btnObj = Instantiate(itemButtonPrefab, contentParent);
            TMP_Text btnText = btnObj.GetComponentInChildren<TMP_Text>();
            Button btn = btnObj.GetComponent<Button>();

            if (item.isUnlocked)
            {
                btnText.text = $"✅ {item.itemName}";
                btn.interactable = true;
                int index = i;
                btn.onClick.AddListener(() => ShowItemDetail(index));
            }
            else
            {
                btnText.text = $"🔒 {item.itemName} (need {item.costFragments} fragments)";
                btn.interactable = false;
            }
        }
    }

    void ShowItemDetail(int index)
    {
        var item = GameManager.Instance.galleryItems[index];
        if (item.type == GameManager.GalleryItemType.Skin)
        {
            // 皮肤类型：直接应用
            GameManager.Instance.SetPlayerSkin(index);
            Debug.Log($"Applied skin: {item.itemName}");
            if (detailPanel != null) detailPanel.SetActive(false);
        }
        else
        {
            // 概念艺术、留言等：显示详情弹窗
            if (detailPanel != null)
            {
                if (detailText != null)
                    detailText.text = item.previewImagePath; // 可扩展为实际文本内容
                // 如果有图片可加载，设置 detailImage.sprite = ...
                detailPanel.SetActive(true);
            }
        }
    }

    public void CloseDetailPanel()
    {
        if (detailPanel != null)
            detailPanel.SetActive(false);
    }
}