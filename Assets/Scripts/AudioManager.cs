using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // 싱글톤 인스턴스

    public AudioSource bgmAudioSource;   // 배경음악 AudioSource
    public AudioSource sfxAudioSource;   // 효과음 AudioSource

    // 4개의 배경음악 클립을 Inspector에서 할당
    [Header("Background Music Clips")]
    public AudioClip bgm1; // StartScene, MainScene, GameSelectScene, CharacterSelectScene용
    public AudioClip bgm2; // ShootingScene (웨이브 1~24)
    public AudioClip bgm3; // ShootingScene (웨이브 25 이상)
    public AudioClip bgm4; // 창 던지기 씬에서 사용할 배경음악

    private bool isBGMMuted = false; // BGM 음소거 상태
    private bool isSFXMuted = false; // SFX 음소거 상태

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 삭제하지 않음
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때마다 호출되는 콜백
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 이름에 따라 BGM 결정
        if (scene.name == "StartScene" ||
            scene.name == "MainScene" ||
            scene.name == "GameSelectScene" ||
            scene.name == "CharacterSelectScene")
        {
            ChangeBGM(bgm1);
        }
        else if (scene.name == "ShootingScene") // 슈팅씬 웨이브 1~24까지 BGM2
        {
            ChangeBGM(bgm2);
        }
        else if (scene.name == "OtherScene") // 나중에 창 던지기 브금
        {
            ChangeBGM(bgm4);
        }
        else
        {
            // 기본적으로 bgm1로 설정
            ChangeBGM(bgm1);
        }
    }

    // 웨이브 번호에 따라 ShootingScene의 BGM을 변경하는 메서드 (웨이브 관리 스크립트에서 호출)
    public void UpdateWave(int currentWave)
    {
        // 현재 씬이 ShootingScene인 경우에만 적용
        if (SceneManager.GetActiveScene().name == "ShootingScene")
        {
            if (currentWave >= 25)
            {
                ChangeBGM(bgm3);
            }
            else
            {
                ChangeBGM(bgm2);
            }
        }
    }

    // 배경음악을 변경하는 메서드
    public void ChangeBGM(AudioClip newClip)
    {
        if (bgmAudioSource.clip == newClip)
            return;

        bgmAudioSource.Stop();
        bgmAudioSource.clip = newClip;
        bgmAudioSource.Play();
    }

    // BGM 음소거 토글
    public void ToggleBGMMute()
    {
        isBGMMuted = !isBGMMuted;
        bgmAudioSource.mute = isBGMMuted;
    }

    // SFX 음소거 토글
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

    // BGM 볼륨 설정
    public void SetBGMVolume(float volume)
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = volume;
        }
    }

    // SFX 볼륨 설정
    public void SetSFXVolume(float volume)
    {
        if (sfxAudioSource != null)
        {
            sfxAudioSource.volume = volume;
        }
    }
}
