using UnityEngine;
using UnityEngine.UI;

public class SettingPanelController : MonoBehaviour
{
    public GameObject settingPanel; // SettingPanel 오브젝트
    public Button settingButton;    // 설정 버튼
    public Button closeButton;      // X 버튼

    private void Start()
    {
        // 버튼 클릭 이벤트 등록
        if (settingButton != null)
        {
            settingButton.onClick.AddListener(ShowSettingPanel);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideSettingPanel);
        }

        // 초기에는 패널을 비활성화
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("🚨 [SettingPanelController] SettingPanel이 할당되지 않았습니다!");
        }
    }

    public void ShowSettingPanel()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
            Debug.Log("✅ SettingPanel 활성화됨");
        }
    }

    public void HideSettingPanel()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
            Debug.Log("✅ SettingPanel 비활성화됨");
        }
    }
}
