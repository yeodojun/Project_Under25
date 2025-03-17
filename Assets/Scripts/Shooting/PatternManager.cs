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
    // 기존 패턴들은 이동 벡터 등으로 정의되어 있음. (P_8 ~ P_13는 별도 코루틴에서 처리)
    private Dictionary<string, Vector2[]> patterns = new Dictionary<string, Vector2[]> {
        { "P_0", new Vector2[] { new Vector2(0, -1) } }, // 아래로 이동
        { "P_1", new Vector2[] { new Vector2(0, 1) } },  // 위로 이동
        { "P_2", new Vector2[] { new Vector2(1, 0) } },  // 오른쪽 이동
        { "P_3", new Vector2[] { new Vector2(-1, 0) } }, // 왼쪽 이동
        { "P_4", new Vector2[] { new Vector2(1, -0.5f) } }, // 대각선 오른쪽 아래 30도
        { "P_5", new Vector2[] { new Vector2(-1, -0.5f) } }, // 대각선 왼쪽 아래 30도
        { "P_6", new Vector2[] { new Vector2(1, 0.5f) } }, // 대각선 오른쪽 위 30도
        { "P_7", new Vector2[] { new Vector2(-1, 0.5f) } }, // 대각선 왼쪽 위 30도
        { "P_8", new Vector2[] { new Vector2(1, -1) } }, // 대각선 오른쪽 아래 45도
        { "P_9", new Vector2[] { new Vector2(-1, -1) } }, // 대각선 왼쪽 아래 45도
        { "P_10", new Vector2[] { new Vector2(1, 1) } }, // 대각선 오른쪽 위 45도
        { "P_11", new Vector2[] { new Vector2(-1, 1) } }, // 대각선 왼쪽 위 45도
        { "P_12", new Vector2[] { new Vector2(0.5f, -1) } }, // 대각선 오른쪽 아래 60도
        { "P_13", new Vector2[] { new Vector2(-0.5f, -1) } }, // 대각선 왼쪽 아래 60도
        { "P_14", new Vector2[] { new Vector2(0.5f, 1) } }, // 대각선 오른쪽 위 60도
        { "P_15", new Vector2[] { new Vector2(-0.5f, 1) } }, // 대각선 왼쪽 위 60도
        { "P_16", null }, // 시계 방향 원 이동
        { "P_17", null }, // 반시계 방향 원 이동
        { "P_18", null }, // 물결 좌→우 이동
        { "P_19", null }, // 물결 우→좌 이동
        { "P_20", null }, // 물결 위→아래 이동
        { "P_21", null }  // 물결 아래→위 이동
        // "P_22"는 아래 TeleportPattern(), ExecutePattern()에서 별도로 처리
        // "P_23"도 아래 ZigzagMovement(), " 별도로 처리
        // "P_23B"도 아래 별도로 처리 '보스 전용'
        // "P_24" 미구현
        // "P_25"도 아래 KamikazeMovement(), " 별도로 처리
        // "P_25R"도 아래 별도로 처리
        // "P_25B"도 아래 별도로 처리 '보스 전용'
        // "P_26"도 아래 별도로 처리
        // "P_27"도 아래 별도로 처리
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
        Boss boss = enemy.GetComponent<Boss>();
        if (enemy == null)
        {
            Debug.LogError("ExecutePattern() 실행 중 enemy가 null입니다!");
            yield break;
        }

        //Debug.Log($"적 {enemy.name} - 패턴 시작: {string.Join(", ", patternSequence)}");

        foreach (string pattern in patternSequence)
        {
            if (boss != null && boss.forceStopPattern)
            {
                yield break; // 즉시 종료
            }
            if (enemy == null) yield break;

            if (pattern == "P_16" || pattern == "P_17")
            {
                bool clockwise = pattern == "P_16";
                yield return StartCoroutine(MoveInCircle(enemy, clockwise, 2f, 72f));
            }
            else if (pattern == "P_18" || pattern == "P_19" || pattern == "P_20" || pattern == "P_21")
            {
                yield return StartCoroutine(MoveInWave(enemy, pattern));
            }
            else if (pattern == "P_22")
            {
                // 순간 이동 패턴
                yield return StartCoroutine(TeleportPattern(enemy));
            }
            else if (pattern == "P_23")
            {
                // 지그재그 이동 패턴
                yield return StartCoroutine(ZigzagMovement(enemy));
            }
            else if (pattern == "P_23B") // 새 지그제그 보스에서만 사용
            {
                yield return StartCoroutine(ZigzagMovementBoss(enemy));
            }
            else if (pattern == "P_25")  // 새 카미카제 패턴
            {
                yield return StartCoroutine(KamikazeMovement(enemy));
            }
            else if (pattern == "P_25R")
            {
                // 카미카제 복귀 패턴: enemy의 스폰 위치(현재 위치를 스폰 위치로 간주)를 기준으로 수행
                Vector3 spawnPos = enemy.transform.position;
                yield return StartCoroutine(KamikazeReturnMovement(enemy, spawnPos));
            }
            else if (pattern == "P_25R_boss")
            {
                // 보스 전용: 단 한번 수행하는 카미카제 복귀 패턴
                Vector3 spawnPos = enemy.transform.position;
                yield return StartCoroutine(KamikazeReturnMovementOnce(enemy, spawnPos));
            }
            else if (pattern == "P_26")
            {
                // 새로운 마름모 이동 패턴
                yield return StartCoroutine(DiamondMovement(enemy));
            }
            else if (pattern == "P_27")
            {
                yield return StartCoroutine(MirrorMovement(enemy));
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
                    if (enemyComponent != null && enemyComponent.isFlipped && pattern == "P_1" && !enemyComponent.isEnemy1)
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

        if (enemy != null)
        {
            Debug.Log($"적 {enemy.name} - 모든 패턴 완료 후 다음 행동 대기");
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
        Boss boss = enemy.GetComponent<Boss>();
        if (enemy == null) yield break;

        float waveSpeed = 2f;      // 이동 속도
        float waveAmplitude = 1f;  // 파동의 높이
        float waveFrequency = 2f;  // 파동의 주기

        float directionX = 1f;
        float directionY = 1f;

        // 패턴에 따른 초기 이동 방향 설정
        if (pattern == "P_18") { directionX = 1f; directionY = 0f; }
        if (pattern == "P_19") { directionX = -1f; directionY = 0f; }
        if (pattern == "P_20") { directionX = 0f; directionY = -1f; }
        if (pattern == "P_21") { directionX = 0f; directionY = 1f; }

        float initialX = enemy.transform.position.x;
        float initialY = enemy.transform.position.y;
        float time = 0f;

        while (enemy != null)
        {
            time += Time.deltaTime;

            float newX = enemy.transform.position.x;
            float newY = enemy.transform.position.y;

            if (boss != null && boss.forceStopPattern)
                yield break;
            if (pattern == "P_18" || pattern == "P_19")
            {
                newX += waveSpeed * directionX * Time.deltaTime;
                newY = initialY + waveAmplitude * Mathf.Sin(waveFrequency * newX);
            }
            else if (pattern == "P_20" || pattern == "P_21")
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

        float destroyBoundaryX = 10f;
        float destroyBoundaryY = 10f;

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
            case "P_0":
                while (enemy != null)
                {
                    enemy.transform.position += Vector3.down * 2f * Time.deltaTime;
                    DestroyIfOutOfScreen(enemy);
                    yield return null;
                }
                break;
        }
    }

    // 새로운 패턴 P_22: 순간 이동 패턴
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
                if (enemy == null) yield break; // 또는 return;
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

    // 새로운 패턴 P_23: 지그재그 이동
    // 1초마다 현재 위치에서 좌우 중 50% 확률로 1단위 이동.
    // 이동하려는 위치가 x:[-2,2], y:[-4.5,4.5] 범위를 벗어나거나, 
    // 이동하려는 위치에 이미 적 또는 플레이어가 있으면 이동하지 않습니다.
    private IEnumerator ZigzagMovement(GameObject enemy)
    {
        Boss boss = enemy.GetComponent<Boss>();
        float step = 1f; // 이동 거리
        float patternMinX = -2f, patternMaxX = 2f;
        float patternMinY = -4.5f, patternMaxY = 4.5f;

        while (enemy != null)
        {
            if (boss != null && boss.forceStopPattern)
                yield break;
            yield return new WaitForSeconds(1f);
            if (enemy == null) yield break;

            if (boss != null && boss.forceStopPattern)
                yield break;

            Vector3 currentPos = enemy.transform.position;
            int direction = Random.Range(0, 2) == 0 ? -1 : 1; // -1: 좌, +1: 우
            Vector3 targetPos = currentPos + new Vector3(direction * step, 0, 0);

            // 이동하려는 좌표가 지정된 범위를 벗어나면 이동하지 않음
            if (targetPos.x < patternMinX || targetPos.x > patternMaxX)
                continue;

            // 이동하려는 위치에 플레이어가 있으면 이동 X
            Collider2D playerCol = Physics2D.OverlapCircle(targetPos, 0.3f);
            if (playerCol != null && playerCol.CompareTag("Player"))
                continue;

            // 이동하려는 위치에 이미 다른 적이 있는지 체크 (자기 자신 제외)
            Collider2D[] cols = Physics2D.OverlapCircleAll(targetPos, 0.3f);
            bool occupied = false;
            foreach (Collider2D col in cols)
            {
                if (col.gameObject != enemy && col.CompareTag("Enemy"))
                {
                    occupied = true;
                    break;
                }
            }
            if (occupied)
                continue;

            // 유효한 이동이면 enemy를 targetPos로 이동
            enemy.transform.position = targetPos;

            // 혹시 이동 후 위치가 범위를 벗어난다면 적 파괴
            if (enemy.transform.position.x < patternMinX || enemy.transform.position.x > patternMaxX ||
                enemy.transform.position.y < patternMinY || enemy.transform.position.y > patternMaxY)
            {
                Destroy(enemy);
                yield break;
            }
        }
    }
    // 패턴 P_23B
    // 이 패턴은 보스에서만 사용
    private IEnumerator ZigzagMovementBoss(GameObject enemy)
    {
        Boss boss = enemy.GetComponent<Boss>();
        float step = 1f; // 이동 거리
        float patternMinX = -2f, patternMaxX = 2f;
        float patternMinY = -4.5f, patternMaxY = 4.5f;

        while (enemy != null)
        {
            // 보스 체력이 9000 이하이면 즉시 종료
            if (boss != null && boss.CurrentHP <= 9000f)
                yield break;
            if (boss != null && boss.forceStopPattern)
                yield break;

            yield return new WaitForSeconds(1f);

            if (enemy == null) yield break;
            if (boss != null && boss.CurrentHP <= 9000f)
                yield break;
            if (boss != null && boss.forceStopPattern)
                yield break;

            Vector3 currentPos = enemy.transform.position;
            int direction = Random.Range(0, 2) == 0 ? -1 : 1; // -1: 좌, +1: 우
            Vector3 targetPos = currentPos + new Vector3(direction * step, 0, 0);

            // 이동하려는 좌표가 지정된 범위를 벗어나면 패스
            if (targetPos.x < patternMinX || targetPos.x > patternMaxX)
                continue;

            // 이동하려는 위치에 플레이어가 있으면 패스
            Collider2D playerCol = Physics2D.OverlapCircle(targetPos, 0.3f);
            if (playerCol != null && playerCol.CompareTag("Player"))
                continue;

            // 이동하려는 위치에 이미 다른 적이 있는지 체크 (자기 자신 제외)
            Collider2D[] cols = Physics2D.OverlapCircleAll(targetPos, 0.3f);
            bool occupied = false;
            foreach (Collider2D col in cols)
            {
                if (col.gameObject != enemy && col.CompareTag("Enemy"))
                {
                    occupied = true;
                    break;
                }
            }
            if (occupied)
                continue;

            // 유효한 이동이면 enemy를 targetPos로 이동
            enemy.transform.position = targetPos;

            // 이동 후 위치가 범위를 벗어나면 파괴 후 종료
            if (enemy.transform.position.x < patternMinX || enemy.transform.position.x > patternMaxX ||
                enemy.transform.position.y < patternMinY || enemy.transform.position.y > patternMaxY)
            {
                Destroy(enemy);
                yield break;
            }
        }
    }


    // 새로운 패턴 P_25
    private IEnumerator KamikazeMovement(GameObject enemy)
    {
        // 허용 범위 (필요하면 사용)
        float minX = -2f, maxX = 2f;
        float minY = -4.5f, maxY = 4.5f;

        // 플레이어의 현재 위치를 목표로 설정
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 targetPos;
        if (player != null)
        {
            targetPos = player.transform.position;
        }
        else
        {
            // 만약 플레이어가 없다면, 현재 위치 유지
            targetPos = enemy.transform.position;
        }

        float speed = 5f;
        Vector3 startPos = enemy.transform.position;
        float distance = Vector3.Distance(startPos, targetPos);
        float moveTime = distance / speed;
        float elapsed = 0f;
        while (elapsed < moveTime && enemy != null)
        {
            enemy.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (enemy != null)
        {
            enemy.transform.position = targetPos;
            // 화면 범위 체크 (필요 시)
            if (targetPos.x < minX || targetPos.x > maxX || targetPos.y < minY || targetPos.y > maxY)
            {
                Destroy(enemy);
                yield break;
            }
        }
        yield return null;
    }
    // P_25R 5번
    private IEnumerator KamikazeReturnMovement(GameObject enemy, Vector3 spawnPos)
    {
        int cycles = 5;
        for (int i = 0; i < cycles; i++)
        {
            // 플레이어 위치를 목표로
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Vector3 target;
            if (player != null)
                target = player.transform.position;
            else
                target = spawnPos; // 플레이어가 없으면 스폰 위치로

            // 플레이어 좌표로 돌진 (1초 동안 이동)
            float moveTime = 1f;
            float elapsed = 0f;
            Vector3 startPos = enemy.transform.position;
            while (elapsed < moveTime && enemy != null)
            {
                enemy.transform.position = Vector3.Lerp(startPos, target, elapsed / moveTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
            if (enemy != null)
                enemy.transform.position = target;
            yield return new WaitForSeconds(1f);

            // spawn 위치로 복귀 (1초 동안 이동)
            moveTime = 1f;
            elapsed = 0f;
            if (enemy == null) yield break;
            startPos = enemy.transform.position;
            while (elapsed < moveTime && enemy != null)
            {
                enemy.transform.position = Vector3.Lerp(startPos, spawnPos, elapsed / moveTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
            if (enemy != null)
                enemy.transform.position = spawnPos;
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }
    // 카미카제 리턴 한번만 보스전용
    private IEnumerator KamikazeReturnMovementOnce(GameObject enemy, Vector3 spawnPos)
    {
        // 플레이어 위치로 돌진 (1초 동안 이동)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 target = (player != null) ? player.transform.position : spawnPos;
        float moveTime = 1f;
        float elapsed = 0f;
        Vector3 startPos = enemy.transform.position;
        while (elapsed < moveTime && enemy != null)
        {
            enemy.transform.position = Vector3.Lerp(startPos, target, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (enemy != null)
            enemy.transform.position = target;
        yield return new WaitForSeconds(1f);

        // spawn 위치로 복귀 (1초 동안 이동)
        moveTime = 1f;
        elapsed = 0f;
        if (enemy == null) yield break;
        startPos = enemy.transform.position;
        while (elapsed < moveTime && enemy != null)
        {
            enemy.transform.position = Vector3.Lerp(startPos, spawnPos, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (enemy != null)
            enemy.transform.position = spawnPos;
        yield return new WaitForSeconds(1f);
    }

    // 패턴 P_26 
    private IEnumerator DiamondMovement(GameObject enemy)
    {
        // 단계 각도 (도 단위)
        float[] angles = { 210f, 300f, 30f, 150f };
        int index = 0;
        while (enemy != null)
        {
            float angleDeg = angles[index];
            float angleRad = angleDeg * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;
            Vector3 currentPos = enemy.transform.position;
            float tMin = Mathf.Infinity;
            Vector3 targetPos = currentPos;

            // 사각형 경계: x in [-2, 2], y in [-4.5, 4.5]
            // 왼쪽 경계
            if (direction.x < 0)
            {
                float t = (-2 - currentPos.x) / direction.x;
                if (t > 0 && t < tMin)
                {
                    Vector3 candidate = currentPos + (Vector3)(direction * t);
                    if (candidate.y >= -4.5f && candidate.y <= 4.5f)
                    {
                        tMin = t;
                        targetPos = candidate;
                    }
                }
            }
            // 오른쪽 경계
            if (direction.x > 0)
            {
                float t = (2 - currentPos.x) / direction.x;
                if (t > 0 && t < tMin)
                {
                    Vector3 candidate = currentPos + (Vector3)(direction * t);
                    if (candidate.y >= -4.5f && candidate.y <= 4.5f)
                    {
                        tMin = t;
                        targetPos = candidate;
                    }
                }
            }
            // 하단 경계
            if (direction.y < 0)
            {
                float t = (-4.5f - currentPos.y) / direction.y;
                if (t > 0 && t < tMin)
                {
                    Vector3 candidate = currentPos + (Vector3)(direction * t);
                    if (candidate.x >= -2f && candidate.x <= 2f)
                    {
                        tMin = t;
                        targetPos = candidate;
                    }
                }
            }
            // 상단 경계
            if (direction.y > 0)
            {
                float t = (4.5f - currentPos.y) / direction.y;
                if (t > 0 && t < tMin)
                {
                    Vector3 candidate = currentPos + (Vector3)(direction * t);
                    if (candidate.x >= -2f && candidate.x <= 2f)
                    {
                        tMin = t;
                        targetPos = candidate;
                    }
                }
            }

            // 만약 유효한 이동 거리를 찾지 못하면 현재 위치 유지
            if (tMin == Mathf.Infinity)
                targetPos = currentPos;

            // 1초 동안 선형 이동
            float moveTime = 1f;
            float elapsed = 0f;
            Vector3 startPos = currentPos;
            while (elapsed < moveTime && enemy != null)
            {
                enemy.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
            if (enemy != null)
                enemy.transform.position = targetPos;

            // 만약 이동 후 위치가 경계 밖이면 파괴
            if (enemy.transform.position.x < -2f || enemy.transform.position.x > 2f ||
                enemy.transform.position.y < -4.5f || enemy.transform.position.y > 4.5f)
            {
                Destroy(enemy);
                yield break;
            }

            index = (index + 1) % angles.Length;
        }
    }
    // 패턴 P_27
    private IEnumerator MirrorMovement(GameObject enemy)
    {
        float minX = -2f, maxX = 2f;
        float minY = -4.5f, maxY = 4.5f;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float interval = 3f;       // 몇 초마다 이동할지
        float moveDuration = 0.3f; // 이동에 걸리는 시간

        while (enemy != null)
        {
            yield return new WaitForSeconds(interval);

            if (player != null && enemy != null)
            {
                float targetX = player.transform.position.x;
                Vector3 startPos = enemy.transform.position;
                Vector3 endPos = new Vector3(targetX, startPos.y, startPos.z);

                float elapsed = 0f;

                while (elapsed < moveDuration)
                {
                    if (enemy == null) yield break; // 안전 체크

                    float t = elapsed / moveDuration;
                    enemy.transform.position = Vector3.Lerp(startPos, endPos, t);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                // 보정
                if (enemy != null)
                    enemy.transform.position = endPos;

                // 화면 범위 체크
                if (endPos.x < minX || endPos.x > maxX || endPos.y < minY || endPos.y > maxY)
                {
                    Destroy(enemy);
                    yield break;
                }
            }
        }
    }

}
