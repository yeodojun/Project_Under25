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
    // 기존 패턴들은 이동 벡터 등으로 정의되어 있음. (N_8 ~ N_13는 별도 코루틴에서 처리)
    private Dictionary<string, Vector2[]> patterns = new Dictionary<string, Vector2[]> {
        { "N_0", new Vector2[] { new Vector2(0, -1) } }, // 아래로 이동
        { "N_1", new Vector2[] { new Vector2(0, 1) } },  // 위로 이동
        { "N_2", new Vector2[] { new Vector2(1, 0) } },  // 오른쪽 이동
        { "N_3", new Vector2[] { new Vector2(-1, 0) } }, // 왼쪽 이동
        { "N_4", new Vector2[] { new Vector2(1, -1) } }, // 대각선 오른쪽 아래
        { "N_5", new Vector2[] { new Vector2(-1, -1) } }, // 대각선 왼쪽 아래
        { "N_6", new Vector2[] { new Vector2(1, 1) } }, // 대각선 오른쪽 위
        { "N_7", new Vector2[] { new Vector2(-1, 1) } }, // 대각선 왼쪽 위
        { "N_8", null }, // 시계 방향 원 이동
        { "N_9", null }, // 반시계 방향 원 이동
        { "N_10", null }, // 물결 좌→우 이동
        { "N_11", null }, // 물결 우→좌 이동
        { "N_12", null }, // 물결 위→아래 이동
        { "N_13", null }  // 물결 아래→위 이동
        // "N_14"는 아래 TeleportPattern(), ExecutePattern()에서 별도로 처리
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

    // patternSequence 배열에 있는 패턴들을 순차적으로 실행합니다.
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

            if (pattern == "N_8" || pattern == "N_9")
            {
                bool clockwise = pattern == "N_8";
                yield return StartCoroutine(MoveInCircle(enemy, clockwise, 2f, 72f));
            }
            else if (pattern == "N_10" || pattern == "N_11" || pattern == "N_12" || pattern == "N_13")
            {
                yield return StartCoroutine(MoveInWave(enemy, pattern));
            }
            else if (pattern == "N_14")
            {
                // 새로운 순간 이동 패턴
                yield return StartCoroutine(TeleportPattern(enemy));
            }
            else // 기본 패턴 처리 (예: "N_0", "N_1", "N_2", "N_3", 등)
            {
                Vector2[] movementSteps = this.patterns[pattern];
                if (movementSteps == null)
                {
                    Debug.LogError($"패턴 {pattern}에 대한 이동 단계가 정의되지 않았습니다.");
                    continue;
                }

                foreach (Vector2 step in movementSteps)
                {
                    if (enemy == null) yield break;

                    // 만약 적이 flipped 상태이고, 패턴이 "N_1" (원래 위로 이동)라면 이동 벡터의 방향을 반전합니다.
                    Vector2 appliedStep = step;
                    Enemy enemyComponent = enemy.GetComponent<Enemy>();
                    if (enemyComponent != null && enemyComponent.isFlipped && pattern == "N_1" && !enemyComponent.isEnemy1)
                    {
                        appliedStep = -step;
                    }

                    Vector3 targetPosition = enemy.transform.position + new Vector3(appliedStep.x, appliedStep.y, 0);
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

        float waveSpeed = 2f;      // 이동 속도
        float waveAmplitude = 1f;  // 파동의 높이
        float waveFrequency = 2f;  // 파동의 주기

        float directionX = 1f;
        float directionY = 1f;

        // 패턴에 따른 초기 이동 방향 설정
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

            if (pattern == "N_10" || pattern == "N_11")
            {
                newX += waveSpeed * directionX * Time.deltaTime;
                newY = initialY + waveAmplitude * Mathf.Sin(waveFrequency * newX);
            }
            else if (pattern == "N_12" || pattern == "N_13")
            {
                newY += waveSpeed * directionY * Time.deltaTime;
                newX = initialX + waveAmplitude * Mathf.Sin(waveFrequency * newY);
            }

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

        float destroyBoundaryX = 3.2f;
        float destroyBoundaryY = 6.2f;

        if (enemy.transform.position.y > destroyBoundaryY || enemy.transform.position.y < -destroyBoundaryY ||
            enemy.transform.position.x < -destroyBoundaryX || enemy.transform.position.x > destroyBoundaryX)
        {
            Debug.Log($"적 {enemy.name} 삭제됨 - 현재 위치: {enemy.transform.position}");
            Destroy(enemy);
        }
    }

    public bool HasHitScreenBoundary(Vector3 position, Vector2 direction)
    {
        return (position.x <= screenLeft && direction.x < 0) ||
               (position.x >= screenRight && direction.x > 0) ||
               (position.y >= screenTop && direction.y > 0) ||
               (position.y <= screenBottom && direction.y < 0);
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

    // 새로운 패턴 N_14: 순간 이동 패턴
    // 3초마다 지정된 좌표 범위 내에서 랜덤하게 순간 이동
    // 플레이어 피격 범위와 적 간 겹침을 피하며, 유효한 좌표가 없으면 최대 10회 시도 후 마지막 후보 사용
    private IEnumerator TeleportPattern(GameObject enemy)
    {
        float minX = -2f, maxX = 2f;
        float minY = -4.6f, maxY = 4.5f;
        float playerSafeRadius = 1.0f;  // 플레이어로부터 최소 안전 거리
        float overlapRadius = 0.5f;     // 다른 적과 겹치지 않을 최소 거리

        while (enemy != null)
        {
            yield return new WaitForSeconds(3f); // 3초마다 순간 이동

            Vector3 candidate = Vector3.zero;
            bool validCandidate = false;
            int attempts = 0;
            while (!validCandidate && attempts < 10)
            {
                attempts++;
                float candidateX = Random.Range(minX, maxX);
                float candidateY = Random.Range(minY, maxY);
                candidate = new Vector3(candidateX, candidateY, enemy.transform.position.z);

                // 플레이어 피격 범위 체크
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    if (Vector3.Distance(candidate, player.transform.position) < playerSafeRadius)
                        continue;
                }

                // 적끼리 겹침 체크 (자기 자신 제외)
                Collider2D[] hits = Physics2D.OverlapCircleAll(candidate, overlapRadius);
                bool overlapFound = false;
                foreach (Collider2D col in hits)
                {
                    if (col.gameObject != enemy && col.CompareTag("Enemy"))
                    {
                        overlapFound = true;
                        break;
                    }
                }
                if (overlapFound)
                    continue;

                validCandidate = true;
            }

            // 후보 좌표가 범위 내에 있는지 체크 (이론상 항상 만족해야 함)
            if (candidate.x < minX || candidate.x > maxX || candidate.y < minY || candidate.y > maxY)
            {
                Destroy(enemy);
                yield break;
            }

            enemy.transform.position = candidate;
        }
    }
}
