using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string key; // 텍스트 키
    private Text textComponent;

    private void Start()
    {
        textComponent = GetComponent<Text>();
        UpdateText();
    }

    public void UpdateText()
    {
        if (LocalizationManager.Instance != null && textComponent != null)
        {
            textComponent.text = LocalizationManager.Instance.GetLocalizedText(key);
        }
    }
}