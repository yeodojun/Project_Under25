using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private int SelectedGame = -1; // 1: Game1, 2: Game2


    private string selectedCharacterName; // ✅ 선택된 캐릭터 저장

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"🚀 씬 로드됨: {scene.name}");
    }

    // 🎮 선택한 게임 설정
    public void SetSelectedGame(int gameIndex)
    {
        SelectedGame = gameIndex;
        Debug.Log($"🎮 Game {gameIndex} 선택됨!");  
    }

    // 선택한 게임 반환
    public int GetSelectedGame()
    {
        return SelectedGame;
    }

    public void SetSelectedCharacter(string characterName)
    {
        selectedCharacterName = characterName;
        Debug.Log($"🎭 선택된 캐릭터: {selectedCharacterName}");
    }

    public string GetSelectedCharacter()
    {
        return selectedCharacterName;
    }
}
