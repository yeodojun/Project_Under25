using System.Collections;
using UnityEngine;

public class ShootingSettingPanelController : MonoBehaviour
{
    // 설정 패널을 인스펙터에서 할당합니다.
    public GameObject settingsPanel;

    void Start()
    {
        // 게임 시작 시 패널은 숨깁니다.
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    // 게임 중 설정 버튼을 누르면 호출됩니다.
    // 게임을 일시정지시키고 설정 패널을 활성화합니다.
    public void PauseGame()
    {
        // 게임 일시정지 (Time.timeScale = 0이면 모든 코루틴 WaitForSeconds는 멈추므로 주의)
        Time.timeScale = 0f;
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    // 설정 패널 내의 "계속하기" 버튼을 눌렀을 때 호출됩니다.
    // 패널을 숨기고 3초 후에 게임을 다시 재개합니다.
    public void OnResumeButtonPressed()
    {
        // 패널 숨김
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        // Time.timeScale이 0이어도 WaitForSecondsRealtime는 실제 시간 기준 대기하므로 사용합니다.
        StartCoroutine(ResumeGameAfterDelay());
    }

    private IEnumerator ResumeGameAfterDelay()
    {
        // 3초간 실제 시간으로 대기합니다.
        yield return new WaitForSecondsRealtime(3f);
        // 게임 재개
        Time.timeScale = 1f;
    }
}
