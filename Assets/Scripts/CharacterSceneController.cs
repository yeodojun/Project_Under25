using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSceneController : MonoBehaviour
{
    public void OnCharacterSelected()
    {
        int selectedGame = GameManager.Instance.GetSelectedGame(); // GameManager에서 선택된 게임 가져오기

        if (selectedGame == 1)
        {
            SceneManager.LoadScene("Game1Scene"); // 선택된 게임 씬으로 이동
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
