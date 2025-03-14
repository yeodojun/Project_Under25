using UnityEngine;

public class ETCPanelController : MonoBehaviour
{
    public GameObject targetPanel; // 보여주고/숨길 패널 오브젝트

    // 패널을 보여주는 함수 (버튼에 연결)
    public void ShowPanel()
    {
        if (targetPanel != null)
            targetPanel.SetActive(true);
    }

    // 패널을 숨기는 함수 (X 버튼에 연결)
    public void HidePanel()
    {
        if (targetPanel != null)
            targetPanel.SetActive(false);
    }
}
