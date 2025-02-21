using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionController : MonoBehaviour
{
    public static CharacterSelectionController Instance; // 싱글톤

    public GameObject characterExplainPanel; // 설명 패널
    public Image characterImage; // 패널의 캐릭터 이미지
    public TextMeshProUGUI characterText; // 패널의 설명 텍스트

    private GameObject selectedCharacter = null; // 현재 선택된 캐릭터
    private GameObject selectedGlowEffect = null; // 현재 활성화된 GlowEffect

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectCharacter(GameObject character, GameObject glowEffect, Sprite characterSprite, string description)
    {
        // 이전 선택된 캐릭터의 GlowEffect 끄기
        if (selectedGlowEffect != null)
        {
            selectedGlowEffect.SetActive(false);
        }

        // 새로운 캐릭터 선택
        selectedCharacter = character;
        selectedGlowEffect = glowEffect;

        // GlowEffect 활성화
        selectedGlowEffect.SetActive(true);

        // 설명 패널 업데이트
        if (characterExplainPanel != null)
        {
            characterExplainPanel.SetActive(true); // 패널 활성화
            characterImage.sprite = characterSprite; // 캐릭터 이미지 변경
            characterText.text = description; // 텍스트 변경
        }

         // GameManager에 선택된 캐릭터 저장
        if (GameManager.Instance != null)
        {
            // UI `Image.sprite` 저장 (게임 씬에서 SpriteRenderer로 적용 예정)
            GameManager.Instance.SetSelectedCharacter(character.name, characterSprite);
            Debug.Log($"GameManager에 캐릭터 저장: {character.name} | 이미지: {characterSprite.name}");
        }
        else
        {
            Debug.LogError("GameManager가 존재하지 않습니다!");
        }
    }
}
