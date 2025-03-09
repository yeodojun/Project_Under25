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
                        GetRepeatedPattern("P_0", 20),
                        GetRepeatedPattern("P_0", 20),
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
                        GetRepeatedPattern("P_0", 20),
                        GetRepeatedPattern("P_0", 20),
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
                // 좌표: {-2,4.6, -1,4.6, 0,4.6, 1,4.6, 2,4.6} – 각 "P_0" 20회 실행
                (
                    new Vector3[] {
                        new Vector3(-2, 4.6f, 0),
                        new Vector3(-1, 4.6f, 0),
                        new Vector3(0, 4.6f, 0),
                        new Vector3(1, 4.6f, 0),
                        new Vector3(2, 4.6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 20),
                        GetRepeatedPattern("P_0", 20),
                        GetRepeatedPattern("P_0", 20),
                        GetRepeatedPattern("P_0", 20),
                        GetRepeatedPattern("P_0", 20)
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
                        ConcatPatterns(GetRepeatedPattern("P_2", 3), GetRepeatedPattern("P_0", 20)),
                        ConcatPatterns(GetRepeatedPattern("P_2", 4), GetRepeatedPattern("P_0", 20)),
                        ConcatPatterns(GetRepeatedPattern("P_3", 3), GetRepeatedPattern("P_0", 20)),
                        ConcatPatterns(GetRepeatedPattern("P_3", 4), GetRepeatedPattern("P_0", 20))
                    },
                    1
                )
            }
        },
        // 웨이브 6 추가
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
                        GetRepeatedPattern("P_1", 20),
                        GetRepeatedPattern("P_1", 20)
                    },
                    1
                )
            }
        },
        // 웨이브 7 추가
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
                    GetRepeatedPattern("P_8", 15),
                    GetRepeatedPattern("P_8", 15),
                    GetRepeatedPattern("P_8", 15),
                    GetRepeatedPattern("P_8", 15),
                    GetRepeatedPattern("P_8", 15),
                    GetRepeatedPattern("P_8", 15)
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
                    GetRepeatedPattern("P_8", 15),
                    GetRepeatedPattern("P_8", 15),
                    GetRepeatedPattern("P_8", 15),
                    GetRepeatedPattern("P_8", 15),
                    GetRepeatedPattern("P_8", 15),
                    GetRepeatedPattern("P_8", 15)
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
                    GetRepeatedPattern("P_9", 15),
                    GetRepeatedPattern("P_9", 15),
                    GetRepeatedPattern("P_9", 15),
                    GetRepeatedPattern("P_9", 15),
                    GetRepeatedPattern("P_9", 15),
                    GetRepeatedPattern("P_9", 15)
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
                    GetRepeatedPattern("P_9", 15),
                    GetRepeatedPattern("P_9", 15),
                    GetRepeatedPattern("P_9", 15),
                    GetRepeatedPattern("P_9", 15),
                    GetRepeatedPattern("P_9", 15),
                    GetRepeatedPattern("P_9", 15)
                },
                0
            )
        }
        },
        // 웨이브 8 추가
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
                            ConcatPatterns( GetRepeatedPattern("P_8", 4), GetRepeatedPattern("P_1", 4) ),
                            ConcatPatterns( GetRepeatedPattern("P_9", 4), GetRepeatedPattern("P_1", 4) )
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
                        GetRepeatedPattern("P_11", 3),
                        GetRepeatedPattern("P_11", 3),
                        GetRepeatedPattern("P_11", 3)
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
        // 웨이브 9 추가
        { 9, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 스폰 이벤트 1
                (
                    new Vector3[] { new Vector3(0f, 4.6f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_16", 15)) },
                    0
                ),
                // 스폰 이벤트 2
                (
                    new Vector3[] { new Vector3(0f, 4.6f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_16", 15)) },
                    0
                ),
                // 스폰 이벤트 3
                (
                    new Vector3[] { new Vector3(0f, 4.6f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_16", 15)) },
                    0
                ),
                // 스폰 이벤트 4
                (
                    new Vector3[] { new Vector3(0f, 4.6f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_16", 15)) },
                    0
                ),
                // 스폰 이벤트 5
                (
                    new Vector3[] { new Vector3(0f, 4.6f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_16", 15)) },
                    0
                )
            }
        },
        // 웨이브 10 추가
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
                        new Vector3(3f, 4.5f, 0)
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
        // 웨이브 11 추가
        { 11, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
        // 그룹 A: 5회 반복, 매 회 (-3, -1)에서 Enemy_0 스폰, 패턴: P_6 3회 + P_17 15회
                (
                    new Vector3[] { new Vector3(-3f, -1f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_6", 3), GetRepeatedPattern("P_17", 15)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-3f, -1f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_6", 3), GetRepeatedPattern("P_17", 15)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-3f, -1f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_6", 3), GetRepeatedPattern("P_17", 15)) },
            0
                ),
                (
                    new Vector3[] { new Vector3(-3f, -1f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_6", 3), GetRepeatedPattern("P_17", 15)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-3f, -1f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_6", 3), GetRepeatedPattern("P_17", 15)) },
                    0
                ),
                // 그룹 B: 5초 후, 5회 반복, 매 회 (3, -1)에서 Enemy_0 스폰, 패턴: P_5 3회 + P_16 15회
                (
                    new Vector3[] { new Vector3(3f, -1f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_5", 3), GetRepeatedPattern("P_16", 15)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(3f, -1f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_5", 3), GetRepeatedPattern("P_16", 15)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(3f, -1f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_5", 3), GetRepeatedPattern("P_16", 15)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(3f, -1f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_5", 3), GetRepeatedPattern("P_16", 15)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(3f, -1f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_5", 3), GetRepeatedPattern("P_16", 15)) },
                    0
                )
            }
        },
        // 웨이브 12 추가
        { 12, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
        // 그룹 A: 5회 반복, 매 회 (-2, -1.5)에서 Enemy_0 스폰, 패턴: P_2 2회 + P_1 5회 + P_3 1회 + P_0 4,3,2,1,0회
                (
                    new Vector3[] {
                        new Vector3(-2f, -1.5f, 0),
                        new Vector3(2f, -1.5f, 0) },
                    new string[][] {
                        // 패턴: P_2 2회 → P_1 5회 → P_3 1회 → P_0 4회
                        ConcatPatterns(
                            ConcatPatterns( GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_1", 5) ),
                            ConcatPatterns( GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_0", 4) )
                        ), // 패턴: P_3 2회 → P_1 5회 → P_2 1회 → P_0 4회
                        ConcatPatterns(
                            ConcatPatterns( GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_1", 5) ),
                            ConcatPatterns( GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_0", 4) )
                        )
                    },
                    0
                ),
                (
                    new Vector3[] {
                        new Vector3(-2f, -1.5f, 0),
                        new Vector3(2f, -1.5f, 0) },
                    new string[][] {
                        // 패턴: P_2 2회 → P_1 5회 → P_3 1회 → P_0 3회
                        ConcatPatterns(
                            ConcatPatterns( GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_1", 5) ),
                            ConcatPatterns( GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_0", 3) )
                        ), // 패턴: P_3 2회 → P_1 5회 → P_2 1회 → P_0 3회
                        ConcatPatterns(
                            ConcatPatterns( GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_1", 5) ),
                            ConcatPatterns( GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_0", 3) )
                        )
                    },
                    0
                ),
                (
                    new Vector3[] {
                        new Vector3(-2f, -1.5f, 0),
                        new Vector3(2f, -1.5f, 0) },
                    new string[][] {
                        // 패턴: P_2 2회 → P_1 5회 → P_3 1회 → P_0 2회
                        ConcatPatterns(
                            ConcatPatterns( GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_1", 5) ),
                            ConcatPatterns( GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_0", 2) )
                        ), // 패턴: P_3 2회 → P_1 5회 → P_2 1회 → P_0 2회
                        ConcatPatterns(
                            ConcatPatterns( GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_1", 5) ),
                            ConcatPatterns( GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_0", 2) )
                        )
                    },
                    0
                ),
                (
                    new Vector3[] {
                        new Vector3(-2f, -1.5f, 0),
                        new Vector3(2f, -1.5f, 0) },
                    new string[][] {
                        // 패턴: P_2 2회 → P_1 5회 → P_3 1회 → P_0 1회
                        ConcatPatterns(
                            ConcatPatterns( GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_1", 5) ),
                            ConcatPatterns( GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_0", 1) )
                        ), // 패턴: P_3 2회 → P_1 5회 → P_2 1회 → P_0 1회
                        ConcatPatterns(
                            ConcatPatterns( GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_1", 5) ),
                            ConcatPatterns( GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_0", 1) )
                        )
                    },
                    0
                ),
                (
                    new Vector3[] {
                        new Vector3(-2f, -1.5f, 0),
                        new Vector3(2f, -1.5f, 0) },
                    new string[][] {
                        // 패턴: P_2 2회 → P_1 5회 → P_3 1회 → P_0 0회
                        ConcatPatterns(
                            ConcatPatterns( GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_1", 5) ),
                            GetRepeatedPattern("P_3", 1)
                        ), // 패턴: P_3 2회 → P_1 5회 → P_2 1회 → P_0 0회
                        ConcatPatterns(
                            ConcatPatterns( GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_1", 5) ),
                            GetRepeatedPattern("P_2", 1)
                        )
                    },
                    0
                ),
            }
        },
        // 웨이브 13 추가
        { 13, new List<(Vector3[], string[][], int)>()
            {
                // 첫 번째 스폰: 좌표 {(-2,4.6), (0,4.6), (2,4.6)} – 각 "N_0" 4회 실행, enemyType 0
                (
                    new Vector3[] {
                        new Vector3(0, 1.5f, 0),
                        new Vector3(0, 1.5f, 0),
                        new Vector3(0, 1.5f, 0),
                        new Vector3(0, 1.5f, 0),
                        new Vector3(0, 1.5f, 0),
                        new Vector3(0, 1.5f, 0),
                        new Vector3(0, 1.5f, 0),
                        new Vector3(0, 1.5f, 0),
                        new Vector3(0, 1.5f, 0),
                        new Vector3(0, 1.5f, 0),
                        new Vector3(0, 1.5f, 0),
                        new Vector3(0, 1.5f, 0),
                    },
                    new string[][] {
                        GetRepeatedPattern("P_4", 2),
                        GetRepeatedPattern("P_5", 2),
                        GetRepeatedPattern("P_6", 2),
                        GetRepeatedPattern("P_7", 2),
                        GetRepeatedPattern("P_8", 2),
                        GetRepeatedPattern("P_9", 2),
                        GetRepeatedPattern("P_10", 2),
                        GetRepeatedPattern("P_11", 2),
                        GetRepeatedPattern("P_12", 2),
                        GetRepeatedPattern("P_13", 2),
                        GetRepeatedPattern("P_14", 2),
                        GetRepeatedPattern("P_15", 2),
                    },
                    0
                ),
            }
        },
        // 웨이브 14 추가
        { 14, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 인덱스 0: Enemy_1 그룹
                (
                    new Vector3[] {
                        new Vector3(-2f, 3.5f, 0),
                        new Vector3(-2f, 2.5f, 0),
                        new Vector3(-2f, -3.5f, 0),
                        new Vector3(-2f, -4.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_2", 20),
                        GetRepeatedPattern("P_2", 20),
                        GetRepeatedPattern("P_2", 20),
                        GetRepeatedPattern("P_2", 20)
                    },
                    1
             ),
                // 인덱스 1: Enemy_0 그룹 1
                (
                    new Vector3[] {
                        new Vector3(-1f, 4.6f, 0),
                        new Vector3(0f, 4.6f, 0),
                        new Vector3(1f, 4.6f, 0),
                        new Vector3(2f, 4.6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 4),
                        GetRepeatedPattern("P_0", 4),
                       GetRepeatedPattern("P_0", 4),
                        GetRepeatedPattern("P_0", 4)
                    },
                    0
                ),
                // 인덱스 2: Enemy_0 그룹 2
                (
                    new Vector3[] {
                        new Vector3(-1f, 4.6f, 0),
                        new Vector3(0f, 4.6f, 0),
                        new Vector3(1f, 4.6f, 0),
                        new Vector3(2f, 4.6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 3),
                        GetRepeatedPattern("P_0", 3),
                        GetRepeatedPattern("P_0", 3),
                        GetRepeatedPattern("P_0", 3)
                    },
                    0
                )
            }
        },
        // 웨이브 15 추가
        { 15, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 그룹 A: 5회 반복 (인덱스 0 ~ 4)
                // 각 이벤트는 (2, -1.5)에서 스폰, 패턴은 P_11 (4회) + P_2 (4회) + P_0 (횟수 조절)
                (
                    new Vector3[] { new Vector3(2f, -1.5f, 0) },
                    new string[][] { ConcatPatterns( ConcatPatterns(GetRepeatedPattern("P_11", 4), GetRepeatedPattern("P_2", 4) ),
                                                        GetRepeatedPattern("P_0", 4) ) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(2f, -1.5f, 0) },
                    new string[][] { ConcatPatterns( ConcatPatterns(GetRepeatedPattern("P_11", 4), GetRepeatedPattern("P_2", 4) ),
                                                        GetRepeatedPattern("P_0", 3) ) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(2f, -1.5f, 0) },
                    new string[][] { ConcatPatterns( ConcatPatterns(GetRepeatedPattern("P_11", 4), GetRepeatedPattern("P_2", 4) ),
                                                        GetRepeatedPattern("P_0", 2) ) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(2f, -1.5f, 0) },
                    new string[][] { ConcatPatterns( ConcatPatterns(GetRepeatedPattern("P_11", 4), GetRepeatedPattern("P_2", 4) ),
                                                        GetRepeatedPattern("P_0", 1) ) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(2f, -1.5f, 0) },
                    new string[][] { ConcatPatterns( ConcatPatterns(GetRepeatedPattern("P_11", 4), GetRepeatedPattern("P_2", 4) ),
                                                        GetRepeatedPattern("P_0", 0) ) },
                    0
                ),
                // 그룹 B: 5회 반복 (인덱스 5 ~ 9)
                // 각 이벤트는 (-2, -1.5)에서 스폰, 패턴은 P_10 2회 + P_17 15회
                (
                    new Vector3[] { new Vector3(-2f, -1.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_10", 2), GetRepeatedPattern("P_17", 15)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-2f, -1.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_10", 2), GetRepeatedPattern("P_17", 15)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-2f, -1.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_10", 2), GetRepeatedPattern("P_17", 15)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-2f, -1.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_10", 2), GetRepeatedPattern("P_17", 15)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-2f, -1.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_10", 2), GetRepeatedPattern("P_17", 15)) },
                    0
                )
            }
        },
        // 웨이브 16
        { 16, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 그룹 A: 웨이브 시작 0초에 Enemy_0가 (-3,5.5), (-2,5.5), (-1,5.5), (0,5.5), (1,5.5)에서 스폰,
                //         각 적은 P_8을 1회 수행.
                (
                    new Vector3[] {
                        new Vector3(-3f, 5.5f, 0),
                        new Vector3(-2f, 5.5f, 0),
                        new Vector3(-1f, 5.5f, 0),
                        new Vector3(0f, 5.5f, 0),
                        new Vector3(1f, 5.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_8", 1),
                        GetRepeatedPattern("P_8", 1),
                        GetRepeatedPattern("P_8", 1),
                        GetRepeatedPattern("P_8", 1),
                        GetRepeatedPattern("P_8", 1)
                    },
                    0
                ),
                // 그룹 B: 웨이브 시작 2초 후에 Enemy_0가 (0,5.5), (1,5.5), (2,5.5), (3,5.5), (4,5.5)에서 스폰,
                //         각 적은 P_9를 2회 수행.
                (
                    new Vector3[] {
                        new Vector3(0f, 5.5f, 0),
                        new Vector3(1f, 5.5f, 0),
                        new Vector3(2f, 5.5f, 0),
                        new Vector3(3f, 5.5f, 0),
                        new Vector3(4f, 5.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_9", 2),
                        GetRepeatedPattern("P_9", 2),
                        GetRepeatedPattern("P_9", 2),
                        GetRepeatedPattern("P_9", 2),
                        GetRepeatedPattern("P_9", 2)
                    },
                    0
                ),
                // 그룹 C: 웨이브 시작 3초 후에 Enemy_0가 (-3,2.5)와 (3,2.5)에서 스폰,
                //         각 적은 P_2를 2회 수행한 후 P_3를 2회 수행.
                (
                    new Vector3[] {
                        new Vector3(-3f, 2.5f, 0),
                        new Vector3(3f, 2.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_2", 2),
                        GetRepeatedPattern("P_3", 2)
                    },
                    0
                ),
                // 그룹 D: 1초 딜레이 후(즉, 웨이브 시작 4초 후) Enemy_0가 (-3,2.5)와 (3,2.5)에서 다시 스폰,
                //         이번엔 각 적이 P_2를 1회, P_3를 1회 수행.
                (
                    new Vector3[] {
                        new Vector3(-3f, 2.5f, 0),
                        new Vector3(3f, 2.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_2", 1),
                        GetRepeatedPattern("P_3", 1)
                    },
                    0
                ),
                // 그룹 E: 웨이브 시작 4초 후에 Enemy_2가 (0,-5.5)에서 1개 스폰,
                //         P_0를 3회 수행.
                (
                    new Vector3[] { new Vector3(0f, 5.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_0", 3) },
                    2
                )
            }
        },
        // 웨이브 17
        { 17, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 그룹 A: 웨이브 시작 0초에 Enemy_0가 (-2, 4.5)에서 스폰
                (
                    new Vector3[] { new Vector3(-2f, 4.5f, 0) },
                    new string[][] {
                        ConcatPatterns(
                            ConcatPatterns(
                                ConcatPatterns( GetRepeatedPattern("P_4", 4), GetRepeatedPattern("P_5", 4) ),
                                GetRepeatedPattern("P_4", 4)
                            ),
                            GetRepeatedPattern("P_5", 20)
                        )
                    },
                    0
                ),
                // 그룹 B: 웨이브 시작 5초 후에 Enemy_0가 (2, 4.5)에서 스폰
                (
                    new Vector3[] { new Vector3(2f, 4.5f, 0) },
                    new string[][] {
                        ConcatPatterns(
                            ConcatPatterns(
                                ConcatPatterns( GetRepeatedPattern("P_5", 4), GetRepeatedPattern("P_4", 4) ),
                            GetRepeatedPattern("P_5", 4)
                            ),
                            GetRepeatedPattern("P_4", 20)
                        )
                    },
                    0
                )
            }
        },
        // 웨이브 18
        { 18, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 그룹 A: 0초, Enemy_0 2마리 스폰
                (
                    new Vector3[] {
                        new Vector3(-2f, 4.5f, 0),
                        new Vector3(2f, 4.5f, 0)
                    },
                        new string[][] {
                        GetRepeatedPattern("P_12", 4),  // 왼쪽 적
                        GetRepeatedPattern("P_13", 3)   // 오른쪽 적
                    },
                    0
                ),
                // 그룹 B: 1초 후, Enemy_0 2마리 스폰
                (
                    new Vector3[] {
                        new Vector3(-2f, 4.5f, 0),
                        new Vector3(2f, 4.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_12", 3),  // 왼쪽 적
                        GetRepeatedPattern("P_13", 2)   // 오른쪽 적
                    },
                    0
                ),
                // 그룹 C: 1초 후, Enemy_0 2마리 스폰
                (
                    new Vector3[] {
                        new Vector3(-2f, 4.5f, 0),
                        new Vector3(2f, 4.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_12", 2),  // 왼쪽 적
                        GetRepeatedPattern("P_13", 1)   // 오른쪽 적
                    },
                    0
                ),
                // 그룹 D: 1초 후, Enemy_0 2마리 스폰
                (
                    new Vector3[] {
                        new Vector3(-2f, 4.5f, 0),
                        new Vector3(2f, 4.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_12", 1),  // 왼쪽 적
                        new string[0]                   // 오른쪽 적: 빈 패턴 (아무것도 수행하지 않음)
                    },
                    0
                ),
                // 그룹 E: 1초 후, Enemy_0 단독 스폰 (왼쪽)
                (
                    new Vector3[] { new Vector3(-2f, 4.5f, 0) },
                    new string[][] { new string[0] },  // 아무 패턴 없음
                    0
                ),
                // 그룹 F: 웨이브 시작 5초 후, Enemy_3 스폰 at (0,3.5), 패턴: P_22 1회
                (
                    new Vector3[] { new Vector3(0f, 3.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_22", 1) },
                    3
                ),
                // 그룹 G: 3초 후, Enemy_3 스폰 at (0,3.5), 패턴: P_22 1회
                (
                    new Vector3[] { new Vector3(0f, 3.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_22", 1) },
                    3
                )
            }
        },
        // 웨이브 19
        { 19, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 그룹 A: t=0, (-2,0.5)와 (-2,1.5)에서 2마리, P_2 4회 수행
                (
                    new Vector3[] { new Vector3(-2f, 0.5f, 0), new Vector3(-2f, 1.5f, 0) },
                    new string[][] {
                        GetRepeatedPattern("P_2", 4),
                        GetRepeatedPattern("P_2", 4)
                    },
                    0
                ),
                // 그룹 B: t=1, (-2,0.5)와 (-2,1.5)에서 2마리, P_2 3회 수행
                (
                    new Vector3[] { new Vector3(-2f, 0.5f, 0), new Vector3(-2f, 1.5f, 0) },
                    new string[][] {
                        GetRepeatedPattern("P_2", 3),
                        GetRepeatedPattern("P_2", 3)
                    },
                    0
                ),
                // 그룹 C: t=5, (-2,0.5)와 (-2,1.5)에서 2마리, P_2 2회 수행
                (
                    new Vector3[] { new Vector3(-2f, 0.5f, 0), new Vector3(-2f, 1.5f, 0) },
                    new string[][] {
                        GetRepeatedPattern("P_2", 2),
                        GetRepeatedPattern("P_2", 2)
                    },
                    0
                ),
                // 그룹 D: t=6, (-2,0.5)와 (-2,1.5)에서 2마리, P_2 1회 수행
                (
                    new Vector3[] { new Vector3(-2f, 0.5f, 0), new Vector3(-2f, 1.5f, 0) },
                    new string[][] {
                        GetRepeatedPattern("P_2", 1),
                        GetRepeatedPattern("P_2", 1)
                    },
                    0
                ),
                // 그룹 E: t=7, Enemy_0가 (-2,-4.5), (-2,-3.5), (-1,-4.5), (-1,-3.5)에서 스폰
                //         단, 이 그룹은 각 적을 1초 간격으로 개별 스폰하여, 각 적은 패턴 "P_25R"를 실행
                (
                    new Vector3[] {
                        new Vector3(-2f, -4.5f, 0),
                        new Vector3(-2f, -3.5f, 0),
                        new Vector3(-1f, -4.5f, 0),
                        new Vector3(-1f, -3.5f, 0)
                    },
                    new string[][] {
                        new string[] { "P_25R" },
                        new string[] { "P_25R" },
                        new string[] { "P_25R" },
                        new string[] { "P_25R" }
                    },
                    0
                )
            }
        },
        
        // 웨이브 20
        { 20, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 그룹 A: Enemy_0 at (2, 2.5)
                (
                    new Vector3[] { new Vector3(2f, 2.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_16", 20)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(2f, 2.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_16", 20)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(2f, 2.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_16", 20)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(2f, 2.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_16", 20)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(2f, 2.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_16", 20)) },
                    0
                ),
                // 그룹 B: Enemy_0 at (-2, 0.5)
                (
                    new Vector3[] { new Vector3(-2f, 1.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_17", 20)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-2f, 1.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_17", 20)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-2f, 1.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_17", 20)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-2f, 1.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_17", 20)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-2f, 1.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_17", 20)) },
                    0
                ),
                // 그룹 C: Enemy_0 at (-2, -2.5)
                (
                    new Vector3[] { new Vector3(-2f, -2.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_2", 3), GetRepeatedPattern("P_17", 20)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-2f, -2.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_2", 3), GetRepeatedPattern("P_17", 20)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-2f, -2.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_2", 3), GetRepeatedPattern("P_17", 20)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-2f, -2.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_2", 3), GetRepeatedPattern("P_17", 20)) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-2f, -2.5f, 0) },
                    new string[][] { ConcatPatterns(GetRepeatedPattern("P_2", 3), GetRepeatedPattern("P_17", 20)) },
                    0
                ),
                // 그룹 D: Enemy_0 at (-2, 4.5), 단 1회 스폰, 패턴: P_25R 1회
                (
                    new Vector3[] { new Vector3(-2f, 4.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_25R", 1) },
                    0
                )
            }
        },
        // 웨이브 21
        { 21, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 그룹 1: 웨이브 시작 0초 – Enemy_2 스폰 그룹
                // 왼쪽 좌표: (-2, 3.5) – 패턴: [P_6 2회, P_8 2회, P_3 4회]를 5번 반복
                (
                    new Vector3[] { new Vector3(-2f, 3.5f, 0) },
                    new string[][] { RepeatPattern(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_6", 2), GetRepeatedPattern("P_8", 2)), GetRepeatedPattern("P_3", 4)), 5) },
                    2
                ),
                // 가운데 좌표: (0, 0.5) – 패턴: [P_3 4회, P_6 2회, P_8 2회]를 5번 반복
                (
                    new Vector3[] { new Vector3(0f, 0.5f, 0) },
                    new string[][] { RepeatPattern(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 4), GetRepeatedPattern("P_6", 2)), GetRepeatedPattern("P_8", 2)), 5) },
                    2
                ),
                // 오른쪽 좌표: (2, 3.5) – 패턴: P_23 40회 수행
                (
                    new Vector3[] { new Vector3(2f, 3.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_23", 40) },
                    2
                ),

                // 그룹 2: 웨이브 시작 5초 후 – Enemy_1 스폰 그룹
                // 매 스폰마다 2마리 동시에, 좌표: (-2,-1.5)와 (2,-1.5)
                // 왼쪽 적: P_18 1회, 오른쪽 적: P_19 1회
                (
                    new Vector3[] { new Vector3(-2f, -1.5f, 0), new Vector3(2f, -1.5f, 0) },
                    new string[][] {
                        GetRepeatedPattern("P_18", 1),
                        GetRepeatedPattern("P_19", 1)
                    },
                    1
                ),
                (
                    new Vector3[] { new Vector3(-2f, -1.5f, 0), new Vector3(2f, -1.5f, 0) },
                    new string[][] {
                        GetRepeatedPattern("P_18", 1),
                        GetRepeatedPattern("P_19", 1)
                    },
                    1
                ),
                (
                    new Vector3[] { new Vector3(-2f, -1.5f, 0), new Vector3(2f, -1.5f, 0) },
                    new string[][] {
                        GetRepeatedPattern("P_18", 1),
                        GetRepeatedPattern("P_19", 1)
                    },
                    1
                )
            }
        },
        // 웨이브 22
        { 22, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 그룹 A: 인덱스 0~4 (5회 반복)
                (
                    new Vector3[] {
                        new Vector3(-2f, 4.6f, 0),
                        new Vector3(0f, 4.6f, 0),
                        new Vector3(1f, 4.6f, 0),
                        new Vector3(2f, 4.6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20", 30),
                        GetRepeatedPattern("P_20", 30),
                        GetRepeatedPattern("P_20", 30),
                        GetRepeatedPattern("P_20", 30)
                    },
                    0
                ),
                (
                    new Vector3[] {
                        new Vector3(-2f, 4.6f, 0),
                        new Vector3(0f, 4.6f, 0),
                        new Vector3(1f, 4.6f, 0),
                        new Vector3(2f, 4.6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20", 30),
                        GetRepeatedPattern("P_20", 30),
                        GetRepeatedPattern("P_20", 30),
                        GetRepeatedPattern("P_20", 30)
                    },
                    0
                ),
                (
                    new Vector3[] {
                        new Vector3(-2f, 4.6f, 0),
                        new Vector3(0f, 4.6f, 0),
                        new Vector3(1f, 4.6f, 0),
                        new Vector3(2f, 4.6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20", 30),
                        GetRepeatedPattern("P_20", 30),
                        GetRepeatedPattern("P_20", 30),
                        GetRepeatedPattern("P_20", 30)
                    },
                    0
                ),
                (
                    new Vector3[] {
                        new Vector3(-2f, 4.6f, 0),
                        new Vector3(0f, 4.6f, 0),
                        new Vector3(1f, 4.6f, 0),
                        new Vector3(2f, 4.6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20", 30),
                        GetRepeatedPattern("P_20", 30),
                        GetRepeatedPattern("P_20", 30),
                        GetRepeatedPattern("P_20", 30)
                    },
                    0
                ),
                (
                    new Vector3[] {
                        new Vector3(-2f, 4.6f, 0),
                        new Vector3(0f, 4.6f, 0),
                        new Vector3(1f, 4.6f, 0),
                        new Vector3(2f, 4.6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20", 30),
                        GetRepeatedPattern("P_20", 30),
                        GetRepeatedPattern("P_20", 30),
                        GetRepeatedPattern("P_20", 30)
                    },
                    0
                ),
                // 그룹 B: 인덱스 5~9 (5회 반복)
                (
                    new Vector3[] { new Vector3(-1f, 4.6f, 0) },
                    new string[][] { GetRepeatedPattern("P_20", 30) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-1f, 4.6f, 0) },
                    new string[][] { GetRepeatedPattern("P_20", 30) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-1f, 4.6f, 0) },
                    new string[][] { GetRepeatedPattern("P_20", 30) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-1f, 4.6f, 0) },
                    new string[][] { GetRepeatedPattern("P_20", 30) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(-1f, 4.6f, 0) },
                    new string[][] { GetRepeatedPattern("P_20", 30) },
                    0
                )
            }
        },
        // 웨이브 23
        { 23, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                (
                    new Vector3[] {
                        new Vector3(-2f, 4.5f, 0),
                        new Vector3(-2f, 3.5f, 0),
                        new Vector3(-2f, 2.5f, 0),
                        new Vector3(-1f, 4.5f, 0),
                        new Vector3(-1f, 3.5f, 0),
                        new Vector3(0f, 4.5f, 0),
                        new Vector3(1f, 4.5f, 0),
                        new Vector3(1f, 3.5f, 0),
                        new Vector3(2f, 4.5f, 0),
                        new Vector3(2f, 3.5f, 0),
                        new Vector3(2f, 2.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20", 1),
                        GetRepeatedPattern("P_20", 1),
                        GetRepeatedPattern("P_20", 1),
                        GetRepeatedPattern("P_20", 1),
                        GetRepeatedPattern("P_20", 1),
                        GetRepeatedPattern("P_20", 1),
                        GetRepeatedPattern("P_20", 1),
                        GetRepeatedPattern("P_20", 1),
                        GetRepeatedPattern("P_20", 1),
                        GetRepeatedPattern("P_20", 1),
                        GetRepeatedPattern("P_20", 1)
                    },
                    0
                )
            }
        },
        // 웨이브 24
        { 24, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 그룹 A: Enemy_2 at (0,1.5) – 5회 반복
                (
                    new Vector3[] { new Vector3(0f, 1.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_16", 30) },
                    2
                ),
                (
                    new Vector3[] { new Vector3(0f, 1.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_16", 30) },
                    2
                ),
                (
                    new Vector3[] { new Vector3(0f, 1.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_16", 30) },
                    2
                ),
                (
                    new Vector3[] { new Vector3(0f, 1.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_16", 30) },
                    2
                ),
                (
                    new Vector3[] { new Vector3(0f, 1.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_16", 30) },
                    2
                ),
                // 그룹 B: Enemy_1 at (0,4.5) – 5회 반복
                (
                    new Vector3[] { new Vector3(0f, 4.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_17", 30) },
                    1
                ),
                (
                    new Vector3[] { new Vector3(0f, 4.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_17", 30) },
                    1
                ),
                (
                    new Vector3[] { new Vector3(0f, 4.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_17", 30) },
                    1
                ),
                (
                    new Vector3[] { new Vector3(0f, 4.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_17", 30) },
                    1
                ),
                (
                    new Vector3[] { new Vector3(0f, 4.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_17", 30) },
                    1
                ),
                // 그룹 C: Enemy_0 at (0,2.5) – 5회 반복
                (
                    new Vector3[] { new Vector3(0f, 2.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_17", 30) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(0f, 2.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_17", 30) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(0f, 2.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_17", 30) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(0f, 2.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_17", 30) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(0f, 2.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_17", 30) },
                    0
                ),
                // 그룹 D: Enemy_0 at (0,3.5) – 5회 반복
                (
                    new Vector3[] { new Vector3(0f, 3.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_16", 30) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(0f, 3.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_16", 30) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(0f, 3.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_16", 30) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(0f, 3.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_16", 30) },
                    0
                ),
                (
                    new Vector3[] { new Vector3(0f, 3.5f, 0) },
                    new string[][] { GetRepeatedPattern("P_16", 30) },
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
    // 주어진 패턴 배열을 times번 반복한 결과를 반환하는 헬퍼 함수
    private static string[] RepeatPattern(string[] pattern, int times)
    {
        List<string> list = new List<string>();
        for (int i = 0; i < times; i++)
        {
            list.AddRange(pattern);
        }
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
                Debug.Log($"Starting Wave {currentWave}");

                if (waveData.ContainsKey(currentWave))
                {
                    var waveSteps = waveData[currentWave];
                    // 웨이브 3
                    if (currentWave == 3)
                    {
                        foreach (var step in waveSteps)
                        {
                            yield return StartCoroutine(SpawnEnemies(step.positions, step.patterns, step.enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                    }
                    // 웨이브 6
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
                    // 웨이브 7
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
                    // 웨이브 8
                    else if (currentWave == 8)
                    {
                        // Wave 8: 이벤트 1 바로 스폰, 이후 5초 대기, 그리고 이벤트 2와 3 스폰 (연달아)
                        yield return StartCoroutine(SpawnEnemies(waveSteps[0].positions, waveSteps[0].patterns, waveSteps[0].enemyType));
                        yield return new WaitForSeconds(5f);
                        yield return StartCoroutine(SpawnEnemies(waveSteps[1].positions, waveSteps[1].patterns, waveSteps[1].enemyType));
                        yield return StartCoroutine(SpawnEnemies(waveSteps[2].positions, waveSteps[2].patterns, waveSteps[2].enemyType));
                    }
                    // 웨이브 9
                    else if (currentWave == 9)
                    {
                        // Wave 9: 각 스폰 이벤트 후 1초 간격으로 진행
                        foreach (var step in waveSteps)
                        {
                            yield return StartCoroutine(SpawnEnemies(step.positions, step.patterns, step.enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                    }
                    // 웨이브 10
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
                    // 웨이브 11
                    else if (currentWave == 11)
                    {
                        waveSteps = waveData[currentWave];
                        // 그룹 A: 인덱스 0 ~ 4, 1초 간격
                        for (int i = 0; i < 5; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                        // 그룹 B: 5초 대기 후, 인덱스 5 ~ 9, 1초 간격
                        yield return new WaitForSeconds(5f);
                        for (int i = 5; i < 10; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                    }
                    // 웨이브 12
                    else if (currentWave == 12)
                    {
                        waveSteps = waveData[currentWave];
                        // 그룹 A: 인덱스 0 ~ 4, 1초 간격
                        for (int i = 0; i < 5; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                    }
                    // 웨이브 14
                    else if (currentWave == 14)
                    {
                        waveSteps = waveData[currentWave];
                        // 그룹 0: Enemy_1 그룹 즉시 스폰
                        yield return StartCoroutine(SpawnEnemies(waveSteps[0].positions, waveSteps[0].patterns, waveSteps[0].enemyType));
                        // 그룹 1: Enemy_0 그룹 1 스폰
                        yield return StartCoroutine(SpawnEnemies(waveSteps[1].positions, waveSteps[1].patterns, waveSteps[1].enemyType));
                        yield return new WaitForSeconds(2f);
                        // 그룹 2: Enemy_0 그룹 2 스폰
                        yield return StartCoroutine(SpawnEnemies(waveSteps[2].positions, waveSteps[2].patterns, waveSteps[2].enemyType));
                    }
                    // 웨이브 15
                    else if (currentWave == 15)
                    {
                        waveSteps = waveData[currentWave];
                        // 그룹 A: 인덱스 0 ~ 4, 1초 간격
                        for (int i = 0; i < 5; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                        // 그룹 B: 그룹 A 완료 후 2초 대기
                        yield return new WaitForSeconds(2f);
                        // 그룹 B: 인덱스 5 ~ 9, 1초 간격
                        for (int i = 5; i < 10; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                    }
                    // 웨이브 16
                    else if (currentWave == 16)
                    {
                        waveSteps = waveData[currentWave];
                        // 그룹 A: 즉시 (t=0)
                        yield return StartCoroutine(SpawnEnemies(waveSteps[0].positions, waveSteps[0].patterns, waveSteps[0].enemyType));
                        yield return new WaitForSeconds(2f);
                        // 그룹 B: (t=2)
                        yield return StartCoroutine(SpawnEnemies(waveSteps[1].positions, waveSteps[1].patterns, waveSteps[1].enemyType));
                        yield return new WaitForSeconds(1f);
                        // 그룹 C: (t=3)
                        yield return StartCoroutine(SpawnEnemies(waveSteps[2].positions, waveSteps[2].patterns, waveSteps[2].enemyType));
                        yield return new WaitForSeconds(1f);
                        // 그룹 D: (t=4) – 첫 번째 Enemy_0 스폰
                        yield return StartCoroutine(SpawnEnemies(waveSteps[3].positions, waveSteps[3].patterns, waveSteps[3].enemyType));
                        // 동시에 또는 바로 이어서 그룹 E: (t=4) – Enemy_2 스폰
                        yield return StartCoroutine(SpawnEnemies(waveSteps[4].positions, waveSteps[4].patterns, waveSteps[4].enemyType));
                    }
                    // 웨이브 17
                    else if (currentWave == 17)
                    {
                        waveSteps = waveData[currentWave];
                        // 그룹 A: 즉시 스폰
                        yield return StartCoroutine(SpawnEnemies(waveSteps[0].positions, waveSteps[0].patterns, waveSteps[0].enemyType));
                        // 그룹 B: 5초 후에 스폰
                        yield return new WaitForSeconds(5f);
                        yield return StartCoroutine(SpawnEnemies(waveSteps[1].positions, waveSteps[1].patterns, waveSteps[1].enemyType));
                    }
                    // 웨이브 18
                    else if (currentWave == 18)
                    {
                        waveSteps = waveData[currentWave];
                        // 그룹 A: 인덱스 0 ~ 4, 1초 간격으로 스폰
                        for (int i = 0; i < 5; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                        // 그룹 F: 웨이브 시작 5초 후
                        yield return new WaitForSeconds(5f);
                        yield return StartCoroutine(SpawnEnemies(waveSteps[5].positions, waveSteps[5].patterns, waveSteps[5].enemyType));
                        // 그룹 G: 3초 후
                        yield return new WaitForSeconds(3f);
                        yield return StartCoroutine(SpawnEnemies(waveSteps[6].positions, waveSteps[6].patterns, waveSteps[6].enemyType));
                    }
                    // 웨이브 19
                    else if (currentWave == 19)
                    {
                        waveSteps = waveData[currentWave];
                        // 그룹 A ~ D: 각각 한 번에 2마리씩 스폰, 1초 간격
                        for (int i = 0; i < 4; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                        // 그룹 E: 4마리를 1개씩 순차적으로 스폰 (1초 간격)
                        Vector3[] groupEPositions = waveSteps[4].positions;
                        string[][] groupEPatterns = waveSteps[4].patterns;
                        for (int i = 0; i < groupEPositions.Length; i++)
                        {
                            yield return StartCoroutine(SpawnEnemyIndividual(groupEPositions[i], groupEPatterns[i], waveSteps[4].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                    }
                    // 웨이브 20
                    else if (currentWave == 20)
                    {
                        waveSteps = waveData[currentWave];
                        // 그룹 A: 인덱스 0 ~ 4, 1초 간격
                        for (int i = 0; i < 5; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                        // 그룹 B: 인덱스 5 ~ 9, 그룹 A 완료 후 2초 대기 후 시작, 1초 간격
                        yield return new WaitForSeconds(2f);
                        for (int i = 5; i < 10; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                        // 그룹 C: 인덱스 10 ~ 14, 그룹 B 완료 후 2초 대기 후 시작, 1초 간격
                        yield return new WaitForSeconds(2f);
                        for (int i = 10; i < 15; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                        // 그룹 D: 인덱스 15, 그룹 C 완료 후 3초 대기 후 시작
                        yield return new WaitForSeconds(3f);
                        yield return StartCoroutine(SpawnEnemies(waveSteps[15].positions, waveSteps[15].patterns, waveSteps[15].enemyType));
                    }
                    // 웨이브 21
                    else if (currentWave == 21)
                    {
                        waveSteps = waveData[currentWave];
                        // 그룹 1: 웨이브 시작 0초 – 3개의 스폰 이벤트 (각 1초 간격)
                        for (int i = 0; i < 3; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                        // 그룹 2: 웨이브 시작 5초 후 – 3번 반복, 매 번 2마리 동시에 스폰 (1초 간격)
                        yield return new WaitForSeconds(5f);
                        for (int i = 3; i < 6; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                    }
                    // 웨이브 22
                    else if (currentWave == 22)
                    {
                        waveSteps = waveData[currentWave];
                        // 그룹 B (인덱스 0~4): 웨이브 시작 0초부터, 1초 간격으로 스폰
                        for (int i = 0; i < 5; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                        // 그룹 C (인덱스 5~9): 웨이브 시작 1.5초 후, 1초 간격으로 스폰
                        yield return new WaitForSeconds(1.5f);
                        for (int i = 5; i < 10; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                    }
                    // 웨이브 23
                    else if (currentWave == 23)
                    {
                        waveSteps = waveData[currentWave];
                        yield return StartCoroutine(SpawnEnemies(waveSteps[0].positions, waveSteps[0].patterns, waveSteps[0].enemyType));
                    }
                    // 웨이브 24
                    else if (currentWave == 24)
                    {
                        waveSteps = waveData[currentWave];
                        // 그룹 A: 인덱스 0 ~ 4, 1초 간격으로 스폰 (Enemy_2)
                        for (int i = 0; i < 5; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                        // 그룹 B: 인덱스 5 ~ 9, 1초 간격으로 스폰 (Enemy_1)
                        for (int i = 5; i < 10; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                        // 그룹 C: 인덱스 10 ~ 14, 그룹 A/B 완료 후 2초 대기 후 1초 간격 (Enemy_0 at (0,2.5))
                        yield return new WaitForSeconds(2f);
                        for (int i = 10; i < 15; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                        // 그룹 D: 인덱스 15 ~ 19, 그룹 C 완료 후 2초 대기 후 1초 간격 (Enemy_0 at (0,3.5))
                        yield return new WaitForSeconds(2f);
                        for (int i = 15; i < 20; i++)
                        {
                            yield return StartCoroutine(SpawnEnemies(waveSteps[i].positions, waveSteps[i].patterns, waveSteps[i].enemyType));
                            yield return new WaitForSeconds(1f);
                        }
                        AudioManager.Instance.UpdateWave(currentWave); // BGM3번으로 전환

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

                Debug.Log($"Wave {currentWave} complete. Waiting {waveDelay} seconds before next wave...");
                if (currentWave < 25 && BossGaugeManager.Instance != null)
                {
                    float increment = BossGaugeManager.Instance.maxGauge / 24f;
                    BossGaugeManager.Instance.AddGauge(increment);
                }
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
        //Debug.Log($"적 {enemy.name} - 패턴 실행 시작: {string.Join(", ", patterns)}");
        yield return StartCoroutine(PatternManager.Instance.ExecutePattern(enemy, patterns));
        if (enemy != null)
        {
            //Debug.Log($"적 {enemy.name} - 패턴 실행 완료");
        }
    }
    IEnumerator SpawnEnemyIndividual(Vector3 position, string[] pattern, int enemyType)
    {
        GameObject prefabToUse = (enemyType == 0) ? enemyPrefab : (enemyType == 1 ? enemyPrefab1 : enemyPrefab2);
        GameObject enemy = Instantiate(prefabToUse, position, Quaternion.identity);
        if (enemy == null)
        {
            Debug.LogError("적 생성 실패! prefab이 올바르게 설정되어 있는지 확인하세요.");
            yield break;
        }
        StartCoroutine(ExecutePattern(enemy, pattern));
        yield return null;
    }

}
