using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TempGallery : MonoBehaviour
{
    public Button greenBtn;
    public Button redBtn;
    public Button goldBtn;
    public TMP_Text greenStatus;
    public TMP_Text redStatus;
    public TMP_Text goldStatus;

    void Start()
    {
        UpdateUnlockStatus();
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
        int total = GameManager.Instance.totalFragmentsCollectedOverall;
        greenBtn.interactable = total >= 4;
        redBtn.interactable = total >= 8;
        goldBtn.interactable = total >= 12;

        greenStatus.text = total >= 4 ? " Green Skin Unlocked" : $"locked  {4 - total}";
        redStatus.text = total >= 8 ? "Purple skin Unlocked" : $"locked  {8 - total}";
        goldStatus.text = total >= 12 ? "Gold skin Unlocked" : $"locked  {12 - total}";
    }

    void ApplySkin(int index)
    {
        GameManager.Instance.SetPlayerSkin(index);
    }
}