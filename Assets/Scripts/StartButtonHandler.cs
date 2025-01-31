using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonHandler : MonoBehaviour
{
    public void OnStartButtonClick()
    {
        if (GameManager.Instance != null)
        {
            string selectedCharacter = GameManager.Instance.GetSelectedCharacter();
            int selectedGame = GameManager.Instance.GetSelectedGame();

            if (string.IsNullOrEmpty(selectedCharacter))
            {
                Debug.LogWarning("⚠ 캐릭터를 선택해야 합니다!");
                return;
            }

            if (selectedGame == 1)
            {
                Debug.Log($"🚀 {selectedCharacter} 선택됨! Game1Scene으로 이동!");
                SceneManager.LoadScene("Game1Scene");
            }
            else if (selectedGame == 2)
            {
                Debug.Log($"🚀 {selectedCharacter} 선택됨! Game2Scene으로 이동!");
                SceneManager.LoadScene("Game2Scene");
            }
            else
            {
                Debug.LogError("🚨 선택된 게임이 없습니다! GameMainScene에서 게임을 선택하세요.");
            }
        }
    }
}
