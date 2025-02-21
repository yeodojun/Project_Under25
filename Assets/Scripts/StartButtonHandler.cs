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

            Debug.Log($"Start 버튼 클릭됨 | 선택된 캐릭터: {selectedCharacter}, 선택된 게임: {selectedGame}");

            if (string.IsNullOrEmpty(selectedCharacter))
            {
                Debug.LogWarning("!!!! 캐릭터를 선택해야 합니다 !!!!");
                return;
            }

            if (selectedGame == 1)
            {
                Debug.Log($"{selectedCharacter} 선택됨! Game1Scene으로 이동!");
                SceneManager.LoadScene("ShootingScene");
            }
            else if (selectedGame == 2)
            {
                Debug.Log($"{selectedCharacter} 선택됨! Game2Scene으로 이동!");
                SceneManager.LoadScene("Game2Scene");
            }
            else
            {
                Debug.LogError("!! 선택된 게임이 없습니다 !! GameMainScene에서 게임을 선택하세요.");
                SceneManager.LoadScene("GameMainScene"); // 선택된 게임이 없으면 다시 선택하러 감
            }
        }
        else
        {
            Debug.LogError("GameManager가 존재하지 않습니다! 씬 설정을 확인하세요.");
        }
    }
}