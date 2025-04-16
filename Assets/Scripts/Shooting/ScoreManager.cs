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
    private int BombScore = 0;

    [SerializeField]
    private TextMeshProUGUI HealthText;
    [SerializeField]
    private TextMeshProUGUI BombText;

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
        UpdateBomb();
    }

    public void Health()
    {
        if (score == 900 || score == 1200 || score == 1500 || score == 1800)
        {
            Player.Instance.RecoverHealth();
        }
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
    public void addBomb(int amount)
    {
        if (BombScore < 9)
        {
            BombScore += amount;
            UpdateBomb();
        }
        else if (BombScore >= 9)
        {
            AddScore(10);
        }
    }
    public void fixBomb(int Bomb)
    {
        BombScore = Bomb;
        UpdateBomb();
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
    private void UpdateBomb()
    {
        if (BombText != null)
        {
            BombText.text = "x" + BombScore;
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
