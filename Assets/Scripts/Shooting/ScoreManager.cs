using UnityEngine;
using TMPro; // TextMeshPro 사용

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // 싱글턴 패턴
    private int score = 0;
    private int highScore = 0; // 최고 점수 저장

    [SerializeField]
    private TextMeshProUGUI scoreText; // UI 텍스트 (TextMeshPro 사용)

    private int health = 3;

    [SerializeField]
    private TextMeshProUGUI HealthText; 

    void Awake()
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

    void Start()
    {
        LoadHighScore();
        UpdateScoreText();
        UpdateHealth();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    public int GetScore()
    {
        return score;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    public void CheckAndUpdateHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            SaveHighScore();
        }
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }
    public void fixHealth(int hp)
    {
        health = hp;
        UpdateHealth();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "" + score;
        }
    }

    private void UpdateHealth()
    {
        if (HealthText != null)
        {
            HealthText.text = "x" + health;
        }
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 1);
    }
}
