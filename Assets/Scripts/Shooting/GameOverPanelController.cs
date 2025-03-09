using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverPanelController : MonoBehaviour
{

    public static GameOverPanelController Instance; // 싱글턴 적용
    public GameObject gameOverPanel; // 패널
    [SerializeField]
    public TextMeshProUGUI scoreText; // 현재 점수 표시
    [SerializeField]
    public TextMeshProUGUI highScoreText; // 최고 점수 표시

    private void Start()
    {
        if (gameOverPanel == null)
        {
            Debug.LogError("GameOverPanel이 연결되지 않았습니다!");
            return;
        }

        // 게임 오버가 아닐 경우에만 비활성화
        if (!gameOverPanel.activeSelf)
        {
            gameOverPanel.SetActive(false);
            Debug.Log("GameOverPanel이 비활성화됨 (Start에서 실행)");
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 유지됨
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
            return;
        }
    }

    public void ShowGameOverPanel()
    {
        Debug.Log("ShowGameOverPanel() 호출됨");

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.CheckAndUpdateHighScore(); // 최고 점수 갱신
            int currentScore = ScoreManager.Instance.GetScore();
            int highScore = ScoreManager.Instance.GetHighScore();

            scoreText.text = $"{currentScore}";
            highScoreText.text = $"{highScore}";

            Debug.Log($"현재 점수: {currentScore}, 최고 점수: {highScore}");
        }
        else
        {
            Debug.LogError("ScoreManager.Instance가 null입니다!");
        }

        gameOverPanel.SetActive(true);
        Debug.Log("GameOverPanel 활성화됨");
    }


    public void RestartGame()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore(); // 점수 초기화
        }
        SceneManager.LoadScene("ShootingScene");
    }

    public void GoToMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void GoToGameMainScene()
    {
        SceneManager.LoadScene("GameSelectScene");
    }
}
