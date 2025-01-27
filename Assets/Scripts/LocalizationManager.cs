using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    private Dictionary<string, Dictionary<string, string>> localizedTexts;
    private string currentLanguage = "Korean";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLocalizationData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadLocalizationData()
    {
        localizedTexts = new Dictionary<string, Dictionary<string, string>>();

        localizedTexts["Korean"] = new Dictionary<string, string>
        {
            { "Play", "게임 하기" },
            { "Settings", "설정" },
            { "BGM", "배경음" },
            { "SFX", "효과음" },
            { "Language", "언어" }
        };

        localizedTexts["English"] = new Dictionary<string, string>
        {
            { "Play", "Play" },
            { "Settings", "Settings" },
            { "BGM", "BGM" },
            { "SFX", "SFX" },
            { "Language", "Language" }
        };
    }

    public void SetLanguage(string language)
    {
        if (localizedTexts.ContainsKey(language))
        {
            currentLanguage = language;
            UpdateAllTexts();
        }
        else
        {
            Debug.LogError($"Language not found: {language}");
        }
    }

    public string GetLocalizedText(string key)
    {
        if (localizedTexts.ContainsKey(currentLanguage) && localizedTexts[currentLanguage].ContainsKey(key))
        {
            return localizedTexts[currentLanguage][key];
        }
        return $"Missing[{key}]";
    }

    private void UpdateAllTexts()
    {
        LocalizedText[] allLocalizedTexts = FindObjectsOfType<LocalizedText>(true); // 비활성화된 오브젝트 포함
        Debug.Log($"LocalizedText found: {allLocalizedTexts.Length}");


        foreach (LocalizedText localizedText in FindObjectsOfType<LocalizedText>(true))
        {
            localizedText.UpdateText();
        }
    }
}