using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;    // Enemy_0 (기본 적) 프리팹
    public GameObject enemyPrefab1;   // Enemy_1 (새 적, 체력 20, 총 미발사) 프리팹
    public GameObject enemyPrefab2;   // Enemy_2 (새 적, 체력 50, 총 발사) 프리팹
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
                        GetRepeatedPattern("P_0", 4),
                        GetRepeatedPattern("P_0", 4),
                        GetRepeatedPattern("P_0", 4)
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
                        GetRepeatedPattern("P_0", 3),
                        GetRepeatedPattern("P_0", 3),
                        GetRepeatedPattern("P_0", 3),
                        GetRepeatedPattern("P_0", 3),
                        GetRepeatedPattern("P_0", 3)
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
                        GetRepeatedPattern("P_2", 4),
                        GetRepeatedPattern("P_2", 4),
                        GetRepeatedPattern("P_2", 4)
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
                        GetRepeatedPattern("P_3", 4),
                        GetRepeatedPattern("P_3", 4),
                        GetRepeatedPattern("P_3", 4)
                    },
                    0
                )
            }
        },
        // 웨이브 3
        { 3, new List<(Vector3[], string[][], int)>()
            {
                // 9마리를 순차적으로 (1초 간격) 스폰, 처음 3마리는 "N_0" 3번 실행, 다음 3마리는 2번 실행, 다음 3마리는 1번 실행, enemyType 0
                ( new Vector3[]{ new Vector3(-2, 4.6f, 0) }, new string[][] { GetRepeatedPattern("P_0", 4) }, 0 ),
                ( new Vector3[]{ new Vector3(0, 4.6f, 0) }, new string[][] { GetRepeatedPattern("P_0", 4) }, 0 ),
                ( new Vector3[]{ new Vector3(2, 4.6f, 0) }, new string[][] { GetRepeatedPattern("P_0", 4) }, 0 ),
                ( new Vector3[]{ new Vector3(-2, 4.6f, 0) }, new string[][] { GetRepeatedPattern("P_0", 3) }, 0 ),
                ( new Vector3[]{ new Vector3(0, 4.6f, 0) }, new string[][] { GetRepeatedPattern("P_0", 3) }, 0 ),
                ( new Vector3[]{ new Vector3(2, 4.6f, 0) }, new string[][] { GetRepeatedPattern("P_0", 3) }, 0 ),
                ( new Vector3[]{ new Vector3(-2, 4.6f, 0) }, new string[][] { GetRepeatedPattern("P_0", 2) }, 0 ),
                ( new Vector3[]{ new Vector3(0, 4.6f, 0) }, new string[][] { GetRepeatedPattern("P_0", 2) }, 0 ),
                ( new Vector3[]{ new Vector3(2, 4.6f, 0) }, new string[][] { GetRepeatedPattern("P_0", 2) }, 0 )
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
                        GetRepeatedPattern("P_0", 12),
                        GetRepeatedPattern("P_0", 12),
                        GetRepeatedPattern("P_2", 4),
                        GetRepeatedPattern("P_2", 4)
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
                        GetRepeatedPattern("P_0", 12),
                        GetRepeatedPattern("P_0", 12),
                        GetRepeatedPattern("P_3", 4),
                        GetRepeatedPattern("P_3", 4)
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
                        GetRepeatedPattern("P_0", 12),
                        GetRepeatedPattern("P_0", 12),
                        GetRepeatedPattern("P_0", 12),
                        GetRepeatedPattern("P_0", 12),
                        GetRepeatedPattern("P_0", 12)
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
                        ConcatPatterns(GetRepeatedPattern("P_2", 3), GetRepeatedPattern("P_0", 10)),
                        ConcatPatterns(GetRepeatedPattern("P_2", 4), GetRepeatedPattern("P_0", 10)),
                        ConcatPatterns(GetRepeatedPattern("P_3", 3), GetRepeatedPattern("P_0", 10)),
                        ConcatPatterns(GetRepeatedPattern("P_3", 4), GetRepeatedPattern("P_0", 10))
                    },
                    1
                )
            }
        },
        { 6, new List<(Vector3[], string[][], int)>()
            {
                // Group A: Enemy_0 3마리 동시에, 위치 (-2,4.6), (0,4.6), (2,4.6), 각 "N_0" 4회
                (
                    new Vector3[] {
                        new Vector3(-2, 4.6f, 0),
                        new Vector3(0, 4.6f, 0),
                        new Vector3(2, 4.6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 4),
                        GetRepeatedPattern("P_0", 4),
                        GetRepeatedPattern("P_0", 4)
                    },
                    0
                ),
                // Group B: Enemy_0 3마리 동시에, 같은 위치, 각 "N_0" 3회
                (
                    new Vector3[] {
                        new Vector3(-2, 4.6f, 0),
                        new Vector3(0, 4.6f, 0),
                        new Vector3(2, 4.6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 3),
                        GetRepeatedPattern("P_0", 3),
                        GetRepeatedPattern("P_0", 3)
                    },
                    0
                ),
                // Group C: Enemy_0 3마리 동시에, 같은 위치, 각 "N_0" 2회
                (
                    new Vector3[] {
                        new Vector3(-2, 4.6f, 0),
                        new Vector3(0, 4.6f, 0),
                        new Vector3(2, 4.6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 2),
                        GetRepeatedPattern("P_0", 2),
                        GetRepeatedPattern("P_0", 2)
                    },
                    0
                ),
                // Group D: 5초 후, Enemy_1 2마리 동시에, 위치 (-1,-4.6)와 (1,4.6), 각 "N_1" 10회
                (
                    new Vector3[] {
                        new Vector3(-1, -4.6f, 0),
                        new Vector3(1, -4.6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_1", 12),
                        GetRepeatedPattern("P_1", 12)
                    },
                    1
                )
            }
        },
        { 7, new List<(Vector3[], string[][], int)>()
        {
            // Group A 이벤트 1
            (
                new Vector3[] {
                    new Vector3(-2f, 4.5f, 0),
                    new Vector3(-1f, 4.5f, 0),
                    new Vector3(0f, 4.5f, 0),
                    new Vector3(-2f, 0.5f, 0),
                    new Vector3(-1f, 0.5f, 0),
                    new Vector3(0f, 0.5f, 0)
                },
                new string[][] {
                    GetRepeatedPattern("P_4", 7),
                    GetRepeatedPattern("P_4", 7),
                    GetRepeatedPattern("P_4", 7),
                    GetRepeatedPattern("P_4", 7),
                    GetRepeatedPattern("P_4", 7),
                    GetRepeatedPattern("P_4", 7)
                },
                0
            ),
            // Group A 이벤트 2 (2초 후)
            (
                new Vector3[] {
                    new Vector3(-2f, 4.5f, 0),
                    new Vector3(-1f, 4.5f, 0),
                    new Vector3(0f, 4.5f, 0),
                    new Vector3(-2f, 0.5f, 0),
                    new Vector3(-1f, 0.5f, 0),
                    new Vector3(0f, 0.5f, 0)
                },
                new string[][] {
                    GetRepeatedPattern("P_4", 7),
                    GetRepeatedPattern("P_4", 7),
                    GetRepeatedPattern("P_4", 7),
                    GetRepeatedPattern("P_4", 7),
                    GetRepeatedPattern("P_4", 7),
                    GetRepeatedPattern("P_4", 7)
                },
                0
            ),
            // Group B 이벤트 1 (그룹 A 완료 후 5초 대기)
            (
                new Vector3[] {
                    new Vector3(0f, 4.5f, 0),
                    new Vector3(1f, 4.5f, 0),
                    new Vector3(2f, 4.5f, 0),
                    new Vector3(0f, 0.5f, 0),
                    new Vector3(1f, 0.5f, 0),
                    new Vector3(2f, 0.5f, 0)
                },
                new string[][] {
                    GetRepeatedPattern("P_5", 7),
                    GetRepeatedPattern("P_5", 7),
                    GetRepeatedPattern("P_5", 7),
                    GetRepeatedPattern("P_5", 7),
                    GetRepeatedPattern("P_5", 7),
                    GetRepeatedPattern("P_5", 7)
                },
                0
            ),
            // Group B 이벤트 2 (2초 후)
            (
                new Vector3[] {
                    new Vector3(0f, 4.5f, 0),
                    new Vector3(1f, 4.5f, 0),
                    new Vector3(2f, 4.5f, 0),
                    new Vector3(0f, 0.5f, 0),
                    new Vector3(1f, 0.5f, 0),
                    new Vector3(2f, 0.5f, 0)
                },
                new string[][] {
                    GetRepeatedPattern("P_5", 7),
                    GetRepeatedPattern("P_5", 7),
                    GetRepeatedPattern("P_5", 7),
                    GetRepeatedPattern("P_5", 7),
                    GetRepeatedPattern("P_5", 7),
                    GetRepeatedPattern("P_5", 7)
                },
                0
            )
        }
        },
        { 8, new List<(Vector3[], string[][], int)>()
            {
                // 이벤트 1: Enemy_2 1마리, 위치 (-2, 4.5)
                (
                    new Vector3[] {
                        new Vector3(-2f, 4.5f, 0)
                    },
                    new string[][] {
                        // 패턴: N_4 4회 → N_1 4회 → N_5 4회 → N_1 4회
                        ConcatPatterns(
                            ConcatPatterns( GetRepeatedPattern("P_4", 4), GetRepeatedPattern("P_1", 4) ),
                            ConcatPatterns( GetRepeatedPattern("P_5", 4), GetRepeatedPattern("P_1", 4) )
                        )
                    },
                    2
                ),
                // 이벤트 2: 게임 시작 5초 후 Enemy_0 3마리, 위치 (3,1.5), (4,1.5), (5,1.5)
                (
                    new Vector3[] {
                        new Vector3(3f, 1.5f, 0),
                        new Vector3(4f, 1.5f, 0),
                        new Vector3(5f, 1.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_7", 3),
                        GetRepeatedPattern("P_7", 3),
                        GetRepeatedPattern("P_7", 3)
                    },
                    0
                ),
                // 이벤트 3: 바로 이어서(5초 후) Enemy_1 3마리, 위치 (3,-2.5), (4,-2.5), (5,-2.5)
                (
                    new Vector3[] {
                        new Vector3(3f, -2.5f, 0),
                        new Vector3(4f, -2.5f, 0),
                        new Vector3(5f, -2.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_3", 3),
                        GetRepeatedPattern("P_3", 3),
                        GetRepeatedPattern("P_3", 3)
                    },
                    1
                )
            }
        },
        { 9, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 스폰 이벤트 1
                (
                    new Vector3[] { new Vector3(0f, 4.6f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_8", 15)) },
                    0
                ),
                // 스폰 이벤트 2
                (
                    new Vector3[] { new Vector3(0f, 4.6f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_8", 15)) },
                    0
                ),
                // 스폰 이벤트 3
                (
                    new Vector3[] { new Vector3(0f, 4.6f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_8", 15)) },
                    0
                ),
                // 스폰 이벤트 4
                (
                    new Vector3[] { new Vector3(0f, 4.6f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_8", 15)) },
                    0
                ),
                // 스폰 이벤트 5
                (
                    new Vector3[] { new Vector3(0f, 4.6f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_8", 15)) },
                    0
                )
            }
        },
        { 10, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
        // 그룹 A: 5번 반복, 매 번 2마리 스폰
                (
                    new Vector3[] {
                        new Vector3(-1f, 4.6f, 0),
                        new Vector3(1f, 4.6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_8", 20)),
                        ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_9", 20))
                    },
                    0
                ),
                (
                    new Vector3[] {
                        new Vector3(-1f, 4.6f, 0),
                        new Vector3(1f, 4.6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_8", 20)),
                        ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_9", 20))
                    },
                    0
                ),
                (
                    new Vector3[] {
                        new Vector3(-1f, 4.6f, 0),
                        new Vector3(1f, 4.6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_8", 20)),
                        ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_9", 20))
                    },
                    0
                ),
                (
                    new Vector3[] {
                        new Vector3(-1f, 4.6f, 0),
                        new Vector3(1f, 4.6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_8", 20)),
                        ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_9", 20))
                    },
                    0
                ),
                (
                    new Vector3[] {
                        new Vector3(-1f, 4.6f, 0),
                        new Vector3(1f, 4.6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_8", 20)),
                        ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_9", 20))
                    },
                    0
                ),
                // 그룹 B: 5초 후에 3마리 동시에 스폰
                (
                    new Vector3[] {
                        new Vector3(-3f, 4.5f, 0),
                        new Vector3(0f, 5.5f, 0),
                        new Vector3(3f, 5.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_2", 1),
                        GetRepeatedPattern("P_0", 1),
                        GetRepeatedPattern("P_3", 1)
                    },
                    0
                )
            }
        },
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

    private static string[] ConcatPatterns(string[] first, string[] second)
    {
        List<string> list = new List<string>(first);
        list.AddRange(second);
        return list.ToArray();
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
                if (currentWave > 5)
                {
                    Debug.Log($"Starting Wave {currentWave - 5}");
                }
                else if (currentWave > 10)
                {
                    Debug.Log($"Starting Wave {currentWave - 10}");
                }
                else Debug.Log($"Starting Wave {currentWave}");

                if (waveData.ContainsKey(currentWave))
                {
                    var waveSteps = waveData[currentWave];

                    if (currentWave == 3)
                    {
                        foreach (var step in waveSteps)
                        {
                            yield return StartCoroutine(SpawnEnemies(step.positions, step.patterns, step.enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                    }
                    else if (currentWave == 6)
                    {
                        // Wave 6: custom delays
                        for (int i = 0; i < waveSteps.Count; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            // Group A -> B: 1초, B -> C: 1초, C -> D: 5초
                            if (i == 0 || i == 1)
                                yield return new WaitForSeconds(1f);
                            else if (i == 2)
                                yield return new WaitForSeconds(5f);
                        }
                    }
                    else if (currentWave == 7)
                    {
                        for (int i = 0; i < waveSteps.Count; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            // Group A 이벤트 1 후 2초, 그룹 A 이벤트 2 후 5초, 그룹 B 이벤트 1 후 2초
                            if (i == 0) yield return new WaitForSeconds(2f);
                            else if (i == 1) yield return new WaitForSeconds(5f);
                            else if (i == 2) yield return new WaitForSeconds(2f);
                        }
                    }
                    else if (currentWave == 8)
                    {
                        // Wave 8: 이벤트 1 바로 스폰, 이후 5초 대기, 그리고 이벤트 2와 3 스폰 (연달아)
                        yield return StartCoroutine(SpawnEnemies(waveSteps[0].positions, waveSteps[0].patterns, waveSteps[0].enemyType));
                        yield return new WaitForSeconds(5f);
                        yield return StartCoroutine(SpawnEnemies(waveSteps[1].positions, waveSteps[1].patterns, waveSteps[1].enemyType));
                        yield return StartCoroutine(SpawnEnemies(waveSteps[2].positions, waveSteps[2].patterns, waveSteps[2].enemyType));
                    }
                    else if (currentWave == 9)
                    {
                        // Wave 9: 각 스폰 이벤트 후 1초 간격으로 진행
                        foreach (var step in waveSteps)
                        {
                            yield return StartCoroutine(SpawnEnemies(step.positions, step.patterns, step.enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                    }
                    else if (currentWave == 10)
                    {
                        waveSteps = waveData[currentWave];
                        // 그룹 A: 5번의 스폰 이벤트, 각 이벤트 후 1초 대기
                        for (int i = 0; i < 5; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                        // 그룹 B: 그 후 1초 대기하여 총 5초 후에 스폰 (혹은 바로 1초 대기 후 스폰)
                        yield return new WaitForSeconds(1f);
                        yield return StartCoroutine(SpawnEnemies(waveSteps[5].positions, waveSteps[5].patterns, waveSteps[5].enemyType));
                    }

                    else
                    {
                        for (int i = 0; i < waveSteps.Count; i++)
                        {
                            if (i > 0) yield return new WaitForSeconds(5f);
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                        }
                    }
                }

                // 스폰 완료 후, 화면에 남은 적이 모두 제거될 때까지 대기
                while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
                {
                    yield return null;
                }

                if (currentWave > 5)
                {
                    Debug.Log($"Wave {currentWave - 5} complete. Waiting {waveDelay} seconds before next wave...");
                }
                else if (currentWave > 10)
                {
                    Debug.Log($"Wave {currentWave - 10} complete. Waiting {waveDelay} seconds before next wave...");
                }
                else Debug.Log($"Wave {currentWave} complete. Waiting {waveDelay} seconds before next wave...");
                yield return new WaitForSeconds(waveDelay);
                currentWave++;
                waveInProgress = false;
            }
            yield return null;
        }
    }

    IEnumerator SpawnEnemies(Vector3[] positions, string[][] patterns, int enemyType)
    {
        GameObject prefabToUse = null;
        if (enemyType == 0)
            prefabToUse = enemyPrefab;
        else if (enemyType == 1)
            prefabToUse = enemyPrefab1;
        else if (enemyType == 2)
            prefabToUse = enemyPrefab2;
        else
            prefabToUse = enemyPrefab; // fallback

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject enemy = Instantiate(prefabToUse, positions[i], Quaternion.identity);
            if (enemy == null)
            {
                Debug.LogError("적 생성 실패! prefab이 올바르게 설정되어 있는지 확인하세요.");
                continue;
            }
            // 웨이브 5의 두 번째 스폰 이벤트에서 enemyType이 1인 경우,
            // 180도 회전 대신 localScale.x를 음수로 설정하고 isFlipped 플래그를 true로 합니다.
            if (currentWave == 6 && enemyType == 1)
            {
                Vector3 scale = enemy.transform.localScale;
                enemy.transform.localScale = new Vector3(scale.x, -Mathf.Abs(scale.y), scale.z);
                Enemy enemyComponent = enemy.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.isFlipped = true;
                    enemyComponent.isEnemy1 = true; // Enemy_1임을 표시 (Enemy.cs에 새 필드 추가)
                }
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
