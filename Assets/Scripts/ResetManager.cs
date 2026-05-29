using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResetManager : MonoBehaviour
{
    public GameObject confirmationPanel;   // 确认弹窗
    public GameObject resetButton;          // 触发重置的按钮（可选）

    void Start()
    {
        confirmationPanel.SetActive(false);
    }

    // 由重置按钮调用
    public void OnResetButtonClick()
    {
        confirmationPanel.SetActive(true);
        // 可选：禁用重置按钮，防止二次点击
        if (resetButton != null) resetButton.GetComponent<Button>().interactable = false;
    }

    // 用户点击“Yes”时调用
    public void ConfirmReset()
    {
        // 重置 GameManager 内存数据并保存
        GameManager.Instance.ResetAllProgress();
        // 重新加载当前场景（让 UI 刷新）
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 用户点击“No”时调用
    public void CancelReset()
    {
        confirmationPanel.SetActive(false);
        if (resetButton != null) resetButton.GetComponent<Button>().interactable = true;
    }
}