using UnityEngine;
using UnityEngine.UI;

public class CharacterClickHandler : MonoBehaviour
{
    public GameObject glowEffect; // 이 캐릭터의 GlowEffect
    public Image characterImage; // 캐릭터의 이미지
    public string characterDescription; // 설명 텍스트

    private void Start()
    {
        if (glowEffect != null)
        {
            glowEffect.SetActive(false); // 처음에는 GlowEffect 비활성화
        }
    }

    public void OnCharacterClick()
    {
        if (CharacterSelectionController.Instance != null)
        {
            CharacterSelectionController.Instance.SelectCharacter(gameObject, glowEffect, characterImage.sprite, characterDescription);
        }
    }
}
