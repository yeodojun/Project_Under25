using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShootingSettingPanelController : MonoBehaviour
{
    // 게임 중 일시정지 시 나타나는 패널 (PausePanel)
    public GameObject pausePanel;
    // 일시정지 패널 내의 설정 버튼을 누르면 나타나는 설정 패널 (MainScene과 연동된 설정 패널)
    public GameObject inGameSettingsPanel;
    // HUD에 표시되는 일시정지 버튼
    public GameObject pauseButton;

    void Start()
    {
        // 게임 시작 시 두 패널 모두 숨김
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (inGameSettingsPanel != null)
            inGameSettingsPanel.SetActive(false);
        if (pauseButton != null)
            pauseButton.SetActive(true);
    }

    // 게임 중 HUD의 일시정지 버튼을 누르면 호출
    public void OnPauseButtonPressed()
    {
        Time.timeScale = 0f;
        if (pausePanel != null)
            pausePanel.SetActive(true);
        if (pauseButton != null)
            pauseButton.SetActive(false);
    }

    // 일시정지 패널 내의 "계속하기" 버튼을 누르면 호출
    public void OnResumeButtonPressed()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (pauseButton != null)
            pauseButton.SetActive(true);
        StartCoroutine(ResumeGameAfterDelay());
    }

    // 일시정지 패널 내의 "게임 종료" 버튼을 누르면 호출
    public void OnExitButtonPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }

    // 일시정지 패널 내의 "설정" 버튼을 누르면 호출
    // 이때 일시정지 패널은 숨기고, 설정 패널(inGameSettingsPanel)을 활성화
    public void OnInGameSettingsButtonPressed()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (inGameSettingsPanel != null)
            inGameSettingsPanel.SetActive(true);
    }

    // 설정 패널 내의 "닫기" 버튼을 누르면 호출
    // 설정 패널을 숨기고 다시 일시정지 패널 등장
    public void OnCloseSettingsButtonPressed()
    {
        if (inGameSettingsPanel != null)
            inGameSettingsPanel.SetActive(false);
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    private IEnumerator ResumeGameAfterDelay()
    {
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;
    }
}
