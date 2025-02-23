using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;    // Enemy_0 (기본 적) 프리팹
    public GameObject enemyPrefab1;   // Enemy_1 (새 적, 체력 20, 총 미발사) 프리팹
    public float waveDelay = 5f;      // 웨이브 간 딜레이
    private int currentWave = 1;      // 현재 웨이브 번호
    private bool waveInProgress = false; // 웨이브 진행 여부

    // waveData: 각 웨이브의 스폰 이벤트 목록.
    // 각 스폰 이벤트는 (positions, patterns, enemyType) 튜플로 구성합니다.
    // positions: 스폰할 좌표 배열
    // patterns: 각 적에 적용할 패턴 배열(각 적마다 string[]), GetRepeatedPattern() 헬퍼로 손쉽게 생성
    // enemyType: 0이면 기본 적(Enemy_0), 1이면 새 적(Enemy_1)
    private Dictionary<int, List<(Vector3[] positions, string[][] patterns, int enemyType)>> waveData =
        new Dictionary<int, List<(Vector3[], string[][], int)>>()
    {
        // 웨이브 1
        { 1, new List<(Vector3[], string[][], int)>()
            {
                // 첫 번째 스폰: 좌표 {(-2,4.6), (0,4.6), (2,4.6)} – 각 "N_0" 4회 실행, enemyType 0
                (
                    new Vector3[] {
                        new Vector3(-2, 4.6f, 0),
                        new Vector3(0, 4.6f, 0),
                        new Vector3(2, 4.6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("N_0", 4),
                        GetRepeatedPattern("N_0", 4),
                        GetRepeatedPattern("N_0", 4)
                    },
                    0
                ),
                // 두 번째 스폰: 좌표 {(-2,4.6), (-1,4.6), (0,4.6), (1,4.6), (2,4.6)} – 각 "N_0" 3회 실행, enemyType 0
                (
                    new Vector3[] {
                        new Vector3(-2, 4.6f, 0),
                        new Vector3(-1, 4.6f, 0),
                        new Vector3(0, 4.6f, 0),
                        new Vector3(1, 4.6f, 0),
                        new Vector3(2, 4.6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("N_0", 3),
                        GetRepeatedPattern("N_0", 3),
                        GetRepeatedPattern("N_0", 3),
                        GetRepeatedPattern("N_0", 3),
                        GetRepeatedPattern("N_0", 3)
                    },
                    0
                )
            }
        },
        // 웨이브 2
        { 2, new List<(Vector3[], string[][], int)>()
            {
                // 첫 번째 스폰: 좌표 {(-2,4.5), (-2,3.5), (-2,2.5)} – 각 "N_2" 4회 실행, enemyType 0
                (
                    new Vector3[] {
                        new Vector3(-2, 4.5f, 0),
                        new Vector3(-2, 3.5f, 0),
                        new Vector3(-2, 2.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("N_2", 4),
                        GetRepeatedPattern("N_2", 4),
                        GetRepeatedPattern("N_2", 4)
                    },
                    0
                ),
                // 두 번째 스폰: 좌표 {(2,4.5), (2,3.5), (2,2.5)} – 각 "N_3" 4회 실행, enemyType 0
                (
                    new Vector3[] {
                        new Vector3(2, 4.5f, 0),
                        new Vector3(2, 3.5f, 0),
                        new Vector3(2, 2.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("N_3", 4),
                        GetRepeatedPattern("N_3", 4),
                        GetRepeatedPattern("N_3", 4)
                    },
                    0
                )
            }
        },
        // 웨이브 3
        { 3, new List<(Vector3[], string[][], int)>()
            {
                // 9마리를 순차적으로 (1초 간격) 스폰, 처음 3마리는 "N_0" 3번 실행, 다음 3마리는 2번 실행, 다음 3마리는 1번 실행, enemyType 0
                ( new Vector3[]{ new Vector3(-2, 4.6f, 0) }, new string[][] { GetRepeatedPattern("N_0", 3) }, 0 ),
                ( new Vector3[]{ new Vector3(0, 4.6f, 0) }, new string[][] { GetRepeatedPattern("N_0", 3) }, 0 ),
                ( new Vector3[]{ new Vector3(2, 4.6f, 0) }, new string[][] { GetRepeatedPattern("N_0", 3) }, 0 ),
                ( new Vector3[]{ new Vector3(-2, 4.6f, 0) }, new string[][] { GetRepeatedPattern("N_0", 2) }, 0 ),
                ( new Vector3[]{ new Vector3(0, 4.6f, 0) }, new string[][] { GetRepeatedPattern("N_0", 2) }, 0 ),
                ( new Vector3[]{ new Vector3(2, 4.6f, 0) }, new string[][] { GetRepeatedPattern("N_0", 2) }, 0 ),
                ( new Vector3[]{ new Vector3(-2, 4.6f, 0) }, new string[][] { GetRepeatedPattern("N_0", 1) }, 0 ),
                ( new Vector3[]{ new Vector3(0, 4.6f, 0) }, new string[][] { GetRepeatedPattern("N_0", 1) }, 0 ),
                ( new Vector3[]{ new Vector3(2, 4.6f, 0) }, new string[][] { GetRepeatedPattern("N_0", 1) }, 0 )
            }
        },
        // 웨이브 4
        { 4, new List<(Vector3[], string[][], int)>()
            {
                // 첫 번째 스폰 이벤트: 4마리 동시 스폰
                // 그룹1: 좌표 {(-2,4.6), (-1,4.6)} – 각 "N_0" 12회 실행
                // 그룹2: 좌표 {(-2,1.5), (-2,0.5)} – 각 "N_2" 6회 실행
                (
                    new Vector3[] {
                        new Vector3(-2, 4.6f, 0),
                        new Vector3(-1, 4.6f, 0),
                        new Vector3(-2, 1.5f, 0),
                        new Vector3(-2, 0.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("N_0", 12),
                        GetRepeatedPattern("N_0", 12),
                        GetRepeatedPattern("N_2", 6),
                        GetRepeatedPattern("N_2", 6)
                    },
                    0
                ),
                // 두 번째 스폰 이벤트 (5초 후): 4마리 동시 스폰
                // 그룹1: 좌표 {(1,4.6), (2,4.6)} – 각 "N_0" 12회 실행
                // 그룹2: 좌표 {(2,1.5), (2,0.5)} – 각 "N_3" 6회 실행
                (
                    new Vector3[] {
                        new Vector3(1, 4.6f, 0),
                        new Vector3(2, 4.6f, 0),
                        new Vector3(2, 1.5f, 0),
                        new Vector3(2, 0.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("N_0", 12),
                        GetRepeatedPattern("N_0", 12),
                        GetRepeatedPattern("N_3", 6),
                        GetRepeatedPattern("N_3", 6)
                    },
                    0
                )
            }
        },
        // 웨이브 5
        { 5, new List<(Vector3[], string[][], int)>()
            {
                // 첫 번째 스폰 이벤트: Enemy_0 (기본 적)
                // 좌표: {-2,4.6, -1,4.6, 0,4.6, 1,4.6, 2,4.6} – 각 "N_0" 12회 실행
                (
                    new Vector3[] {
                        new Vector3(-2, 4.6f, 0),
                        new Vector3(-1, 4.6f, 0),
                        new Vector3(0, 4.6f, 0),
                        new Vector3(1, 4.6f, 0),
                        new Vector3(2, 4.6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("N_0", 12),
                        GetRepeatedPattern("N_0", 12),
                        GetRepeatedPattern("N_0", 12),
                        GetRepeatedPattern("N_0", 12),
                        GetRepeatedPattern("N_0", 12)
                    },
                    0
                ),
                // 두 번째 스폰 이벤트 (5초 후): Enemy_1 (새 적)
                // 좌표: {(-2,3.5), (-2,1.5), (2,2.5), (2,0.5)}
                // 앞의 2마리: "N_2" 4회 실행, 뒤의 2마리: "N_3" 4회 실행
                (
                    new Vector3[] {
                        new Vector3(-2, 3.5f, 0),
                        new Vector3(-2, 1.5f, 0),
                        new Vector3(2, 2.5f, 0),
                        new Vector3(2, 0.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("N_2", 4),
                        GetRepeatedPattern("N_2", 4),
                        GetRepeatedPattern("N_3", 4),
                        GetRepeatedPattern("N_3", 4)
                    },
                    1
                )
            }
        }
    };

    // 헬퍼 메서드: 지정한 패턴을 count번 반복하는 배열을 생성합니다.
    private static string[] GetRepeatedPattern(string pattern, int count)
    {
        string[] arr = new string[count];
        for (int i = 0; i < count; i++)
        {
            arr[i] = pattern;
        }
        return arr;
    }

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
                    var waveSteps = waveData[currentWave];
                    // 웨이브 3는 각 적을 1초 간격으로 스폰
                    if (currentWave == 3)
                    {
                        foreach (var step in waveSteps)
                        {
                            yield return StartCoroutine(SpawnEnemies(step.positions, step.patterns, step.enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < waveSteps.Count; i++)
                        {
                            var step = waveSteps[i];
                            if (i > 0) yield return new WaitForSeconds(5f);
                            yield return StartCoroutine(SpawnEnemies(step.positions, step.patterns, step.enemyType));
                        }
                    }
                }

                // 스폰 완료 후, 화면에 남은 적이 모두 제거될 때까지 대기
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

    IEnumerator SpawnEnemies(Vector3[] positions, string[][] patterns, int enemyType)
    {
        // enemyType에 따라 사용할 프리팹을 선택합니다.
        GameObject prefabToUse = enemyType == 0 ? enemyPrefab : enemyPrefab1;
        for (int i = 0; i < positions.Length; i++)
        {
            GameObject enemy = Instantiate(prefabToUse, positions[i], Quaternion.identity);
            if (enemy == null)
            {
                Debug.LogError("적 생성 실패! prefab이 올바르게 설정되어 있는지 확인하세요.");
                continue;
            }
            StartCoroutine(ExecutePattern(enemy, patterns[i]));
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
