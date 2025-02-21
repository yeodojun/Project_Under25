using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab; // 적 프리팹
    public float waveDelay = 5f; // 웨이브 간 딜레이
    private int currentWave = 1; // 현재 웨이브
    private bool waveInProgress = false; // 웨이브 진행 여부

    private Dictionary<int, Vector3[]> waveSpawnPositions = new Dictionary<int, Vector3[]> // 웨이브별 스폰 위치 설정
    {
        { 1, new Vector3[] { new Vector3(-2, 5.5f, 0), new Vector3(2, 5.5f, 0) } }, // 첫 번째 웨이브: 좌우 두 곳에서 스폰
        { 2, new Vector3[] { new Vector3(-1, 4f, 0), new Vector3(0, 4f, 0), new Vector3(1, 5.5f, 0) } }, // 두 번째 웨이브: 세 곳에서 스폰
        { 3, new Vector3[] { new Vector3(-1, 5.5f, 0), new Vector3(0, 6f, 0), new Vector3(1, 5.5f, 0) } }, // 세 번째 웨이브: 중앙 포함
        { 4, new Vector3[] { new Vector3(-1, 5f, 0), new Vector3(1, 5f, 0) } }, // 네 번째 웨이브: 좌우
        { 5, new Vector3[] { new Vector3(0, -6f, 0) } } // 다섯 번째 웨이브: 중앙에서만 스폰
    };

    void Start()
    {
        StartCoroutine(ManageWaves());
    }

    IEnumerator ManageWaves()
    {
        while (true)
        {
            if (!waveInProgress)
            {
                waveInProgress = true;
                Debug.Log($"Starting Wave {currentWave}");

                switch (currentWave)
                {
                    case 1:
                        yield return StartCoroutine(SpawnPatternSequence(new string[] { "N_0" }, 10));
                        break;
                    case 2:
                        yield return StartCoroutine(SpawnPatternSequence(new string[] { "N_0", "N_2", "N_3" }, 10));
                        break;
                    case 3:
                        yield return StartCoroutine(SpawnPatternSequence(new string[] { "N_0", "N_2", "N_0", "N_3" }, 10));
                        break;
                    case 4:
                        yield return StartCoroutine(SpawnPatternSequence(new string[] { "N_4", "N_5" }, 10));
                        break;
                    case 5:
                        yield return StartCoroutine(SpawnPatternSequence(new string[] { "N_6", "N_7" }, 10));
                        break;
                }

                Debug.Log($"Wave {currentWave} complete. Waiting {waveDelay} seconds before next wave...");

                yield return new WaitForSeconds(waveDelay);

                currentWave++;
                waveInProgress = false;

                if (currentWave > 5)
                {
                    Debug.Log("All waves completed!");
                    break;
                }
            }

            yield return null;
        }
    }

    IEnumerator SpawnPatternSequence(string[] patterns, int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = GetSpawnPosition(currentWave);
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            if (enemy == null)
            {
                Debug.LogError("적 생성 실패! enemyPrefab이 올바르게 설정되어 있는지 확인하세요.");
                continue;
            }

            Debug.Log($"적 스폰됨: {enemy.name}, 패턴: {string.Join(", ", patterns)}, 위치: {spawnPosition}");

            StartCoroutine(ExecutePattern(enemy, patterns));

            yield return new WaitForSeconds(0.7f);
        }

        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return null;
        }
    }

    Vector3 GetSpawnPosition(int wave)
    {
        if (wave == 1) {
            return new Vector3(Random.Range(-2f, 2f), 5.5f, 0);
        }
        if (waveSpawnPositions.ContainsKey(wave))
        {
            Vector3[] positions = waveSpawnPositions[wave];
            return positions[Random.Range(0, positions.Length)]; // 해당 웨이브의 위치 중 랜덤 선택
        }
        else
        {
            return new Vector3(Random.Range(-2f, 2f), 5.5f, 0); // 기본 스폰 위치 (랜덤)
        }
    }

    IEnumerator ExecutePattern(GameObject enemy, string[] patterns)
    {
        if (enemy == null)
        {
            Debug.LogError("ExecutePattern() 실행 중 enemy가 null입니다!");
            yield break;
        }

        if (PatternManager.Instance == null)
        {
            Debug.LogError("PatternManager.Instance가 null입니다! PatternManager가 씬에 있는지 확인하세요.");
            yield break;
        }

        Debug.Log($"적 이동 패턴 시작: {enemy.name}");

        while (enemy != null)
        {
            foreach (string pattern in patterns)
            {
                Vector2[] movementSteps = PatternManager.Instance.GetPattern(pattern);
                if (movementSteps == null)
                {
                    Debug.LogError($"패턴 {pattern}을 찾을 수 없습니다!");
                    continue;
                }

                Debug.Log($"패턴 {pattern} 실행: {movementSteps.Length} 스텝");

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
                        yield return null;
                    }

                    enemy.transform.position = targetPosition;

                    // 화면 밖으로 나가면 적 제거
                    if (IsOutOfScreen(enemy.transform.position, pattern))
                    {
                        Debug.Log($"적 {enemy.name}이 화면 밖으로 나가 제거됨");
                        Destroy(enemy);
                        yield break;
                    }

                    Debug.Log($"적 이동 완료: {enemy.name} → {targetPosition}");
                }
            }
        }
    }
    bool IsOutOfScreen(Vector3 position, string pattern)
    {
        float screenTop = 6f;     // 화면 위쪽 한계
        float screenBottom = -6f; // 화면 아래쪽 한계
        float screenLeft = -4f;   // 화면 왼쪽 한계
        float screenRight = 4f;   // 화면 오른쪽 한계

        switch (pattern)
        {
            case "N_0": // 아래로 내려가는 패턴 (Y 좌표 체크)
                if (position.y < screenBottom) return true;
                break;
            case "N_4": // 대각선 방향 (X 또는 Y 벗어날 때 제거)
                if (position.y < screenBottom || position.x < screenLeft || position.x > screenRight) return true;
                break;
            case "N_5":
                if (position.y < screenBottom || position.x < screenLeft || position.x > screenRight) return true;
                break;
            case "N_6":
                if (position.y < screenBottom || position.x < screenLeft || position.x > screenRight) return true;
                break;
            case "N_7":
                if (position.y < screenBottom || position.x < screenLeft || position.x > screenRight) return true;
                break;
        }

        return false;
    }


}
