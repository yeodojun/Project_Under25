using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ExplainPanelController : MonoBehaviour
{
    public GameObject glowEffect;
    public int gameIndex; // 1 또는 2 설정

    private static List<ExplainPanelController> allPanels = new List<ExplainPanelController>();
    private static ExplainPanelController selectedPanel = null; // 현재 선택된 패널
    private bool isSelected = false; // 현재 패널이 선택되었는지 여부

    private void Start()
    {
        // GameMainScene이 다시 로드될 때 기존 리스트 초기화
        if (SceneManager.GetActiveScene().name == "GameMainScene")
        {
            allPanels.Clear();
            selectedPanel = null;
        }

        if (glowEffect != null)
        {
            glowEffect.SetActive(false);
        }
        else
        {
            Debug.LogError("GlowEffect가 연결되지 않았습니다.");
        }

        // 현재 패널을 리스트에 추가
        allPanels.Add(this);
    }

    public void OnPanelClick()
    {
        if (selectedPanel == this)
        {
            // 🔹 같은 패널을 두 번 클릭하면 다음 씬으로 이동
            if (GameManager.Instance == null)
            {
                GameManager.Instance = FindObjectOfType<GameManager>();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetSelectedGame(gameIndex);
                SceneManager.LoadScene("CharacterScene");
            }
            else
            {
                Debug.LogError("GameManager 인스턴스를 찾을 수 없습니다. 씬에 존재하는지 확인하세요.");
            }
            return;
        }

        // 🔹 기존 선택된 패널의 GlowEffect 끄기
        if (selectedPanel != null && selectedPanel.glowEffect != null)
        {
            selectedPanel.glowEffect.SetActive(false);
            selectedPanel.isSelected = false;
        }

        // 🔹 현재 패널 선택
        glowEffect.SetActive(true);
        isSelected = true;
        selectedPanel = this; // 현재 패널을 선택된 패널로 설정
    }

    private void OnEnable()
    {
        // 🔹 씬이 다시 로드될 때 GlowEffect를 끄고 선택 상태 초기화
        isSelected = false;
        if (glowEffect != null)
        {
            glowEffect.SetActive(false);
        }
    }
}
