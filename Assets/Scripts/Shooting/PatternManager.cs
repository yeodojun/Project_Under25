using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternManager : MonoBehaviour
{
    public static PatternManager Instance;

    private float screenTop = 6f;
    private float screenBottom = -6f;
    private float screenLeft = -3f;
    private float screenRight = 3f;
    private Dictionary<string, Vector2[]> patterns = new Dictionary<string, Vector2[]>
    {
        { "N_0", new Vector2[] { new Vector2(0, -1) } }, // 아래로 이동
        { "N_1", new Vector2[] { new Vector2(0, 1) } },  // 위로 이동
        { "N_2", new Vector2[] { new Vector2(1, 0) } },  // 오른쪽 이동
        { "N_3", new Vector2[] { new Vector2(-1, 0) } }, // 왼쪽 이동
        { "N_4", new Vector2[] { new Vector2(1, -1) } }, // 대각선 오른쪽 아래
        { "N_5", new Vector2[] { new Vector2(-1, -1) } }, // 대각선 왼쪽 아래
        { "N_6", new Vector2[] { new Vector2(1, 1) } }, // 대각선 오른쪽 위
        { "N_7", new Vector2[] { new Vector2(-1, 1) } }, // 대각선 왼쪽 위
        { "N_8", null}, // 시계 방향 원 이동
        { "N_9", null}, // 반시계 방향 원 이동
        { "N_10", null }, // 물결 좌→우 이동
        { "N_11", null }, // 물결 우→좌 이동
        { "N_12", null }, // 물결 위→아래 이동
        { "N_13", null }  // 물결 아래→위 이동
    };

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

    public Vector2[] GetPattern(string patternName)
    {
        if (patterns.ContainsKey(patternName))
        {
            Debug.Log($"패턴 {patternName} 로드 성공!");
            return patterns[patternName];
        }
        else
        {
            Debug.LogError($"패턴 {patternName}을 찾을 수 없습니다.");
            return null;
        }
    }

    // ExecutePattern은 전달된 문자열 배열의 순서대로 패턴을 실행합니다.
    // 예를 들어 "N_0"가 6번 들어있다면, 적은 "N_0"을 6회 수행합니다.
    public IEnumerator ExecutePattern(GameObject enemy, string[] patternSequence)
    {
        if (enemy == null)
        {
            Debug.LogError("ExecutePattern() 실행 중 enemy가 null입니다!");
            yield break;
        }

        Debug.Log($"적 {enemy.name} - 패턴 시작: {string.Join(", ", patternSequence)}");

        foreach (string pattern in patternSequence)
        {
            if (enemy == null) yield break;

            if (!this.patterns.ContainsKey(pattern))
            {
                Debug.LogError($"패턴 {pattern}을 찾을 수 없습니다!");
                continue;
            }

            // 원형 이동 패턴 처리
            if (pattern == "N_8" || pattern == "N_9")
            {
                bool clockwise = pattern == "N_8";
                yield return StartCoroutine(MoveInCircle(enemy, clockwise, 2f, 72f));
            }
            // 물결 이동 패턴 처리
            else if (pattern == "N_10" || pattern == "N_11" || pattern == "N_12" || pattern == "N_13")
            {
                yield return StartCoroutine(MoveInWave(enemy, pattern));
            }
            else // 일반 패턴, 예: "N_0"
            {
                Vector2[] movementSteps = this.patterns[pattern];

                foreach (Vector2 step in movementSteps)
                {
                    if (enemy == null) yield break;

                    Vector3 targetPosition = enemy.transform.position + new Vector3(step.x, step.y, 0);
                    float moveTime = 0.5f;
                    float elapsedTime = 0;
                    Vector3 startPosition = enemy.transform.position;

                    while (elapsedTime < moveTime)
                    {
                        if (enemy == null) yield break;

                        enemy.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveTime);
                        elapsedTime += Time.deltaTime;

                        DestroyIfOutOfScreen(enemy);
                        yield return null;
                    }

                    if (enemy == null) yield break;
                    enemy.transform.position = targetPosition;
                    DestroyIfOutOfScreen(enemy);
                }
            }
        }

        Debug.Log($"적 {enemy.name} - 모든 패턴 완료 후 다음 행동 대기");
    }

    private IEnumerator MoveInCircle(GameObject enemy, bool clockwise, float radius, float rotationSpeed)
    {
        if (enemy == null) yield break;

        float angle = 0f;
        float fullRotation = 360f;

        while (enemy != null && Mathf.Abs(angle) < fullRotation)
        {
            angle += (clockwise ? rotationSpeed : -rotationSpeed) * Time.deltaTime;

            if (enemy == null) yield break;

            float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);

            enemy.transform.position += new Vector3(x, y, 0) * Time.deltaTime;

            yield return null;
        }

        if (enemy != null)
        {
            Debug.Log($"적 {enemy.name} 한 바퀴 회전 완료 후 다음 패턴 실행");
        }
    }

    private IEnumerator MoveInWave(GameObject enemy, string pattern)
    {
        if (enemy == null) yield break;

        float waveSpeed = 2f; // 이동 속도
        float waveAmplitude = 1f; // 파동의 높이
        float waveFrequency = 2f; // 파동의 주기

        float directionX = 1f;
        float directionY = 1f;

        // 패턴별 초기 이동 방향 설정
        if (pattern == "N_10") { directionX = 1f; directionY = 0f; }
        if (pattern == "N_11") { directionX = -1f; directionY = 0f; }
        if (pattern == "N_12") { directionX = 0f; directionY = -1f; }
        if (pattern == "N_13") { directionX = 0f; directionY = 1f; }

        float initialX = enemy.transform.position.x;
        float initialY = enemy.transform.position.y;
        float time = 0f;

        while (enemy != null)
        {
            time += Time.deltaTime;

            float newX = enemy.transform.position.x;
            float newY = enemy.transform.position.y;

            if (pattern == "N_10" || pattern == "N_11") // 좌우 물결 이동
            {
                newX += waveSpeed * directionX * Time.deltaTime;
                newY = initialY + waveAmplitude * Mathf.Sin(waveFrequency * newX);
            }
            else if (pattern == "N_12" || pattern == "N_13") // 상하 물결 이동
            {
                newY += waveSpeed * directionY * Time.deltaTime;
                newX = initialX + waveAmplitude * Mathf.Sin(waveFrequency * newY);
            }

            // 경계 충돌 처리
            if (newY >= 5.5f || newY <= -5.5f)
            {
                waveAmplitude = -waveAmplitude;
            }

            if (newX >= 2.3f)
            {
                directionX = -Mathf.Abs(directionX);
            }
            if (newX <= -2.3f)
            {
                directionX = Mathf.Abs(directionX);
            }

            if (newY >= 5.5f)
            {
                directionY = -Mathf.Abs(directionY);
            }
            if (newY <= -5.5f)
            {
                directionY = Mathf.Abs(directionY);
            }

            enemy.transform.position = new Vector3(newX, newY, 0);
            DestroyIfOutOfScreen(enemy);
            yield return null;
        }
    }

    public void DestroyIfOutOfScreen(GameObject enemy)
    {
        if (enemy == null) return;

        float destroyBoundaryX = 3.2f; // X 축 화면 밖 기준
        float destroyBoundaryY = 6.2f; // Y 축 화면 밖 기준

        if (enemy.transform.position.y > destroyBoundaryY || enemy.transform.position.y < -destroyBoundaryY ||
            enemy.transform.position.x < -destroyBoundaryX || enemy.transform.position.x > destroyBoundaryX)
        {
            Debug.Log($"적 {enemy.name} 삭제됨 - 현재 위치: {enemy.transform.position}");
            Destroy(enemy);
        }
    }

    public bool HasHitScreenBoundary(Vector3 position, Vector2 direction)
    {
        return (position.x <= screenLeft && direction.x < 0) || (position.x >= screenRight && direction.x > 0) ||
               (position.y >= screenTop && direction.y > 0) || (position.y <= screenBottom && direction.y < 0);
    }

    private IEnumerator ApplyPattern(GameObject enemy, string pattern)
    {
        if (enemy == null) yield break;

        Debug.Log($"적 {enemy.name} - {pattern} 패턴 적용 중");

        switch (pattern)
        {
            case "N_0":
                while (enemy != null)
                {
                    enemy.transform.position += Vector3.down * 2f * Time.deltaTime;
                    DestroyIfOutOfScreen(enemy);
                    yield return null;
                }
                break;
        }
    }
}
