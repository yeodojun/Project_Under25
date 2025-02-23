using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab; // 적 프리팹
    public float waveDelay = 5f; // 웨이브 간 딜레이
    private int currentWave = 1; // 현재 웨이브
    private bool waveInProgress = false; // 웨이브 진행 여부

    // 웨이브 데이터
    private Dictionary<int, List<(Vector3[], string[])>> waveData = new Dictionary<int, List<(Vector3[], string[])>>()
    {
        {
            1, new List<(Vector3[], string[])>
            {
                // 웨이브 1: 첫 번째 스폰 - 3개 좌표, 각 적이 "N_0" 패턴을 4회 실행
                (
                    new Vector3[] {
                        new Vector3(-2, 4.6f, 0),
                        new Vector3(0, 4.6f, 0),
                        new Vector3(2, 4.6f, 0)
                    },
                    new string[] {
                        "N_0", "N_0", "N_0", "N_0"
                    }
                ),
                // 웨이브 1: 두 번째 스폰 - 5개 좌표, 각 적이 "N_0" 패턴을 4회 실행
                (
                    new Vector3[] {
                        new Vector3(-2, 4.6f, 0),
                        new Vector3(-1, 4.6f, 0),
                        new Vector3(0, 4.6f, 0),
                        new Vector3(1, 4.6f, 0),
                        new Vector3(2, 4.6f, 0)
                    },
                    new string[] {
                        "N_0", "N_0", "N_0"
                    }
                )
            }
        },
        {
            2, new List<(Vector3[], string[])>
            {
                // 웨이브 2: 첫 번째 스폰: (-2,4.5), (-2,3.5), (-2,2.5)에서 각 적이 "N_2" 4회 실행
                (
                    new Vector3[] {
                        new Vector3(-2, 4.5f, 0),
                        new Vector3(-2, 3.5f, 0),
                        new Vector3(-2, 2.5f, 0)
                    },
                    new string[] {
                        "N_2", "N_2", "N_2", "N_2"
                    }
                ),
                // 웨이브 2: 두 번째 스폰: 5초 후, (2,4.5), (2,3.5), (2,2.5)에서 각 적이 "N_3" 4회 실행
                (
                    new Vector3[] {
                        new Vector3(2, 4.5f, 0),
                        new Vector3(2, 3.5f, 0),
                        new Vector3(2, 2.5f, 0)
                    },
                    new string[] {
                        "N_3", "N_3", "N_3", "N_3"
                    }
                )
            }
        },
        {
            3, new List<(Vector3[], string[])>
            {
                // 웨이브 3: 총 9마리 적을 순차적으로 스폰 (1초 딜레이)
                // 스폰 순서는: (-2,4.6) → (0,4.6) → (2,4.6)를 3회 반복
                (
                    new Vector3[]{ new Vector3(-2, 4.6f, 0) },
                    new string[]{ "N_0", "N_0", "N_0", "N_0", "N_0", "N_0",
                                  "N_0", "N_0", "N_0", "N_0", "N_0", "N_0" }
                ),
                (
                    new Vector3[]{ new Vector3(0, 4.6f, 0) },
                    new string[]{ "N_0", "N_0", "N_0", "N_0", "N_0", "N_0",
                                  "N_0", "N_0", "N_0", "N_0", "N_0", "N_0" }
                ),
                (
                    new Vector3[]{ new Vector3(2, 4.6f, 0) },
                    new string[]{ "N_0", "N_0", "N_0", "N_0", "N_0", "N_0",
                                  "N_0", "N_0", "N_0", "N_0", "N_0", "N_0" }
                ),
                (
                    new Vector3[]{ new Vector3(-2, 4.6f, 0) },
                    new string[]{ "N_0", "N_0", "N_0", "N_0", "N_0", "N_0",
                                  "N_0", "N_0", "N_0", "N_0", "N_0", "N_0" }
                ),
                (
                    new Vector3[]{ new Vector3(0, 4.6f, 0) },
                    new string[]{ "N_0", "N_0", "N_0", "N_0", "N_0", "N_0",
                                  "N_0", "N_0", "N_0", "N_0", "N_0", "N_0" }
                ),
                (
                    new Vector3[]{ new Vector3(2, 4.6f, 0) },
                    new string[]{ "N_0", "N_0", "N_0", "N_0", "N_0", "N_0",
                                  "N_0", "N_0", "N_0", "N_0", "N_0", "N_0" }
                ),
                (
                    new Vector3[]{ new Vector3(-2, 4.6f, 0) },
                    new string[]{ "N_0", "N_0", "N_0", "N_0", "N_0", "N_0",
                                  "N_0", "N_0", "N_0", "N_0", "N_0", "N_0" }
                ),
                (
                    new Vector3[]{ new Vector3(0, 4.6f, 0) },
                    new string[]{ "N_0", "N_0", "N_0", "N_0", "N_0", "N_0",
                                  "N_0", "N_0", "N_0", "N_0", "N_0", "N_0" }
                ),
                (
                    new Vector3[]{ new Vector3(2, 4.6f, 0) },
                    new string[]{ "N_0", "N_0", "N_0", "N_0", "N_0", "N_0",
                                  "N_0", "N_0", "N_0", "N_0", "N_0", "N_0" }
                )
            }
        }
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

                if (waveData.ContainsKey(currentWave))
                {
                    List<(Vector3[], string[])> waveSteps = waveData[currentWave];

                    if (currentWave == 3)
                    {
                        // 웨이브 3: 각 적을 순차적으로 스폰하며, 스폰 후 1초 대기
                        foreach (var step in waveSteps)
                        {
                            Vector3[] spawnPositions = step.Item1; // 한 좌표만 있음
                            string[] patterns = step.Item2;
                            yield return StartCoroutine(SpawnEnemies(spawnPositions, patterns));
                            yield return new WaitForSeconds(1f);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < waveSteps.Count; i++)
                        {
                            Vector3[] spawnPositions = waveSteps[i].Item1;
                            string[] patterns = waveSteps[i].Item2;
                            if (i > 0) yield return new WaitForSeconds(5f);
                            yield return StartCoroutine(SpawnEnemies(spawnPositions, patterns));
                        }
                    }
                }

                // 모든 스폰이 완료된 후, 적이 남아 있다면 제거될 때까지 대기
                while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
                {
                    yield return null;
                }

                Debug.Log($"Wave {currentWave} complete. Waiting {waveDelay} seconds before next wave...");
                yield return new WaitForSeconds(waveDelay);
                currentWave++;
                waveInProgress = false;
            }
            yield return null;
        }
    }

    IEnumerator SpawnEnemies(Vector3[] positions, string[] patterns)
    {
        foreach (Vector3 pos in positions)
        {
            GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
            if (enemy == null)
            {
                Debug.LogError("적 생성 실패! enemyPrefab이 올바르게 설정되어 있는지 확인하세요.");
                continue;
            }
            StartCoroutine(ExecutePattern(enemy, patterns));
        }
        yield return null;
    }

    IEnumerator ExecutePattern(GameObject enemy, string[] patterns)
    {
        if (enemy == null)
        {
            Debug.LogError("ExecutePattern() 실행 중 enemy가 null입니다!");
            yield break;
        }
        Debug.Log($"적 {enemy.name} - 패턴 실행 시작: {string.Join(", ", patterns)}");
        yield return StartCoroutine(PatternManager.Instance.ExecutePattern(enemy, patterns));
        Debug.Log($"적 {enemy.name} - 패턴 실행 완료");
    }
}
