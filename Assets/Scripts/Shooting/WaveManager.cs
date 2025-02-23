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
        { 1, new Vector3[] { new Vector3(-1, 3.5f, 0)} }, // 첫 번째 웨이브: 
        { 2, new Vector3[] { new Vector3(-1, 4f, 0), new Vector3(0, 4f, 0), new Vector3(1, 5.5f, 0) } }, // 두 번째 웨이브: 세 곳에서 스폰
        { 3, new Vector3[] { new Vector3(-1, 5.5f, 0), new Vector3(0, 5.5f, 0), new Vector3(1, 5.5f, 0) } }, // 세 번째 웨이브: 중앙 포함
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
                        yield return StartCoroutine(SpawnPatternSequence(new string[] { "N_10" }, 10));

                        break;
                    case 2:
                        yield return StartCoroutine(SpawnPatternSequence(new string[] { "N_0", "N_9" }, 10));
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
        if (waveSpawnPositions.ContainsKey(wave))
        {
            Vector3[] positions = waveSpawnPositions[wave];
            Vector3 selectedPosition = positions[Random.Range(0, positions.Length)];

            Debug.Log($"Wave {wave} - 적 스폰 위치: {selectedPosition}"); // 스폰 위치 확인용 로그
            return selectedPosition;
        }
        else
        {
            Vector3 randomPosition = new Vector3(Random.Range(-2f, 2f), 5.5f, 0);
            Debug.Log($"Wave {wave} - 기본 랜덤 스폰 위치: {randomPosition}");
            return randomPosition; // 기본 스폰 위치 (랜덤)

        }

    }

    IEnumerator ExecutePattern(GameObject enemy, string[] patterns)
    {
        if (enemy == null)
        {
            Debug.LogError("ExecutePattern() 실행 중 enemy가 null입니다!");
            yield break;
        }

        Debug.Log($"적 {enemy.name} - 패턴 실행 전 위치: {enemy.transform.position}");

        // 생성 직후 즉시 이동하는 문제 방지 (0.1초 대기)
        yield return new WaitForSeconds(0.1f);

        foreach (string pattern in patterns)
        {
            if (enemy == null) yield break;

            Debug.Log($"적 {enemy.name} - 패턴 실행 시작 위치: {enemy.transform.position}");

            yield return StartCoroutine(PatternManager.Instance.ExecutePattern(enemy, new string[] { pattern }));
        }
    }



}
