using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TempGallery : MonoBehaviour
{
    public Button defaultBtn;   // 新增默认皮肤按钮
    public Button greenBtn;
    public Button redBtn;
    public Button goldBtn;
    public TMP_Text defaultStatus;
    public TMP_Text greenStatus;
    public TMP_Text redStatus;
    public TMP_Text goldStatus;

    void Start()
    {
        UpdateUnlockStatus();
        defaultBtn.onClick.AddListener(() => ApplySkin(0));
        greenBtn.onClick.AddListener(() => ApplySkin(1));
        redBtn.onClick.AddListener(() => ApplySkin(2));
        goldBtn.onClick.AddListener(() => ApplySkin(3));
    }

    void OnEnable()
    {
        UpdateUnlockStatus();
    }

    void UpdateUnlockStatus()
    {
        // 从 GameManager 的画廊数据读取实际解锁状态
        bool defaultUnlocked = GameManager.Instance.IsGalleryItemUnlocked(0); // 索引0应为默认皮肤（始终解锁）
        bool greenUnlocked = GameManager.Instance.IsGalleryItemUnlocked(1);
        bool redUnlocked = GameManager.Instance.IsGalleryItemUnlocked(2);
        bool goldUnlocked = GameManager.Instance.IsGalleryItemUnlocked(3);

        defaultBtn.interactable = defaultUnlocked;
        greenBtn.interactable = greenUnlocked;
        redBtn.interactable = redUnlocked;
        goldBtn.interactable = goldUnlocked;

        defaultStatus.text = defaultUnlocked ? "Default Skin" : "Locked";
        greenStatus.text = greenUnlocked ? "Green Skin" : "Locked (Fully Complete Level 1)";
        redStatus.text = redUnlocked ? "Purple Skin" : "Locked (Fully Complete Level 2)";
        goldStatus.text = goldUnlocked ? "Gold Skin" : "Locked (Complete True Ending)";

        defaultBtn.interactable = true;
        defaultStatus.text = "Default Skin"; 
    }

    void ApplySkin(int index)
    {
        Debug.Log($"ApplySkin called with index {index}");
        GameManager.Instance.SetPlayerSkin(index);
    }
}