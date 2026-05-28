using UnityEngine;
using UnityEngine.UI;

public class SkinPanel : MonoBehaviour
{
    public Button greenSkinBtn;
    public Button redSkinBtn;
    public Button goldSkinBtn;

    void OnEnable()
    {
        // 移除旧监听，防止重复
        if (greenSkinBtn != null) greenSkinBtn.onClick.RemoveAllListeners();
        if (redSkinBtn != null) redSkinBtn.onClick.RemoveAllListeners();
        if (goldSkinBtn != null) goldSkinBtn.onClick.RemoveAllListeners();

        // 添加新监听
        if (greenSkinBtn != null) greenSkinBtn.onClick.AddListener(() => ApplySkin(1));
        if (redSkinBtn != null) redSkinBtn.onClick.AddListener(() => ApplySkin(2));
        if (goldSkinBtn != null) goldSkinBtn.onClick.AddListener(() => ApplySkin(3));

        Debug.Log("SkinPanel OnEnable: buttons rebound.");
    }

    void ApplySkin(int index)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPlayerSkin(index);
            Debug.Log($"Skin button clicked: index {index}");
        }
        else
        {
            Debug.LogError("GameManager.Instance is null!");
        }
    }
}