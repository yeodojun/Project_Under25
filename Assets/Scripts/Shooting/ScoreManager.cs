using UnityEngine;
using TMPro; // TextMeshPro 사용

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // 싱글턴 패턴
    private int score = 0;

    [SerializeField]
    private TextMeshProUGUI scoreText; // UI 텍스트 (TextMeshPro 사용)

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
        UpdateScoreText();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "" + score;
        }
    }
}
