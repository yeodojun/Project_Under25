using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // 싱글톤 인스턴스

    public AudioSource bgmAudioSource;   // 배경음악 AudioSource
    public AudioSource sfxAudioSource;   // 효과음 AudioSource

    private bool isBGMMuted = false; // BGM 음소거 상태
    private bool isSFXMuted = false; // SFX 음소거 상태

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 오브젝트를 씬 전환 시 삭제하지 않음
        }
        else
        {
            Destroy(gameObject);
        }
    }

     // BGM 음소거 ON/OFF
    public void ToggleBGMMute()
    {
        isBGMMuted = !isBGMMuted;
        bgmAudioSource.mute = isBGMMuted;
    }

    // SFX 음소거 ON/OFF
    public void ToggleSFXMute()
    {
        isSFXMuted = !isSFXMuted;
        sfxAudioSource.mute = isSFXMuted;
    }

    // BGM 음소거 상태 반환
    public bool IsBGMMuted()
    {
        return isBGMMuted;
    }

    // SFX 음소거 상태 반환
    public bool IsSFXMuted()
    {
        return isSFXMuted;
    }

    public void SetBGMVolume(float volume)
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = volume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxAudioSource != null)
        {
            sfxAudioSource.volume = volume;
        }
    }
}