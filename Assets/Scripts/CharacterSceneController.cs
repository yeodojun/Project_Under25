using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSceneController : MonoBehaviour
{
    public SpriteRenderer characterHead; // CharacterHead의 SpriteRenderer

    void Start()
    {
        if (GameManager.Instance != null)
        {
            Sprite selectedCharacterSprite = GameManager.Instance.GetSelectedCharacterImage();
            
            if (selectedCharacterSprite != null && characterHead != null)
            {
                characterHead.sprite = selectedCharacterSprite;
                Debug.Log($"Game1Scene에서 CharacterHead 스프라이트 변경 완료: {selectedCharacterSprite.name}");
            }
            else
            {
                Debug.LogWarning("Game1Scene에서 선택된 캐릭터 스프라이트가 없습니다!");
            }
        }
        else
        {
            Debug.LogError("GameManager 인스턴스가 존재하지 않습니다!");
        }
    }

    public void OnCharacterSelected()
    {
        int selectedGame = GameManager.Instance.GetSelectedGame(); // GameManager에서 선택된 게임 가져오기

        if (selectedGame == 1)
        {
            SceneManager.LoadScene("GameSelectScene"); // 선택된 게임 씬으로 이동
        }
        else if (selectedGame == 2)
        {
            SceneManager.LoadScene("Game2Scene"); // 선택된 게임 씬으로 이동
        }
        else
        {
            Debug.LogError("No game selected");
        }
    }

    
}
