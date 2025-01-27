using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // 배경음악 슬라이더와 텍스트
    public Slider bgmSlider;
    public TextMeshProUGUI bgmText;

    // 효과음 슬라이더와 텍스트
    public Slider sfxSlider;
    public TextMeshProUGUI sfxText;

     // 아이콘 이미지
    public Image bgmMuteIcon;
    public Image sfxMuteIcon;

     // 음소거 버튼
    public Button bgmMuteButton;
    public Button sfxMuteButton;

    // 언어 버튼
    public Button koreanButton;
    public Button englishButton;

    private void Start()
    {
        // 배경음악 초기화
        if (AudioManager.Instance != null && AudioManager.Instance.bgmAudioSource != null)
        {
            bgmSlider.value = AudioManager.Instance.bgmAudioSource.volume;
        }

        // 효과음 초기화
        if (AudioManager.Instance != null && AudioManager.Instance.sfxAudioSource != null)
        {
            sfxSlider.value = AudioManager.Instance.sfxAudioSource.volume;
        }

        // 슬라이더 값 변경 이벤트 등록
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // 음소거 버튼 이벤트
        bgmMuteButton.onClick.AddListener(ToggleBGMMute);
        sfxMuteButton.onClick.AddListener(ToggleSFXMute);

        // 초기 아이콘 업데이트
        UpdateMuteIcons();

        // 버튼 이벤트 연결
        koreanButton.onClick.AddListener(() => ChangeLanguage("Korean"));
        englishButton.onClick.AddListener(() => ChangeLanguage("English"));
    }

    private void SetBGMVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetBGMVolume(volume);
        }

        UpdateBGMText(volume);
    }

    private void SetSFXVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(volume);
        }

        UpdateSFXText(volume);
    }

    private void ToggleBGMMute()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleBGMMute();
            UpdateMuteIcons();
        }
    }

    private void ToggleSFXMute()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleSFXMute();
            UpdateMuteIcons();
        }
    }

    private void UpdateMuteIcons()
    {
        if (AudioManager.Instance != null)
        {
            // BGM 음소거 상태에 따라 아이콘 밝기 조정
            bgmMuteIcon.color = AudioManager.Instance.IsBGMMuted() ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1f);

            // SFX 음소거 상태에 따라 아이콘 밝기 조정
            sfxMuteIcon.color = AudioManager.Instance.IsSFXMuted() ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1f);
        }
    }

    private void UpdateBGMText(float volume)
    {
        int volumePercentage = Mathf.RoundToInt(volume * 100);
        bgmText.text = volumePercentage.ToString();
    }

    private void UpdateSFXText(float volume)
    {
        int volumePercentage = Mathf.RoundToInt(volume * 100);
        sfxText.text = volumePercentage.ToString();
    }

    private void ChangeLanguage(string language)
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.SetLanguage(language);
        }
    }

}