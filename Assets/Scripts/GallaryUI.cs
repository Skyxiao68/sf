using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GallaryUI : MonoBehaviour
{
    public Transform contentParent;
    public GameObject itemButtonPrefab;

    void Start()
    {
        RefreshUI();
    }

    void RefreshUI()
    {
        
        foreach (Transform child in contentParent) Destroy(child.gameObject);

        for (int i = 0; i < GameManager.Instance.gallaryItems.Count; i++)
        {
            var item = GameManager.Instance.gallaryItems[i];
            GameObject btnObj = Instantiate(itemButtonPrefab, contentParent);
            Button btn = btnObj.GetComponent<Button>();
            TMP_Text text = btnObj.GetComponentInChildren<TMP_Text>();
            text.text = $"{item.itemName} - {item.costFragments} fragments";

            bool unlocked = GameManager.Instance.IsGalleryItemUnlocked(i);
            btn.interactable = !unlocked && GameManager.Instance.totalFragmentsCollectedOverall >= item.costFragments;
            if (unlocked)
                text.text += " [UNLOCKED]";

            int index = i;
            btn.onClick.AddListener(() => TryUnlock(index));
        }
    }

    void TryUnlock(int index)
    {
        GameManager.Instance.UnlockGalleryItem(index);
        RefreshUI();
        
    }
}