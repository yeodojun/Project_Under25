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
        { "N_8", null},
        { "N_9", null},
        { "N_10", null },
        { "N_11", null },
        { "N_12", null },
        { "N_13", null }
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

    public IEnumerator ExecutePattern(GameObject enemy, string[] patterns)
    {
        if (enemy == null) yield break;

        for (int i = 0; i < patterns.Length; i++)
        {
            if (enemy == null)
            {
                Debug.LogError("ExecutePattern() 실행 중 enemy가 null입니다!");
                yield break;
            }
            Debug.Log($"적 {enemy.name} - 패턴 실행 전 위치: {enemy.transform.position}");

            string pattern = patterns[i];

            if (!this.patterns.ContainsKey(pattern))
            {
                Debug.LogError($"패턴 {pattern}을 찾을 수 없습니다!");
                continue;
            }

            if (pattern == "N_8" || pattern == "N_9")
            {
                bool clockwise = pattern == "N_8";
                yield return StartCoroutine(MoveInCircle(enemy, clockwise, 2f, 72f));
            }
            else if (pattern == "N_10" || pattern == "N_11" || pattern == "N_12" || pattern == "N_13")
            {
                yield return StartCoroutine(MoveInWave(enemy, pattern));
            }
            else
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
    }

    private IEnumerator MoveInCircle(GameObject enemy, bool clockwise, float radius, float rotationSpeed)
    {
        if (enemy == null) yield break;

        float angle = 0f;
        float fullRotation = 360f;

        while (enemy != null && Mathf.Abs(angle) < fullRotation)
        {
            angle += (clockwise ? rotationSpeed : -rotationSpeed) * Time.deltaTime;

            if (enemy == null) yield break; // 적이 삭제되었는지 확인 후 종료

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

        float waveSpeed = 2f; // 이동 속도 (n속도)
        float waveAmplitude = 1f; // 파동의 높이 (a)
        float waveFrequency = 2f; // 파동의 주기 (b)

        float directionX = 1f; // 기본 x 이동 방향
        float directionY = 1f; // 기본 y 이동 방향

        // 패턴별 초기 이동 방향 설정
        if (pattern == "N_10") { directionX = 1f; directionY = 0f; } // 좌 → 우
        if (pattern == "N_11") { directionX = -1f; directionY = 0f; } // 우 → 좌
        if (pattern == "N_12") { directionX = 0f; directionY = -1f; } // 상 → 하
        if (pattern == "N_13") { directionX = 0f; directionY = 1f; } // 하 → 상

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

            // 예외 처리 (경계 충돌 감지)
            if (newY >= 5.5f || newY <= -5.5f) // 화면 상하 경계 충돌
            {
                waveAmplitude = -waveAmplitude; // 출렁이는 방향 반전
            }

            if (newX >= 2.3f) // 화면 오른쪽 경계 충돌
            {
                directionX = -Mathf.Abs(directionX); // 왼쪽으로 이동
            }
            if (newX <= -2.3f) // 화면 왼쪽 경계 충돌
            {
                directionX = Mathf.Abs(directionX); // 오른쪽으로 이동
            }

            if (newY >= 5.5f) // 화면 위쪽 경계 충돌
            {
                directionY = -Mathf.Abs(directionY); // 아래로 이동
            }
            if (newY <= -5.5f) // 화면 아래쪽 경계 충돌
            {
                directionY = Mathf.Abs(directionY); // 위로 이동
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
}
