using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }
    public float waveDelay = 10f;      // 웨이브 간 딜레이
    private int currentWave = 1;      // 현재 웨이브 번호
    public int CurrentWave { get { return currentWave; } }  // 외부 접근용 프로퍼티
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
                // 첫 번째 스폰: 0초에 좌표 {(-2,6), (0,6), (2,6)} – 각 "P_0" 5회 실행, enemyType 0
                (
                    new Vector3[] {
                        new Vector3(-2, 6f, 0),
                        new Vector3(0, 6f, 0),
                        new Vector3(2, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 5),
                        GetRepeatedPattern("P_0", 5),
                        GetRepeatedPattern("P_0", 5)
                    },
                    0
                ),
                // 두 번째 스폰: 3초에 좌표 {(-1,6), (-1,6), (1,6)} – 각 "P_0" 4회 실행, enemyType 0
                (
                    new Vector3[] {
                        new Vector3(-1, 6f, 0),
                        new Vector3(-1, 6f, 0),
                        new Vector3(1, 6f, 0),
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 4),
                        GetRepeatedPattern("P_0", 4),
                        GetRepeatedPattern("P_0", 4)
                    },
                    0
                )
            }
        },
        // 웨이브 2
        { 2, new List<(Vector3[], string[][], int)>()
            {
                // 첫 번째 스폰: 0초에 좌표 {(-2,6), (0,6), (2,6)} – 각 "P_0" 5회 실행, enemyType 1
                (
                    new Vector3[] {
                        new Vector3(-2, 6f, 0),
                        new Vector3(0, 6f, 0),
                        new Vector3(2, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 5),
                        GetRepeatedPattern("P_0", 5),
                        GetRepeatedPattern("P_0", 5)
                    },
                    1
                ),
                // 두 번째 스폰: 3초에 좌표 {(-1,6), (1,6)} – 각 "P_0" 4회 실행, enemyType 0
                (
                    new Vector3[] {
                        new Vector3(-1, 6f, 0),
                        new Vector3(1, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 4),
                        GetRepeatedPattern("P_0", 4)
                    },
                    0
                )
            }
        },
        // 웨이브 3
        { 3, new List<(Vector3[], string[][], int)>()
            {
                // 첫번째 스폰: 0초에 좌표 {(-1,6), (1,6)} - 각 "P_0" 4회 실행, 4초 후에 "P_1" 1회 실행, "P_25R_boss" 1회 실행, enemyType 1
                (
                    new Vector3[] {
                        new Vector3(-1, 6f, 0),
                        new Vector3(1, 6f, 0)
                    },
                    new string[][] {
                        // 패턴: N_0 4회 → N_1 1회 → N_25 1회 → N_25R 1회
                        ConcatPatterns(
                            ConcatPatterns( GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_1", 1) ),
                            GetRepeatedPattern("P_25R", 1)
                        ),
                        // 패턴: N_0 4회 → N_1 1회 → N_25 1회 → N_25R 1회
                        ConcatPatterns(
                            ConcatPatterns( GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_1", 1) ),
                            GetRepeatedPattern("P_25R", 1)
                        )
                    },
                    1
                ),
                // 두번째 스폰: 3초에 좌표{(0,6)} - "P_0" 2회 실행, "P_16" 무한 실행, enemyType 0
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        // 패턴: N_0 2회 -> P_16 25회
                        ConcatPatterns( GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_16", 25) )

                    },
                    1
                ),
                // 세번째 스폰: 3.5초에 좌표 {(0,6)}
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        // 패턴: N_0 2회 -> P_16 25회
                        ConcatPatterns( GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_16", 25) )

                    },
                    1
                ),
                // 네번째 스폰: 4초에 좌표 {(0,6)}
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        // 패턴: N_0 2회 -> P_16 25회
                        ConcatPatterns( GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_16", 25) )

                    },
                    1
                ),
                // 다섯번째 스폰: 4.5초에 좌표 {(0,6)}
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        // 패턴: N_0 2회 -> P_16 25회
                        ConcatPatterns( GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_16", 25) )

                    },
                    1
                ),
                // 여섯번째 스폰: 5초에 좌표 {(0,6)}
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        // 패턴: N_0 2회 -> P_16 25회
                        ConcatPatterns( GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_16", 25) )

                    },
                    1
                ),
            }
        },
        // 웨이브 4
        { 4, new List<(Vector3[], string[][], int)>()
            {
                // 첫번째 스폰: 0초에 좌표 {(0,6), (3,6), (-3,2)} - 각각 "P_0" 20회 실행, enemyType 1, "P_28" 25회 실행, "P_29" 25회 실행, enemyType 0 //0
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        // 패턴: N_0 20회
                        GetRepeatedPattern("P_0", 20)
                    },
                    1
                ),
                //1
                (
                    new Vector3[] {
                        new Vector3(3, 2f, 0),
                        new Vector3(-3, 2f, 0)
                    },
                    new string[][] {
                        // 패턴: N_28,29 25회
                        GetRepeatedPattern("P_28", 25),
                        GetRepeatedPattern("P_29", 25)
                    },
                    0
                ),
                // 두번째 스폰 : 0.5초에 좌표 {(3,2), (-3,2)} - 각 "P_28", "P_29" 25회 실행, enemyType 0 // 2
                (
                    new Vector3[] {
                        new Vector3(3, 2f, 0),
                        new Vector3(-3, 2f, 0)
                    },
                    new string[][] {
                        // 패턴: N_28,29 25회
                        GetRepeatedPattern("P_28", 25),
                        GetRepeatedPattern("P_29", 25)
                    },
                    0
                ),
                // 3
                (
                    new Vector3[] {
                        new Vector3(3, 2f, 0),
                        new Vector3(-3, 2f, 0)
                    },
                    new string[][] {
                        // 패턴: N_28,29 25회
                        GetRepeatedPattern("P_28", 25),
                        GetRepeatedPattern("P_29", 25)
                    },
                    0
                ),
                // 4
                (
                    new Vector3[] {
                        new Vector3(3, 2f, 0),
                        new Vector3(-3, 2f, 0)
                    },
                    new string[][] {
                        // 패턴: N_28,29 25회
                        GetRepeatedPattern("P_28", 25),
                        GetRepeatedPattern("P_29", 25)
                    },
                    0
                ),
                // 5
                (
                    new Vector3[] {
                        new Vector3(3, 2f, 0),
                        new Vector3(-3, 2f, 0)
                    },
                    new string[][] {
                        // 패턴: N_28,29 25회
                        GetRepeatedPattern("P_28", 25),
                        GetRepeatedPattern("P_29", 25)
                    },
                    0
                ),
                // 6
                (
                    new Vector3[] {
                        new Vector3(3, 2f, 0),
                        new Vector3(-3, 2f, 0)
                    },
                    new string[][] {
                        // 패턴: N_28,29 25회
                        GetRepeatedPattern("P_28", 25),
                        GetRepeatedPattern("P_29", 25)
                    },
                    0
                )
            }
        },
        // 웨이브 5
        { 5, new List<(Vector3[], string[][], int)>()
            {
                // 첫 번째 스폰: enemyType 1
                // 좌표: {-2,6, -1,6, 0,6, 1,6, 2,6}
                (
                    new Vector3[] {
                        new Vector3(-2, 4.6f, 0),
                        new Vector3(-1, 4.6f, 0),
                        new Vector3(0, 4.6f, 0),
                        new Vector3(1, 4.6f, 0),
                        new Vector3(2, 4.6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 5),
                        ConcatPatterns(GetRepeatedPattern("P_0", 2),ConcatPatterns(GetRepeatedPattern("P_1", 1), GetRepeatedPattern("P_25R_boss", 1))),
                        GetRepeatedPattern("P_0", 5),
                        ConcatPatterns(GetRepeatedPattern("P_0", 5),ConcatPatterns(GetRepeatedPattern("P_1", 1), GetRepeatedPattern("P_25R_boss", 1))),
                        GetRepeatedPattern("P_0", 5)
                    },
                    1
                ),
                // 두 번째 스폰: enemyType 2
                // 좌표: {-1,6, 1,6}
                (
                    new Vector3[] {
                        new Vector3(-1, 6f, 0),
                        new Vector3(1, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 2),
                        GetRepeatedPattern("P_0", 2)
                    },
                    2
                ),
                // 세 번째 스폰: enemyType 0
                // 좌표: {-0.5,6, 0,6, 0.5,6}
                (
                    new Vector3[] {
                        new Vector3(-0.5f, 6f, 0),
                        new Vector3(0, 6f, 0),
                        new Vector3(0.5f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 4),GetRepeatedPattern("P_3RL", 1)),
                        ConcatPatterns(GetRepeatedPattern("P_0", 4),GetRepeatedPattern("P_3RL", 1)),
                        ConcatPatterns(GetRepeatedPattern("P_0", 4),GetRepeatedPattern("P_3RL", 1))
                    },
                    0
                ),
            }
        },
        // 웨이브 6
        { 6, new List<(Vector3[], string[][], int)>()
            {
                // Group A: Enemy_3 7마리 동시에, 위치 (3,6), (2,6), (1,6), (0,6), (-1,6), (-2,6), (3,6)
                (
                    new Vector3[] {
                        new Vector3(3, 6f, 0),
                        new Vector3(2, 6f, 0),
                        new Vector3(1, 6f, 0),
                        new Vector3(0, 6f, 0),
                        new Vector3(-1, 6f, 0),
                        new Vector3(-2, 6f, 0),
                        new Vector3(-3, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_5", 25),
                        GetRepeatedPattern("P_5", 25),
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_4", 25),
                        GetRepeatedPattern("P_4", 25),
                    },
                    3
                ),
                // Group B: Enemy_3 6마리 동시에, 위치 (3,4), (3,3), (3,2), (-3,2), (-3,3), (-3,4)
                (
                    new Vector3[] {
                        new Vector3(3, 4f, 0),
                        new Vector3(3, 3f, 0),
                        new Vector3(3, 2f, 0),
                        new Vector3(-3, 2f, 0),
                        new Vector3(-3, 3f, 0),
                        new Vector3(-3, 4f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_5", 25),
                        GetRepeatedPattern("P_9", 25),
                        GetRepeatedPattern("P_9", 25),
                        GetRepeatedPattern("P_8", 25),
                        GetRepeatedPattern("P_8", 25),
                        GetRepeatedPattern("P_4", 25)
                    },
                    3
                ),
                // Group C: Enemy_3 5마리 동시에, 위치 (-2,6), (-1,6), (0,6), (1,6), (2,6)
                (
                    new Vector3[] {
                        new Vector3(-2, 6f, 0),
                        new Vector3(-1, 6f, 0),
                        new Vector3(0, 6f, 0),
                        new Vector3(1, 6f, 0),
                        new Vector3(2, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25)
                    },
                    3
                ),
                // Group D: Enemy_3 5마리 동시에, 위치 (-2,6), (-1,6), (0,6), (1,6), (2,6)
                (
                    new Vector3[] {
                        new Vector3(-2, 6f, 0),
                        new Vector3(-1, 6f, 0),
                        new Vector3(0, 6f, 0),
                        new Vector3(1, 6f, 0),
                        new Vector3(2, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25)
                    },
                    3
                )
            }
        },
        // 웨이브 7
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
        // 웨이브 8
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
        // 웨이브 9
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
        // 웨이브 10
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
        // 웨이브 11
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
        // 웨이브 12
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
        // 웨이브 13
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
        // 웨이브 14
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
        // 웨이브 15
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
                    new Vector3[] { new Vector3(-2f, 4.5f, 0), new Vector3(-1f, 4.5f, 0), new Vector3(2f, 4.5f, 0), new Vector3(1f, 4.5f, 0) },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_4", 3), GetRepeatedPattern("P_1", 1)),
                        ConcatPatterns(GetRepeatedPattern("P_4", 3), GetRepeatedPattern("P_1", 1)),
                        ConcatPatterns(GetRepeatedPattern("P_5", 3), GetRepeatedPattern("P_1", 1)),
                        ConcatPatterns(GetRepeatedPattern("P_5", 3), GetRepeatedPattern("P_1", 1))
                    },
                    0
                ),
                // 그룹 B: 웨이브 시작 2초 후에 Enemy_1가 (-1, -1), (1, -1)에서 스폰
                (
                    new Vector3[] { new Vector3(-1f, -1f, 0), new Vector3(1f, -1f, 0) },
                    new string[][] {
                        GetRepeatedPattern("P_16", 25),
                        GetRepeatedPattern("P_17", 25),
                    },
                    1
                ),
                // 그룹 C: 웨이브 시작 3초 후에 Enemy_1가 (-1, -1), (1, -1)에서 스폰
                (
                    new Vector3[] { new Vector3(-1f, -1f, 0), new Vector3(1f, -1f, 0) },
                    new string[][] {
                        GetRepeatedPattern("P_16", 25),
                        GetRepeatedPattern("P_17", 25),
                    },
                    1
                ),
                // 그룹 D: 웨이브 시작 4초 후에 Enemy_1가 (-1, -1), (1, -1)에서 스폰
                (
                    new Vector3[] { new Vector3(-1f, -1f, 0), new Vector3(1f, -1f, 0) },
                    new string[][] {
                        GetRepeatedPattern("P_16", 25),
                        GetRepeatedPattern("P_17", 25),
                    },
                    1
                ),
                // 그룹 E: 웨이브 시작 5초 후에 Enemy_1가 (-1, -1), (1, -1)에서 스폰
                (
                    new Vector3[] { new Vector3(-1f, -1f, 0), new Vector3(1f, -1f, 0) },
                    new string[][] {
                        GetRepeatedPattern("P_16", 25),
                        GetRepeatedPattern("P_17", 25),
                    },
                    1
                ),
                // 그룹 F: 웨이브 시작 6초 후에 Enemy_1가 (-1, -1), (1, -1)에서 스폰, (0, 4.6)
                (
                    new Vector3[] { new Vector3(-1f, -1f, 0), new Vector3(1f, -1f, 0) },
                    new string[][] {
                        GetRepeatedPattern("P_16", 25),
                        GetRepeatedPattern("P_17", 25),
                    },
                    1
                ),
                (
                    new Vector3[] { new Vector3(0, 4.6f, 0)},
                    new string[][] {
                        GetRepeatedPattern("P_0", 4),
                    },
                    0
                ),
                // 그룹 G: 웨이브 시작 7초 후에 Enemy_1가 (-2, -2.5), (-2, -2.5), (2, -2.5), (2, -2.5)에서 스폰, (0, 4.6)
                (
                    new Vector3[] { new Vector3(-2f, -2.5f, 0), new Vector3(-2f, -2.5f, 0), new Vector3(2f, -2.5f, 0), new Vector3(2f, -2.5f, 0), },
                    new string[][] {
                        GetRepeatedPattern("P_2", 1),
                        new string[0],
                        GetRepeatedPattern("P_3", 1),
                        new string[0]
                    },
                    1
                ),
                (
                    new Vector3[] { new Vector3(0, 4.6f, 0)},
                    new string[][] {
                        GetRepeatedPattern("P_0", 3),
                    },
                    0
                ),
                // 그룹 H: 웨이브 시작 8초 후에 Enemy_0이 (0, 4.6) 에서 스폰
                (
                    new Vector3[] { new Vector3(0, 4.6f, 0)},
                    new string[][] {
                        GetRepeatedPattern("P_0", 2),
                    },
                    0
                ),
                // 그룹 I: 웨이브 시작 9초 후에 Enemy_0이 (0, 4.6) 에서 스폰
                (
                    new Vector3[] { new Vector3(0, 4.6f, 0)},
                    new string[][] {
                        GetRepeatedPattern("P_0", 1),
                    },
                    0
                ),
                // 그룹 J: 웨이브 시작 10초 후에 Enemy_0이 (0, 4.6) 에서 스폰
                (
                    new Vector3[] { new Vector3(0, 4.6f, 0)},
                    new string[][] {
                        new string[0]
                    },
                    0
                ),
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
        // 웨이브 25
        { 25, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                ( // 일단 버그 땜에 하나 추가
                // 스폰 좌표: (0,4.6)
                new Vector3[] { new Vector3(0f, 4.6f, 0f) },
                // 패턴: "P_0" 한 번
                new string[][] { new string[] { "P_0" } },
                // enemyType: 0 (Enemy_0)
                0
            ),
            }
        }

    };

    // 헬퍼 메서드: 지정한 패턴을 count만큼 반복하는 배열을 생성
    private static string[] GetRepeatedPattern(string pattern, int count)
    {
        string[] result = new string[count];
        for (int i = 0; i < count; i++)
            result[i] = pattern;
        return result;
    }

    private static string[] ConcatPatterns(string[] a, string[] b)
    {
        string[] result = new string[a.Length + b.Length];
        a.CopyTo(result, 0);
        b.CopyTo(result, a.Length);
        return result;
    }

    private static string[] RepeatPattern(string[] patternGroup, int times)
    {
        List<string> result = new List<string>();
        for (int i = 0; i < times; i++)
        {
            result.AddRange(patternGroup);
        }
        return result.ToArray();
    }
    void Awake()
    {
        // 싱글턴 패턴 적용: 인스턴스가 없으면 자신을 할당, 이미 있으면 파괴
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    void Start()
    {
        StartCoroutine(StartNextWave());
    }
    private IEnumerator StartNextWave()
    {
        while (true)
        {
            if (!waveInProgress)
            {
                waveInProgress = true;
                yield return StartCoroutine(ManageWaves(currentWave));
                currentWave++;
                yield return new WaitForSeconds(waveDelay);
                waveInProgress = false;
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator ManageWaves(int waveNumber)
    {
        if (!waveData.ContainsKey(waveNumber))
        {
            Debug.Log("Wave " + waveNumber + " is not defined.");
            yield break;
        }

        var waveEvents = waveData[waveNumber];

        for (int i = 0; i < waveEvents.Count; i++)
        {
            var (spawnPositions, spawnPatterns, type) = waveEvents[i];

            // 지연 처리 (Wave마다 커스텀 딜레이 반영)
            if (waveNumber == 1 && i == 1) yield return new WaitForSeconds(3f);
            if (waveNumber == 2 && i == 1) yield return new WaitForSeconds(3f);
            if (waveNumber == 3 && i == 1) yield return new WaitForSeconds(3f);
            if (waveNumber == 3 && i == 2) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 3 && i == 3) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 3 && i == 4) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 3 && i == 5) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 4 && i == 2) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 4 && i == 3) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 4 && i == 4) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 4 && i == 5) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 4 && i == 6) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 5 && i == 1) yield return new WaitForSeconds(3f);
            if (waveNumber == 6 && i == 1) yield return new WaitForSeconds(3f);
            if (waveNumber == 6 && i == 2) yield return new WaitForSeconds(3f);
            if (waveNumber == 6 && i == 3) yield return new WaitForSeconds(2f);
            if (waveNumber == 7 && i == 1) yield return new WaitForSeconds(2f);
            if (waveNumber == 7 && i == 2) yield return new WaitForSeconds(5f);
            if (waveNumber == 7 && i == 3) yield return new WaitForSeconds(2f);
            if (waveNumber == 8 && i == 1) yield return new WaitForSeconds(5f);
            if (waveNumber == 8 && i == 2) yield return new WaitForSeconds(5f);
            if (waveNumber == 16 && i == 1) yield return new WaitForSeconds(2f);
            if (waveNumber == 16 && i == 2) yield return new WaitForSeconds(1f);
            if (waveNumber == 16 && i == 3) yield return new WaitForSeconds(1f);
            if (waveNumber == 16 && i == 4) yield return new WaitForSeconds(0f);
            if (waveNumber == 18 && i == 1) yield return new WaitForSeconds(1f);
            if (waveNumber == 18 && i == 2) yield return new WaitForSeconds(1f);
            if (waveNumber == 18 && i == 3) yield return new WaitForSeconds(1f);
            if (waveNumber == 18 && i == 4) yield return new WaitForSeconds(1f);
            if (waveNumber == 18 && i == 5) yield return new WaitForSeconds(1f);
            if (waveNumber == 18 && i == 6) yield return new WaitForSeconds(3f);
            if (waveNumber == 19 && i == 1) yield return new WaitForSeconds(1f);
            if (waveNumber == 19 && i == 2) yield return new WaitForSeconds(4f);
            if (waveNumber == 19 && i == 3) yield return new WaitForSeconds(1f);

            if (waveNumber == 19 && i == 4)
            {
                for (int k = 0; k < 4; k++)
                {
                    string enemyTypeName = "Enemy_" + type;
                    GameObject enemy = Pool.Instance.SpawnEnemy(enemyTypeName, spawnPositions[k], Quaternion.identity);

                    if (enemy != null)
                    {
                        StartCoroutine(PatternManager.Instance.ExecutePattern(enemy, spawnPatterns[k]));
                    }
                    yield return new WaitForSeconds(1f);
                }
                continue;
            }

            for (int j = 0; j < spawnPositions.Length; j++)
            {
                string enemyTypeName = "Enemy_" + type;
                GameObject enemy = Pool.Instance.SpawnEnemy(enemyTypeName, spawnPositions[j], Quaternion.identity);

                if (enemy != null)
                {
                    StartCoroutine(PatternManager.Instance.ExecutePattern(enemy, spawnPatterns[j]));
                }
            }
        }
    }
}
