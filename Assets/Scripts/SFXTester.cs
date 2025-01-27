using UnityEngine;

public class SFXTester : MonoBehaviour
{
    public AudioClip testClip;

    public void PlayTestSFX()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.sfxAudioSource != null)
        {
            AudioManager.Instance.sfxAudioSource.PlayOneShot(testClip);
        }
    }
}