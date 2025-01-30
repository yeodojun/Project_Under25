using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ExplainPanelController : MonoBehaviour
{
    public GameObject glowEffect;
    public int gameIndex; // 1 또는 2 설정

    private static List<ExplainPanelController> allPanels = new List<ExplainPanelController>();
    private bool isSelected = false; // 현재 패널이 선택되었는지 여부

    private void Start()
    {
        if (glowEffect != null)
        {
            glowEffect.SetActive(false);
        }
        else
        {
            Debug.LogError("GlowEffect가 연결되지 않았습니다.");
        }

        // 모든 패널을 리스트에 추가
        if (!allPanels.Contains(this))
        {
            allPanels.Add(this);
        }
    }

    public void OnPanelClick()
    {
        if (isSelected)
        {
            // 패널이 이미 선택된 상태에서 다시 클릭하면 CharacterScene으로 이동
            GameManager.Instance.SetSelectedGame(gameIndex); // 선택한 게임 저장
            SceneManager.LoadScene("CharacterScene");
            return;
        }

        // 모든 패널의 GlowEffect를 끄고 현재 패널만 활성화
        foreach (var panel in allPanels)
        {
            if (panel != this)
            {
                panel.glowEffect.SetActive(false);
                panel.isSelected = false; // 다른 패널은 선택 해제
            }
        }

        // 현재 패널 GlowEffect 활성화
        glowEffect.SetActive(true);
        isSelected = true; // 선택된 상태로 변경
    }
}