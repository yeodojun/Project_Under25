using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject SettingPanel;
    [SerializeField]
    private TextMeshProUGUI text1;

    public static GameManager Instance;

    public int SelectedGame { get; private set; } = -1; // 1: Game1, 2: Game2

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

    public void SetSelectedGame(int gameIndex)
    {
        SelectedGame = gameIndex;
    }

    public int GetSelectedGame()
    {
        return SelectedGame;
    }
    
    public void ShowSettingPanel() {
        SettingPanel.SetActive(true);
    }
    
    public void HideSettingPanel() {
        SettingPanel.SetActive(false);
    }
}
