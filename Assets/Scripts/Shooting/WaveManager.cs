using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }
    private int currentWave = 1;      // 현재 웨이브 번호
    public int CurrentWave { get { return currentWave; } }  // 외부 접근용 프로퍼티

    // waveData: 각 웨이브의 스폰 이벤트 목록
    // 각 스폰 이벤트는 (positions, patterns, enemyType) 튜플로 구성
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
                            ConcatPatterns( ConcatPatterns(GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_24", 4)), GetRepeatedPattern("P_1", 1) ),
                            GetRepeatedPattern("P_25R", 1)
                        ),
                        // 패턴: N_0 4회 → N_1 1회 → N_25 1회 → N_25R 1회
                        ConcatPatterns(
                            ConcatPatterns( ConcatPatterns(GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_24", 4)), GetRepeatedPattern("P_1", 1) ),
                            GetRepeatedPattern("P_25R", 1)
                        ),
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
                        ConcatPatterns( GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_16", 25) )

                    },
                    0
                ),
                // 세번째 스폰: 3.5초에 좌표 {(0,6)}
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        // 패턴: N_0 2회 -> P_16 25회
                        ConcatPatterns( GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_16", 25) )

                    },
                    0
                ),
                // 네번째 스폰: 4초에 좌표 {(0,6)}
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        // 패턴: N_0 2회 -> P_16 25회
                        ConcatPatterns( GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_16", 25) )

                    },
                    0
                ),
                // 다섯번째 스폰: 4.5초에 좌표 {(0,6)}
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        // 패턴: N_0 2회 -> P_16 25회
                        ConcatPatterns( GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_16", 25) )

                    },
                    0
                ),
                // 여섯번째 스폰: 5초에 좌표 {(0,6)}
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        // 패턴: N_0 2회 -> P_16 25회
                        ConcatPatterns( GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_16", 25) )

                    },
                    0
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
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 5),GetRepeatedPattern("P_24", 2)), ConcatPatterns(GetRepeatedPattern("P_1", 1), GetRepeatedPattern("P_25R_boss", 1))),
                        GetRepeatedPattern("P_0", 5),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 5),GetRepeatedPattern("P_24", 5)), ConcatPatterns(GetRepeatedPattern("P_1", 1), GetRepeatedPattern("P_25R_boss", 1))),
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
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)),GetRepeatedPattern("P_0", 25)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)),GetRepeatedPattern("P_0", 25)),
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)),GetRepeatedPattern("P_0", 25)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)),GetRepeatedPattern("P_0", 25))
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
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)),GetRepeatedPattern("P_0", 25)),
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
                // 첫 번째 스폰: (0,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 7)
                    },
                    4
                ),
                // 두 번째 스폰: (-1,6), (1,6)
                (
                    new Vector3[] {
                        new Vector3(-1f, 6f, 0),
                        new Vector3(1f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 6),
                        GetRepeatedPattern("P_0", 6)
                    },
                    4
                ),
                // 세 번째 스폰: (-2,6), (2,6)
                (
                    new Vector3[] {
                        new Vector3(-2f, 6f, 0),
                        new Vector3(2f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 5),
                        GetRepeatedPattern("P_0", 5)
                    },
                    4
                ),
                // 네 번째 스폰: (3,6)
                (
                    new Vector3[] {
                        new Vector3(3f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)), ConcatPatterns(GetRepeatedPattern("P_1",4), GetRepeatedPattern("P_4",25)))
                    },
                    3
                ),
                // 다섯 번째 스폰: (-1,6)
                (
                    new Vector3[] {
                        new Vector3(-1f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)), ConcatPatterns(GetRepeatedPattern("P_1",4), GetRepeatedPattern("P_5",25)))
                    },
                    3
                ),
                // 여섯 번째 스폰: (3,3)
                (
                    new Vector3[] {
                        new Vector3(3f, 3f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)), ConcatPatterns(GetRepeatedPattern("P_1",4), GetRepeatedPattern("P_4",25)))
                    },
                    4
                ),
                // 일곱 번째 스폰: (-3,2)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)), ConcatPatterns(GetRepeatedPattern("P_1",4), GetRepeatedPattern("P_5",25)))
                    },
                    3
                ),
                // 여덟 번째 스폰: (-2,6)
                (
                    new Vector3[] {
                        new Vector3(-2f, 6f, 0),
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)), ConcatPatterns(GetRepeatedPattern("P_1",4), GetRepeatedPattern("P_4",25))),
                    },
                    3
                ),
                // 아홉 번째 스폰: (3,2)
                (
                    new Vector3[] {
                        new Vector3(3f, 2f, 0),
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)), ConcatPatterns(GetRepeatedPattern("P_1",4), GetRepeatedPattern("P_5",25))),
                    },
                    4
                ),
                // 열 번째 스폰: (-3,6), (3,4)
                (
                    new Vector3[] {
                        new Vector3(-3f, 6f, 0),
                        new Vector3(3f, 4f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)), ConcatPatterns(GetRepeatedPattern("P_1",4), GetRepeatedPattern("P_4",25))),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)), ConcatPatterns(GetRepeatedPattern("P_1",4), GetRepeatedPattern("P_5",25))),
                    },
                    3
                ),
            }
        },
        // 웨이브 8
        { 8, new List<(Vector3[], string[][], int)>()
            {
                // 첫 번째 스폰: (3,2), (-3,2)
                (
                    new Vector3[] {
                        new Vector3(3f, 2f, 0),
                        new Vector3(-3f, 2f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_24", 2)), GetRepeatedPattern("P_0", 25)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_24", 2)), GetRepeatedPattern("P_0", 25))
                    },
                    4
                ),
                // 두 번째 스폰: (3,3), (-3,3)
                (
                    new Vector3[] {
                        new Vector3(3f, 3f, 0),
                        new Vector3(-3f, 3f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_24", 2)), GetRepeatedPattern("P_0", 25)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_24", 2)), GetRepeatedPattern("P_0", 25))
                    },
                    4
                ),
                // 세 번째 스폰: (3,4), (-3,4)
                (
                    new Vector3[] {
                        new Vector3(3f, 4f, 0),
                        new Vector3(-3f, 4f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_24", 2)), GetRepeatedPattern("P_0", 25)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_24", 2)), GetRepeatedPattern("P_0", 25))
                    },
                    4
                ),
                // 네 번째 스폰: (3,4.5)
                (
                    new Vector3[] {
                        new Vector3(3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_20D", 25))
                    },
                    4
                ),
                // 다섯 번째 스폰: (-3,4.5)
                (
                    new Vector3[] {
                        new Vector3(-3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_20D", 25))
                    },
                    4
                ),
                // 여섯 번째 스폰: (3,4.5)
                (
                    new Vector3[] {
                        new Vector3(3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_20D", 25))
                    },
                    4
                ),
                // 일곱 번째 스폰: (-3,4.5)
                (
                    new Vector3[] {
                        new Vector3(-3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_20D", 25))
                    },
                    4
                ),
                // 여덟 번째 스폰: (3,4.5)
                (
                    new Vector3[] {
                        new Vector3(3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_20D", 25))
                    },
                    4
                ),
                // 아홉 번째 스폰: (-3,4.5)
                (
                    new Vector3[] {
                        new Vector3(-3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_20D", 25))
                    },
                    4
                ),
                // 열 번째 스폰: (3,4.5)
                (
                    new Vector3[] {
                        new Vector3(3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_20D", 25))
                    },
                    4
                ),
                // 열하나 번째 스폰: (-3,4.5)
                (
                    new Vector3[] {
                        new Vector3(-3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_20D", 25))
                    },
                    4
                ),
                // 12 번째 스폰: (3,4.5)
                (
                    new Vector3[] {
                        new Vector3(3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_20D", 25))
                    },
                    4
                ),
                // 13 번째 스폰: (-3,4.5)
                (
                    new Vector3[] {
                        new Vector3(-3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_20D", 25))
                    },
                    4
                ),
                // 14 번째 스폰: (3,4.5)
                (
                    new Vector3[] {
                        new Vector3(3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_20D", 25))
                    },
                    4
                ),
                // 15 번째 스폰: (-3,4.5)
                (
                    new Vector3[] {
                        new Vector3(-3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_20D", 25))
                    },
                    4
                ),
                // 16 번째 스폰: (3,4.5)
                (
                    new Vector3[] {
                        new Vector3(3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_20D", 25))
                    },
                    4
                ),
                // 17 번째 스폰: (-3,4.5)
                (
                    new Vector3[] {
                        new Vector3(-3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_20D", 25))
                    },
                    4
                ),
                // 18 번째 스폰: (3,4.5)
                (
                    new Vector3[] {
                        new Vector3(3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_20D", 25))
                    },
                    4
                )
            }
        },
        // 웨이브 9
        { 9, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 첫 번째 스폰: (2,6), (-2,6)
                (
                    new Vector3[] {
                        new Vector3(2f, 6f, 0),
                        new Vector3(-2f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25)
                    },
                    3
                ),
                // 두 번째 스폰: (1,6), (-1,6)
                (
                    new Vector3[] {
                        new Vector3(1f, 6f, 0),
                        new Vector3(-1f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25)
                    },
                    3
                ),
                // 세 번째 스폰: (0,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 25)
                    },
                    3
                ),
                // 네 번째 스폰: (1,6), (-1,6)
                (
                    new Vector3[] {
                        new Vector3(1f, 6f, 0),
                        new Vector3(-1f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25)
                    },
                    3
                ),
                // 다섯 번째 스폰: (2,6), (-2,6)
                (
                    new Vector3[] {
                        new Vector3(2f, 6f, 0),
                        new Vector3(-2f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25)
                    },
                    3
                ),
                // 여섯 번째 스폰: (3,6)
                (
                    new Vector3[] {
                        new Vector3(3f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)),GetRepeatedPattern("P_0", 25))
                    },
                    3
                ),
                // 7 번째 스폰: (-3,6)
                (
                    new Vector3[] {
                        new Vector3(-3f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)),GetRepeatedPattern("P_0", 25))
                    },
                    3
                ),
                // 8 번째 스폰: (3,6)
                (
                    new Vector3[] {
                        new Vector3(3f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)),GetRepeatedPattern("P_0", 25))
                    },
                    3
                ),
                // 9 번째 스폰: (-3,6)
                (
                    new Vector3[] {
                        new Vector3(-3f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)),GetRepeatedPattern("P_0", 25))
                    },
                    3
                ),
                // 10 번째 스폰: (3,6), (-3,6)
                (
                    new Vector3[] {
                        new Vector3(3f, 6f, 0),
                        new Vector3(-3f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)),GetRepeatedPattern("P_0", 25)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)),GetRepeatedPattern("P_0", 25))
                    },
                    3
                ),
                // 11 번째 스폰: (3,6), (-3,6)
                (
                    new Vector3[] {
                        new Vector3(3f, 6f, 0),
                        new Vector3(-3f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)),GetRepeatedPattern("P_0", 25)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_25", 1), GetRepeatedPattern("P_24", 3)),GetRepeatedPattern("P_0", 25))
                    },
                    3
                ),
                // 12 번째 스폰: (1,6)
                (
                    new Vector3[] {
                        new Vector3(1, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20_D", 25)
                    },
                    3
                ),
                // 13 번째 스폰: (-1,6)
                (
                    new Vector3[] {
                        new Vector3(-1, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20_D", 25)
                    },
                    4
                ),
                // 14 번째 스폰: (1,6)
                (
                    new Vector3[] {
                        new Vector3(1, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20_D", 25)
                    },
                    3
                ),
                // 15 번째 스폰: (-1,6)
                (
                    new Vector3[] {
                        new Vector3(-1, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20_D", 25)
                    },
                    3
                ),
                // 16 번째 스폰: (1,6)
                (
                    new Vector3[] {
                        new Vector3(1, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20_D", 25)
                    },
                    4
                ),
                // 17 번째 스폰: (-1,6)
                (
                    new Vector3[] {
                        new Vector3(-1, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20_D", 25)
                    },
                    3
                ),
                // 18 번째 스폰: (1,6)
                (
                    new Vector3[] {
                        new Vector3(1, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20_D", 25)
                    },
                    3
                ),
                // 19 번째 스폰: (-1,6)
                (
                    new Vector3[] {
                        new Vector3(-1, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20_D", 25)
                    },
                    4
                ),
                // 20 번째 스폰: (1,6)
                (
                    new Vector3[] {
                        new Vector3(1, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20_D", 25)
                    },
                    4
                ),
                // 21 번째 스폰: (-1,6)
                (
                    new Vector3[] {
                        new Vector3(-1, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20_D", 25)
                    },
                    3
                ),
            }
        },
        // 웨이브 10
        { 10, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 첫 번째 스폰: (3,4)
                (
                    new Vector3[] {
                        new Vector3(3f, 4f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_19", 1)
                    },
                    5
                ),
                // 두 번째 스폰: (3,4)
                (
                    new Vector3[] {
                        new Vector3(3f, 4f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_19", 1)
                    },
                    5
                ),
                // 세 번째 스폰: (3,4)
                (
                    new Vector3[] {
                        new Vector3(3f, 4f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_19", 1)
                    },
                    5
                ),
                // 네 번째 스폰: (3,1)
                (
                    new Vector3[] {
                        new Vector3(3f, 1f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_3", 2), ConcatPatterns(GetRepeatedPattern("P_1", 1), GetRepeatedPattern("P_25_boss", 1)))
                    },
                    4
                ),
                // 다섯 번째 스폰: (-3,1)
                (
                    new Vector3[] {
                        new Vector3(-3f, 1f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_2", 2), ConcatPatterns(GetRepeatedPattern("P_1", 1), GetRepeatedPattern("P_25_boss", 1)))
                    },
                    4
                ),
                // 6 번째 스폰: (3,2)
                (
                    new Vector3[] {
                        new Vector3(3f, 2f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_3", 2), ConcatPatterns(GetRepeatedPattern("P_1", 1), GetRepeatedPattern("P_25_boss", 1)))
                    },
                    4
                ),
                // 7 번째 스폰: (-3,2)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_2", 2), ConcatPatterns(GetRepeatedPattern("P_1", 1), GetRepeatedPattern("P_25_boss", 1)))
                    },
                    4
                ),
                // 8 번째 스폰: (3,3)
                (
                    new Vector3[] {
                        new Vector3(3f, 3f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_3", 1), ConcatPatterns(GetRepeatedPattern("P_1", 1), GetRepeatedPattern("P_25_boss", 1)))
                    },
                    4
                ),
                // 9 번째 스폰: (-3,3)
                (
                    new Vector3[] {
                        new Vector3(-3f, 3f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_2", 1), ConcatPatterns(GetRepeatedPattern("P_1", 1), GetRepeatedPattern("P_25_boss", 1)))
                    },
                    4
                ),
                // 10 번째 스폰: (3,4)
                (
                    new Vector3[] {
                        new Vector3(3f, 4f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_3", 1), ConcatPatterns(GetRepeatedPattern("P_1", 1), GetRepeatedPattern("P_25_boss", 1)))
                    },
                    4
                ),
                // 11 번째 스폰: (-3,4)
                (
                    new Vector3[] {
                        new Vector3(-3f, 4f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_2", 1), ConcatPatterns(GetRepeatedPattern("P_1", 1), GetRepeatedPattern("P_25_boss", 1)))
                    },
                    4
                ),
            }
        },
        // 웨이브 11
        { 11, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 첫 번째 스폰: (3,2)
                (
                    new Vector3[] {
                        new Vector3(3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_28", 1)
                    },
                    6
                ),
                // 2 번째 스폰: (-3,2)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_29", 1)
                    },
                    6
                ),
                // 3 번째 스폰: (3,2)
                (
                    new Vector3[] {
                        new Vector3(3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_28", 1)
                    },
                    6
                ),
                // 4 번째 스폰: (-3,2)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_29", 1)
                    },
                    6
                ),
                // 5 번째 스폰: (3,2)
                (
                    new Vector3[] {
                        new Vector3(3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_28", 1)
                    },
                    6
                ),
                // 6 번째 스폰: (-3,2)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_29", 1)
                    },
                    6
                ),
                // 7 번째 스폰: (3,2)
                (
                    new Vector3[] {
                        new Vector3(3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_28", 1)
                    },
                    6
                ),
                // 8 번째 스폰: (-3,2)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_29", 1)
                    },
                    6
                ),
                // 9 번째 스폰: (3,2)
                (
                    new Vector3[] {
                        new Vector3(3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_28", 1)
                    },
                    6
                ),
                // 10 번째 스폰: (-3,2)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_29", 1)
                    },
                    6
                ),
                // 11 번째 스폰: (3,2)
                (
                    new Vector3[] {
                        new Vector3(3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_28", 1)
                    },
                    6
                ),
                // 12 번째 스폰: (-3,2)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_29", 1)
                    },
                    6
                ),
                // 13 번째 스폰: (3,2)
                (
                    new Vector3[] {
                        new Vector3(3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_28", 1)
                    },
                    6
                ),
                // 14 번째 스폰: (-3,2)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_29", 1)
                    },
                    6
                ),

            }
        },
        // 웨이브 12
        { 12, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 첫 번째 스폰: (0,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_27", 1))
                    },
                    7
                ),
                // 2 번째 스폰: (3,4)
                (
                    new Vector3[] {
                        new Vector3(3f, 4f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_19", 1)
                    },
                    6
                ),
                // 3 번째 스폰: (3,4)
                (
                    new Vector3[] {
                        new Vector3(3f, 4f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_19", 1)
                    },
                    6
                ),
                // 4 번째 스폰: (3,4)
                (
                    new Vector3[] {
                        new Vector3(3f, 4f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_19", 1)
                    },
                    6
                ),
                // 5 번째 스폰: (0,6), (-1,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0),
                        new Vector3(-1f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25)
                    },
                    4
                ),
                // 6 번째 스폰: (1,6), (2,6)
                (
                    new Vector3[] {
                        new Vector3(1f, 6f, 0),
                        new Vector3(2f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 25),
                        GetRepeatedPattern("P_0", 25)
                    },
                    4
                ),
                // 7 번째 스폰: (-2,6)
                (
                    new Vector3[] {
                        new Vector3(-2f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 25)
                    },
                    4
                ),
            }
        }, 
        // 웨이브 13
        { 13, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 첫 번째 스폰: (0,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_24", 2)), ConcatPatterns(GetRepeatedPattern("P_0", 5),GetRepeatedPattern("P_1", 5)))
                    },
                    7
                ),
                // 2 번째 스폰: (-1,6), (1,6)
                (
                    new Vector3[] {
                        new Vector3(-1f, 6f, 0),
                        new Vector3(1f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 8), GetRepeatedPattern("P_1", 5)),
                        ConcatPatterns(GetRepeatedPattern("P_0", 8), GetRepeatedPattern("P_1", 5))
                    },
                    7
                ),
                // 3 번째 스폰: (2,6), (-2,6)
                (
                    new Vector3[] {
                        new Vector3(2f, 6f, 0),
                        new Vector3(-2f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 9), GetRepeatedPattern("P_1", 6)),
                        ConcatPatterns(GetRepeatedPattern("P_0", 9), GetRepeatedPattern("P_1", 6))
                    },
                    7
                ),
                // 4 번째 스폰: (0,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_27", 1))
                    },
                    7
                ),
                // 5 번째 스폰: (3,4)
                (
                    new Vector3[] {
                        new Vector3(3f, 4f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_19", 1)
                    },
                    6
                ),
                // 6 번째 스폰: (3,4)
                (
                    new Vector3[] {
                        new Vector3(3f, 4f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_19", 1)
                    },
                    6
                ),
                // 7 번째 스폰: (3,4)
                (
                    new Vector3[] {
                        new Vector3(3f, 4f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_19", 1)
                    },
                    6
                ),
            }
        },
        // 웨이브 14
        { 14, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 첫 번째 스폰: (0,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_22", 1)
                    },
                    7
                ),
                // 2 번째 스폰: (3,2)
                (
                    new Vector3[] {
                        new Vector3(3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_28", 1)
                    },
                    6
                ),
                // 3 번째 스폰: (-3,2)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_29", 1)
                    },
                    6
                ),
                // 4 번째 스폰: (3,2)
                (
                    new Vector3[] {
                        new Vector3(3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_28", 1)
                    },
                    6
                ),
                // 5 번째 스폰: (-3,2)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_29", 1)
                    },
                    6
                ),
                // 6 번째 스폰: (3,2)
                (
                    new Vector3[] {
                        new Vector3(3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_28", 1)
                    },
                    6
                ),
                // 7 번째 스폰: (-3,2)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_29", 1)
                    },
                    6
                ),
                // 8 번째 스폰: (0,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_22", 1)
                    },
                    7
                ),
            }
        },
        // 웨이브 15
        { 15, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 첫 번째 스폰: (-1.5,6), (0,6), (1.5,6)
                (
                    new Vector3[] {
                        new Vector3(-1.5f, 6f, 0),
                        new Vector3(0, 6f, 0),
                        new Vector3(1.5f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_24", 3)), GetRepeatedPattern("P_0F", 7)), ConcatPatterns(GetRepeatedPattern("P_1", 7), GetRepeatedPattern("P_22F", 1))),
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_24", 4)), GetRepeatedPattern("P_0F", 7)), GetRepeatedPattern("P_1", 7)),
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_24", 3)), GetRepeatedPattern("P_0F", 7)), ConcatPatterns(GetRepeatedPattern("P_1", 7), GetRepeatedPattern("P_22F", 1))),
                    },
                    8
                ),
                // 2 번째 스폰: (3,4)
                (
                    new Vector3[] {
                        new Vector3(3f, 4f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_19", 1)
                    },
                    6
                ),
                // 3 번째 스폰: (3,4)
                (
                    new Vector3[] {
                        new Vector3(3f, 4f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_19", 1)
                    },
                    6
                ),
                // 4 번째 스폰: (3,4)
                (
                    new Vector3[] {
                        new Vector3(3f, 4f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_19", 1)
                    },
                    6
                ),
            }
        },
        // 웨이브 16
        { 16, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 첫 번째 스폰: (-3,6), (-2,6), (-1,6), (0,6), (1,6)
                (
                    new Vector3[] {
                        new Vector3(-3f, 6f, 0),
                        new Vector3(-2f, 6f, 0),
                        new Vector3(-1f, 6f, 0),
                        new Vector3(0, 6f, 0),
                        new Vector3(1f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_8", 2),
                        GetRepeatedPattern("P_8", 2),
                        GetRepeatedPattern("P_8", 2),
                        GetRepeatedPattern("P_8", 2),
                        GetRepeatedPattern("P_8", 2)
                    },
                    9
                ),
                // 2 번째 스폰: (-2,0), (2,0)
                (
                    new Vector3[] {
                        new Vector3(-2f, 0, 0),
                        new Vector3(2f, 0, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_24", 4), GetRepeatedPattern("P_3", 5)),
                        ConcatPatterns(GetRepeatedPattern("P_24", 4), GetRepeatedPattern("P_2", 5))
                    },
                    11
                ),
                // 3 번째 스폰: (0,6), (1,6), (2,6), (3,6), (4,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0),
                        new Vector3(1f, 6f, 0),
                        new Vector3(2f, 6f, 0),
                        new Vector3(3f, 6f, 0),
                        new Vector3(4f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_9", 3),
                        GetRepeatedPattern("P_9", 3),
                        GetRepeatedPattern("P_9", 3),
                        GetRepeatedPattern("P_9", 3),
                        GetRepeatedPattern("P_9", 3)
                    },
                    9
                ),
                // 4 번째 스폰: (-3,2.5), (3,2.5)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2.5f, 0),
                        new Vector3(3f, 2.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_2", 2),
                        GetRepeatedPattern("P_3", 2)
                    },
                    9
                ),
                // 5 번째 스폰: (-3,2.5), (3,2.5)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2.5f, 0),
                        new Vector3(3f, 2.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_2", 1),
                        GetRepeatedPattern("P_3", 1)
                    },
                    9
                ),
                // 6 번째 스폰: (0,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 3)
                    },
                    10
                ),
            }
        },
        // 웨이브 17
        { 17, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 첫 번째 스폰: (-2,6), (2,6), (0,6)
                (
                    new Vector3[] {
                        new Vector3(-2f, 6f, 0),
                        new Vector3(2f, 6f, 0),
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20", 1),
                        GetRepeatedPattern("P_20", 1),
                        GetRepeatedPattern("P_0", 15),
                    },
                    9
                ),
                // 2 번째 스폰: (-2,6), (2,6)
                (
                    new Vector3[] {
                        new Vector3(-2f, 6f, 0),
                        new Vector3(2f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20", 1),
                        GetRepeatedPattern("P_20", 1)
                    },
                    9
                ),
                // 3 번째 스폰: (0,6), (-2,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0),
                        new Vector3(-2f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 15),
                        GetRepeatedPattern("P_20", 1)
                    },
                    9
                ),
                // 4 번째 스폰: (-2,6), (2,6)
                (
                    new Vector3[] {
                        new Vector3(-2f, 6f, 0),
                        new Vector3(2f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_20", 1),
                        GetRepeatedPattern("P_20", 1)
                    },
                    9
                ),
                // 5 번째 스폰: (0,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 15)
                    },
                    9
                ),
                // 5 번째 스폰: (0,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 15)
                    },
                    9
                ),
            }
        },
        // 웨이브 18
        { 18, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 첫 번째 스폰: (-2,6), (0,6), (2,6)
                (
                    new Vector3[] {
                        new Vector3(-2f, 6f, 0),
                        new Vector3(0, 6f, 0),
                        new Vector3(2f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_24", 2)), ConcatPatterns(GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_24", 3))), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_24", 2)), ConcatPatterns(GetRepeatedPattern("P_0", 1), GetRepeatedPattern("P_24", 3))), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_24", 2)), ConcatPatterns(GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_24", 3))), GetRepeatedPattern("P_0", 15))
                    },
                    10
                ),
                // 2 번째 스폰: (-3,1.5), (3,1.5)
                (
                    new Vector3[] {
                        new Vector3(-3f, 1.5f, 0),
                        new Vector3(3, 1.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_24", 1)), ConcatPatterns(GetRepeatedPattern("P_17", 25), GetRepeatedPattern("P_24", 4))), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_24", 1)), ConcatPatterns(GetRepeatedPattern("P_16", 25), GetRepeatedPattern("P_24", 4))), GetRepeatedPattern("P_0", 15))
                    },
                    9
                ),
                // 3 번째 스폰: (-3,1.5), (3,1.5)
                (
                    new Vector3[] {
                        new Vector3(-3f, 1.5f, 0),
                        new Vector3(3, 1.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_24", 1)), ConcatPatterns(GetRepeatedPattern("P_17", 25), GetRepeatedPattern("P_24", 4))), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_24", 1)), ConcatPatterns(GetRepeatedPattern("P_16", 25), GetRepeatedPattern("P_24", 4))), GetRepeatedPattern("P_0", 15))
                    },
                    9
                ),
                // 4 번째 스폰: (-3,4), (3,4)
                (
                    new Vector3[] {
                        new Vector3(-3f, 4f, 0),
                        new Vector3(3, 4f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_24", 3)), ConcatPatterns(GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_24", 3))), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_24", 3)), ConcatPatterns(GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_24", 3))), GetRepeatedPattern("P_0", 15))
                    },
                    10
                ),
                // 5 번째 스폰: (-3,1.5), (3,1.5)
                (
                    new Vector3[] {
                        new Vector3(-3f, 1.5f, 0),
                        new Vector3(3, 1.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_24", 1)), ConcatPatterns(GetRepeatedPattern("P_17", 25), GetRepeatedPattern("P_24", 4))), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_24", 1)), ConcatPatterns(GetRepeatedPattern("P_16", 25), GetRepeatedPattern("P_24", 4))), GetRepeatedPattern("P_0", 15))
                    },
                    9
                ),
                // 6 번째 스폰: (-3,1.5), (3,1.5)
                (
                    new Vector3[] {
                        new Vector3(-3f, 1.5f, 0),
                        new Vector3(3, 1.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_24", 1)), ConcatPatterns(GetRepeatedPattern("P_17", 25), GetRepeatedPattern("P_24", 4))), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_24", 1)), ConcatPatterns(GetRepeatedPattern("P_16", 25), GetRepeatedPattern("P_24", 4))), GetRepeatedPattern("P_0", 15))
                    },
                    9
                ),
                // 7 번째 스폰: (-3,1.5), (3,1.5)
                (
                    new Vector3[] {
                        new Vector3(-3f, 1.5f, 0),
                        new Vector3(3, 1.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_24", 1)), ConcatPatterns(GetRepeatedPattern("P_17", 25), GetRepeatedPattern("P_24", 4))), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_24", 1)), ConcatPatterns(GetRepeatedPattern("P_16", 25), GetRepeatedPattern("P_24", 4))), GetRepeatedPattern("P_0", 15))
                    },
                    9
                ),
            }
        },
        // 웨이브 19
        { 19, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 첫 번째 스폰: (-3,3), (-3,3), (3,3), (3,3)
                (
                    new Vector3[] {
                        new Vector3(-3f, 3f, 0),
                        new Vector3(-3f, 3f, 0),
                        new Vector3(3f, 3f, 0),
                        new Vector3(3f, 3f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_2", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_2", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_3", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_3", 1))
                    },
                    9
                ),
                // 2 번째 스폰: (0,-1.5)
                (
                    new Vector3[] {
                        new Vector3(0, -1.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_13", 15)
                    },
                    11
                ),
                // 3 번째 스폰: (-3,4.5), (3,4.5)
                (
                    new Vector3[] {
                        new Vector3(-3, 4.5f, 0),
                        new Vector3(3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_2", 2), ConcatPatterns(GetRepeatedPattern("P_24", 3), GetRepeatedPattern("P_23", 1))),
                        ConcatPatterns(GetRepeatedPattern("P_3", 2), ConcatPatterns(GetRepeatedPattern("P_24", 3), GetRepeatedPattern("P_23", 1)))
                    },
                    10
                ),
                // 4 번째 스폰: (-3,3), (-3,3), (3,3), (3,3)
                (
                    new Vector3[] {
                        new Vector3(-3f, 3f, 0),
                        new Vector3(-3f, 3f, 0),
                        new Vector3(3f, 3f, 0),
                        new Vector3(3f, 3f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_2", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_2", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_3", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_3", 1))
                    },
                    9
                ),
                // 5 번째 스폰: (0,-1.5)
                (
                    new Vector3[] {
                        new Vector3(0, -1.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_12", 15)
                    },
                    11
                ),
                // 6 번째 스폰: (-3,3), (-3,3), (3,3), (3,3)
                (
                    new Vector3[] {
                        new Vector3(-3f, 3f, 0),
                        new Vector3(-3f, 3f, 0),
                        new Vector3(3f, 3f, 0),
                        new Vector3(3f, 3f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_2", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_2", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_3", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_3", 1))
                    },
                    9
                ),
                // 7 번째 스폰: (-3,3), (-3,3), (3,3), (3,3)
                (
                    new Vector3[] {
                        new Vector3(-3f, 3f, 0),
                        new Vector3(-3f, 3f, 0),
                        new Vector3(3f, 3f, 0),
                        new Vector3(3f, 3f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_2", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_2", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_3", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_3", 1))
                    },
                    9
                ),
                // 8 번째 스폰: (-3,3), (-3,3), (3,3), (3,3)
                (
                    new Vector3[] {
                        new Vector3(-3f, 3f, 0),
                        new Vector3(-3f, 3f, 0),
                        new Vector3(3f, 3f, 0),
                        new Vector3(3f, 3f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_2", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_2", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_3", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_0", 5)), GetRepeatedPattern("P_3", 1))
                    },
                    9
                ),
                // 9 번째 스폰: (0,-1.5)
                (
                    new Vector3[] {
                        new Vector3(0, -1.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_13", 15)
                    },
                    11
                ),
                // 10 번째 스폰: (0,-1.5)
                (
                    new Vector3[] {
                        new Vector3(0, -1.5f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_12", 15)
                    },
                    11
                ),
            }
        },
        // 웨이브 20
        { 20, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 첫 번째 스폰: (0,7)
                (
                    new Vector3[] {
                        new Vector3(0, 7f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_24", 2)), ConcatPatterns(GetRepeatedPattern("P_1", 1), GetRepeatedPattern("P_24", 3))), GetRepeatedPattern("P_2", 2))
                    },
                    12
                ),
                // 2 번째 스폰: (-1,6), (1,6)
                (
                    new Vector3[] {
                        new Vector3(-1f, 6f, 0),
                        new Vector3(1f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 15),
                        GetRepeatedPattern("P_0", 15)
                    },
                    9
                ),
                // 3 번째 스폰: (-2,6), (2,6)
                (
                    new Vector3[] {
                        new Vector3(-2f, 6f, 0),
                        new Vector3(2f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 15),
                        GetRepeatedPattern("P_0", 15)
                    },
                    10
                ),
                // 4 번째 스폰: (-3,-2)
                (
                    new Vector3[] {
                        new Vector3(-3f, -2f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_24", 1)), ConcatPatterns(GetRepeatedPattern("P_6", 2), GetRepeatedPattern("P_24", 1))), GetRepeatedPattern("P_8", 2))
                    },
                    11
                ),
                // 5 번째 스폰: (-3,4), (-3,2), (3,3), (3,1)
                (
                    new Vector3[] {
                        new Vector3(-3f, 4f, 0),
                        new Vector3(-3f, 2f, 0),
                        new Vector3(3f, 3f, 0),
                        new Vector3(3f, 1f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_4", 3), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(GetRepeatedPattern("P_4", 3), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(GetRepeatedPattern("P_5", 3), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(GetRepeatedPattern("P_5", 3), GetRepeatedPattern("P_0", 15)),
                    },
                    9
                ),
                // 6 번째 스폰: (-1,6), (1,6)
                (
                    new Vector3[] {
                        new Vector3(-1f, 6f, 0),
                        new Vector3(1f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 15),
                        GetRepeatedPattern("P_0", 15)
                    },
                    9
                ),
                // 7 번째 스폰: (-2,6), (2,6)
                (
                    new Vector3[] {
                        new Vector3(-2f, 6f, 0),
                        new Vector3(2f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 15),
                        GetRepeatedPattern("P_0", 15)
                    },
                    10
                ),
                // 8 번째 스폰: (-3,4), (-3,2), (3,3), (3,1)
                (
                    new Vector3[] {
                        new Vector3(-3f, 4f, 0),
                        new Vector3(-3f, 2f, 0),
                        new Vector3(3f, 3f, 0),
                        new Vector3(3f, 1f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_4", 3), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(GetRepeatedPattern("P_4", 3), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(GetRepeatedPattern("P_5", 3), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(GetRepeatedPattern("P_5", 3), GetRepeatedPattern("P_0", 15)),
                    },
                    9
                ),
                // 9 번째 스폰: (-1,6), (1,6)
                (
                    new Vector3[] {
                        new Vector3(-1f, 6f, 0),
                        new Vector3(1f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 15),
                        GetRepeatedPattern("P_0", 15)
                    },
                    9
                ),
                // 10 번째 스폰: (-2,6), (2,6)
                (
                    new Vector3[] {
                        new Vector3(-2f, 6f, 0),
                        new Vector3(2f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 15),
                        GetRepeatedPattern("P_0", 15)
                    },
                    10
                ),
                // 11 번째 스폰: (-3,4), (-3,2), (3,3), (3,1)
                (
                    new Vector3[] {
                        new Vector3(-3f, 4f, 0),
                        new Vector3(-3f, 2f, 0),
                        new Vector3(3f, 3f, 0),
                        new Vector3(3f, 1f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_4", 3), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(GetRepeatedPattern("P_4", 3), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(GetRepeatedPattern("P_5", 3), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(GetRepeatedPattern("P_5", 3), GetRepeatedPattern("P_0", 15)),
                    },
                    9
                ),
            }
        },
        // 웨이브 21
        { 21, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 첫 번째 스폰: (-1.5,6), (1.5,6), (-3,4.5), (3,4.5)
                (
                    new Vector3[] {
                        new Vector3(-1.5f, 6f, 0),
                        new Vector3(1.5f, 6f, 0),
                        new Vector3(-3f, 4.5f, 0),
                        new Vector3(3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_16", 25)),
                        ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_17", 25)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 4), GetRepeatedPattern("P_24", 4)), GetRepeatedPattern("P_2", 10)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 4), GetRepeatedPattern("P_24", 4)), GetRepeatedPattern("P_3", 10))
                    },
                    0
                ),
                // 2 번째 스폰: (-1.5,6), (1.5,6)
                (
                    new Vector3[] {
                        new Vector3(-1.5f, 6f, 0),
                        new Vector3(1.5f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_16", 25)),
                        ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_17", 25))
                    },
                    0
                ),
                // 3 번째 스폰: (-3,4.5), (-1.5,6), (3,4.5), (1.5,6)
                (
                    new Vector3[] {
                        new Vector3(-3f, 4.5f, 0),
                        new Vector3(-1.5f, 6f, 0),
                        new Vector3(3f, 4.5f, 0),
                        new Vector3(1.5f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 3), GetRepeatedPattern("P_24", 3)), GetRepeatedPattern("P_2", 10)),
                        ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_16", 25)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 3), GetRepeatedPattern("P_24", 3)), GetRepeatedPattern("P_3", 10)),
                        ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_17", 25))
                    },
                    0
                ),
                // 4 번째 스폰: (-1.5,6), (1.5,6)
                (
                    new Vector3[] {
                        new Vector3(-1.5f, 6f, 0),
                        new Vector3(1.5f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_16", 25)),
                        ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_17", 25))
                    },
                    0
                ),
                // 5 번째 스폰: (-1.5,6), (1.5,6), (-3,4.5), (3,4.5)
                (
                    new Vector3[] {
                        new Vector3(-1.5f, 6f, 0),
                        new Vector3(1.5f, 6f, 0),
                        new Vector3(-3f, 4.5f, 0),
                        new Vector3(3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_16", 25)),
                        ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_17", 25)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_24", 2)), GetRepeatedPattern("P_2", 10)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_24", 2)), GetRepeatedPattern("P_3", 10))
                    },
                    0
                ),
                // 6 번째 스폰: (-3,4.5), (3,4.5)
                (
                    new Vector3[] {
                        new Vector3(-3f, 4.5f, 0),
                        new Vector3(3f, 4.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_2", 10)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_3", 10))
                    },
                    0
                ),
                // 7 번째 스폰: (-2,6), (2,6)
                (
                    new Vector3[] {
                        new Vector3(-2f, 6f, 0),
                        new Vector3(2f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 15),
                        GetRepeatedPattern("P_0", 15)
                    },
                    4
                ),
                // 8 번째 스폰: (-2,6), (2,6)
                (
                    new Vector3[] {
                        new Vector3(-2f, 6f, 0),
                        new Vector3(2f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 15),
                        GetRepeatedPattern("P_0", 15)
                    },
                    4
                ),
                // 9 번째 스폰: (-3,0.5), (-3,0.5), (3,0.5), (3,0.5)
                (
                    new Vector3[] {
                        new Vector3(-3f, 0.5f, 0),
                        new Vector3(-3f, 0.5f, 0),
                        new Vector3(3f, 0.5f, 0),
                        new Vector3(3f, 0.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_24", 2)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_24", 2)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_24", 2)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_24", 2)), GetRepeatedPattern("P_0", 15)),
                    },
                    1
                ),
                // 10 번째 스폰: (-1,6), (-2,6), (1,6), (2,6)
                (
                    new Vector3[] {
                        new Vector3(-1f, 6f, 0),
                        new Vector3(-2f, 6f, 0),
                        new Vector3(1f, 6f, 0),
                        new Vector3(2f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_25R", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_25R", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_25R", 1)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_25R", 1)),
                    },
                    3
                ),
            }
        },
        // 웨이브 22
        { 22, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 1 번째 스폰: (-1,6), (1,6)
                (
                    new Vector3[] {
                        new Vector3(-1f, 6f, 0),
                        new Vector3(1f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_25", 25)),
                        ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_25", 25))
                    },
                    3
                ),
                // 2 번째 스폰: (-2,6), (-1,6), (0,6), (1,6), (2,6)
                (
                    new Vector3[] {
                        new Vector3(-2f, 6f, 0),
                        new Vector3(-1f, 6f, 0),
                        new Vector3(0f, 6f, 0),
                        new Vector3(1f, 6f, 0),
                        new Vector3(2f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 4), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_0", 15))
                    },
                    4
                ),
                // 3 번째 스폰: (-2,6), (-1,6), (0,6), (1,6), (2,6)
                (
                    new Vector3[] {
                        new Vector3(-2f, 6f, 0),
                        new Vector3(-1f, 6f, 0),
                        new Vector3(0f, 6f, 0),
                        new Vector3(1f, 6f, 0),
                        new Vector3(2f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_0", 15))
                    },
                    4
                ),
                // 4 번째 스폰: (-2,6), (-1,6), (0,6), (1,6), (2,6)
                (
                    new Vector3[] {
                        new Vector3(-2f, 6f, 0),
                        new Vector3(-1f, 6f, 0),
                        new Vector3(0f, 6f, 0),
                        new Vector3(1f, 6f, 0),
                        new Vector3(2f, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 2), GetRepeatedPattern("P_24", 1)), GetRepeatedPattern("P_0", 15))
                    },
                    4
                ),
                // 5 번째 스폰: (-3,1), (-3,1), (-3,1), (-3,1), (-3,1), (3,1), (3,1), (3,1), (3,1), (3,1)
                (
                    new Vector3[] {
                        new Vector3(-3f, 1f, 0),
                        new Vector3(-3f, 1f, 0),
                        new Vector3(-3f, 1f, 0),
                        new Vector3(-3f, 1f, 0),
                        new Vector3(-3f, 1f, 0),
                        new Vector3(3f, 1f, 0),
                        new Vector3(3f, 1f, 0),
                        new Vector3(3f, 1f, 0),
                        new Vector3(3f, 1f, 0),
                        new Vector3(3f, 1f, 0),
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 5), GetRepeatedPattern("P_24", 3)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 4), GetRepeatedPattern("P_24", 3)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 3), GetRepeatedPattern("P_24", 3)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_24", 3)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_24", 3)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 5), GetRepeatedPattern("P_24", 3)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 4), GetRepeatedPattern("P_24", 3)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 3), GetRepeatedPattern("P_24", 3)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_24", 3)), GetRepeatedPattern("P_0", 15)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_24", 3)), GetRepeatedPattern("P_0", 15)),
                    },
                    6
                ),
            }
        },
        // 웨이브 23
        { 23, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 1 번째 스폰: (-2,-2)
                (
                    new Vector3[] {
                        new Vector3(-2f, -2f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_8", 4), GetRepeatedPattern("P_10", 4)), GetRepeatedPattern("P_11", 4))
                    },
                    11
                ),
                // 2 번째 스폰: (-2,6)
                (
                    new Vector3[] {
                        new Vector3(-2f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 2)
                    },
                    10
                ),
                // 3 번째 스폰: (-1,6), (1,6)
                (
                    new Vector3[] {
                        new Vector3(-1f, 6f, 0),
                        new Vector3(1f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 2),
                        GetRepeatedPattern("P_0", 2)
                    },
                    10
                ),
                // 4 번째 스폰: (-2,2)
                (
                    new Vector3[] {
                        new Vector3(-2f, 2f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_24", 1), GetRepeatedPattern("P_0", 1))
                    },
                    9
                ),
                // 5 번째 스폰: (-1,2)
                (
                    new Vector3[] {
                        new Vector3(-1f, 2f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_24", 1), GetRepeatedPattern("P_0", 1))
                    },
                    9
                ),
                // 6 번째 스폰: (1,2)
                (
                    new Vector3[] {
                        new Vector3(1f, 2f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_24", 1), GetRepeatedPattern("P_0", 1))
                    },
                    9
                ),
                // 7 번째 스폰: (2,2)
                (
                    new Vector3[] {
                        new Vector3(2f, 2f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_24", 1), GetRepeatedPattern("P_0", 1))
                    },
                    9
                ),
                // 8 번째 스폰: (2,3)
                (
                    new Vector3[] {
                        new Vector3(2f, 3f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_24", 1), GetRepeatedPattern("P_0", 1))
                    },
                    9
                ),
                // 9 번째 스폰: (0,6), (2,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0),
                        new Vector3(2f, 6f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_0", 2),
                        GetRepeatedPattern("P_0", 2)
                    },
                    10
                ),
                // 10 번째 스폰: (1,3)
                (
                    new Vector3[] {
                        new Vector3(1f, 3f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_24", 1), GetRepeatedPattern("P_0", 1))
                    },
                    9
                ),
                // 11 번째 스폰: (-1,3)
                (
                    new Vector3[] {
                        new Vector3(-1f, 3f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_24", 1), GetRepeatedPattern("P_0", 1))
                    },
                    9
                ),
                // 12 번째 스폰: (-2,3)
                (
                    new Vector3[] {
                        new Vector3(-2f, 3f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_24", 1), GetRepeatedPattern("P_0", 1))
                    },
                    9
                ),
                // 13 번째 스폰: (-3,3), (3,3)
                (
                    new Vector3[] {
                        new Vector3(-3f, 3f, 0),
                        new Vector3(3f, 3f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_25", 25),
                        GetRepeatedPattern("P_25", 25)
                    },
                    7
                ),
                // 14 번째 스폰: (-3,3), (3,3)
                (
                    new Vector3[] {
                        new Vector3(-3f, 3f, 0),
                        new Vector3(3f, 3f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_25", 25),
                        GetRepeatedPattern("P_25", 25)
                    },
                    7
                ),
                // 15 번째 스폰: (0,2)
                (
                    new Vector3[] {
                        new Vector3(0, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_16", 25)
                    },
                    9
                ),
                // 16 번째 스폰: (0,2)
                (
                    new Vector3[] {
                        new Vector3(0, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_16", 25)
                    },
                    9
                ),
                // 17 번째 스폰: (0,2), (-3,3), (3,3)
                (
                    new Vector3[] {
                        new Vector3(0, 2f, 0),
                        new Vector3(-3f, 3f, 0),
                        new Vector3(3f, 3f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_16", 25),
                        GetRepeatedPattern("P_25", 25),
                        GetRepeatedPattern("P_25", 25)
                    },
                    9
                ),
                // 18 번째 스폰: (0,2)
                (
                    new Vector3[] {
                        new Vector3(0, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_16", 25)
                    },
                    9
                ),
                // 19 번째 스폰: (0,2)
                (
                    new Vector3[] {
                        new Vector3(0, 2f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_16", 25)
                    },
                    9
                ),
            }
        },
        // 웨이브 24
        { 24, new List<(Vector3[] positions, string[][] patterns, int enemyType)>()
            {
                // 1 번째 스폰: (0,6.5)
                (
                    new Vector3[] {
                        new Vector3(0, 6.5f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_24", 6)), GetRepeatedPattern("P_0", 15))
                    },
                    2
                ),
                // 2 번째 스폰: (-1.5,3), (1.5,3)
                (
                    new Vector3[] {
                        new Vector3(-1.5f, 3f, 0),
                        new Vector3(1.5f, 3f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_2", 1), GetRepeatedPattern("P_24", 7)), GetRepeatedPattern("P_3", 10)),
                        ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_3", 1), GetRepeatedPattern("P_24", 7)), GetRepeatedPattern("P_2", 10))
                    },
                    5
                ),
                // 3 번째 스폰: (-3,2), (3,2)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2f, 0),
                        new Vector3(3f, 2f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_25", 25)),
                        ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_25", 25))
                    },
                    1
                ),
                // 4 번째 스폰: (-3,2), (3,2)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2f, 0),
                        new Vector3(3f, 2f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_25", 25)),
                        ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_25", 25))
                    },
                    1
                ),
                // 5 번째 스폰: (-3,2), (3,2)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2f, 0),
                        new Vector3(3f, 2f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_25", 25)),
                        ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_25", 25))
                    },
                    4
                ),
                // 6 번째 스폰: (-3,2), (3,2)
                (
                    new Vector3[] {
                        new Vector3(-3f, 2f, 0),
                        new Vector3(3f, 2f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_2", 2), GetRepeatedPattern("P_25", 25)),
                        ConcatPatterns(GetRepeatedPattern("P_3", 2), GetRepeatedPattern("P_25", 25))
                    },
                    4
                ),
                // 7 번째 스폰: (0,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_17", 25))
                    },
                    8
                ),
                // 8 번째 스폰: (0,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_17", 25))
                    },
                    8
                ),
                // 9 번째 스폰: (0,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_17", 25))
                    },
                    6
                ),
                // 10 번째 스폰: (0,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_17", 25))
                    },
                    6
                ),
                // 11 번째 스폰: (0,6)
                (
                    new Vector3[] {
                        new Vector3(0, 6f, 0)
                    },
                    new string[][] {
                        ConcatPatterns(GetRepeatedPattern("P_0", 3), GetRepeatedPattern("P_17", 25))
                    },
                    6
                ),
                // 12 번째 스폰: (-2,0)
                (
                    new Vector3[] {
                        new Vector3(-2f, 0, 0)
                    },
                    new string[][] {
                        ConcatPatterns(ConcatPatterns(ConcatPatterns(GetRepeatedPattern("P_8", 4), GetRepeatedPattern("P_10", 4)), ConcatPatterns(GetRepeatedPattern("P_11", 4), GetRepeatedPattern("P_9", 4))), GetRepeatedPattern("P_0", 15))
                    },
                    11
                ),
                // 13 번째 스폰: (0,4)
                (
                    new Vector3[] {
                        new Vector3(0, 4f, 0)
                    },
                    new string[][] {
                        GetRepeatedPattern("P_27", 1)
                    },
                    12
                ),
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
            // 현재 웨이브 실행
            yield return StartCoroutine(ManageWaves(currentWave));

            // → 마지막 스폰이 끝나면 10초 대기
            yield return new WaitForSeconds(10f);

            // 다음 웨이브로
            currentWave++;
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
            if (waveNumber == 1 && i == 3) Pool.Instance.SpawnWeapon("UpgradeItem", new Vector3(0f, 6f, 0f), Quaternion.identity); ;
            if (waveNumber == 2 && i == 1) yield return new WaitForSeconds(3f);
            // wave 3
            if (waveNumber == 3 && i == 1) yield return new WaitForSeconds(3f);
            if (waveNumber == 3 && i >= 2) yield return new WaitForSeconds(0.5f);
            // wave 4
            if (waveNumber == 4 && i >= 2) yield return new WaitForSeconds(0.5f);
            // wave 5
            if (waveNumber == 5 && i == 1) yield return new WaitForSeconds(3f);
            // wave 6
            if (waveNumber == 6 && i == 1) yield return new WaitForSeconds(3f);
            if (waveNumber == 6 && i == 2) yield return new WaitForSeconds(3f);
            if (waveNumber == 6 && i == 3) yield return new WaitForSeconds(2f);
            // wave 7
            if (waveNumber == 7 && i == 1) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 7 && i == 2) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 7 && i == 3) yield return new WaitForSeconds(2f);
            if (waveNumber == 7 && i >= 4 && i <= 7) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 7 && i == 9) yield return new WaitForSeconds(0.5f);
            // wave 8
            if (waveNumber == 8 && i == 1) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 8 && i == 2) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 8 && i == 3) yield return new WaitForSeconds(2f);
            if (waveNumber == 8 && i >= 4) yield return new WaitForSeconds(1f);
            // wave 9
            if (waveNumber == 9 && i <= 4) yield return new WaitForSeconds(0.2f);
            if (waveNumber == 9 && i == 5) yield return new WaitForSeconds(2.2f);
            if (waveNumber == 9 && i >= 6 && i <= 10) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 9 && i == 11) yield return new WaitForSeconds(1.5f);
            if (waveNumber == 9 && i >= 12) yield return new WaitForSeconds(1f);
            // wave 10
            if (waveNumber == 10 && i == 1) yield return new WaitForSeconds(1f);
            if (waveNumber == 10 && i == 2) yield return new WaitForSeconds(1f);
            if (waveNumber == 10 && i == 3) yield return new WaitForSeconds(2f);
            if (waveNumber == 10 && i >= 4) yield return new WaitForSeconds(0.5f);
            // wave 11
            if (waveNumber == 11 && i >= 1) yield return new WaitForSeconds(1f);
            // wave 12
            if (waveNumber == 12 && i >= 1) yield return new WaitForSeconds(0.5f);
            // wave 13
            if (waveNumber == 13 && i <= 2) yield return new WaitForSeconds(2f);
            if (waveNumber == 13 && i == 3) yield return new WaitForSeconds(1f);
            if (waveNumber == 13 && i >= 5) yield return new WaitForSeconds(0.5f);
            // wave 14
            if (waveNumber == 14 && i >= 2 && i < 7) yield return new WaitForSeconds(1f);
            if (waveNumber == 14 && i == 7) yield return new WaitForSeconds(2f);
            // wave 15
            if (waveNumber == 15 && i == 1) yield return new WaitForSeconds(4f);
            if (waveNumber == 15 && i >= 2) yield return new WaitForSeconds(0.5f);
            // wave 16
            if (waveNumber == 16 && i == 2) yield return new WaitForSeconds(2f);
            if (waveNumber == 16 && i == 3) yield return new WaitForSeconds(1f);
            if (waveNumber == 16 && i == 4) yield return new WaitForSeconds(1f);
            // wave 17
            if (waveNumber == 17 && i <= 4 && i >= 0) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 17 && i == 5) yield return new WaitForSeconds(1f);
            // wave 18
            if (waveNumber == 18 && i == 2) yield return new WaitForSeconds(1f);
            if (waveNumber == 18 && i == 3) yield return new WaitForSeconds(1f);
            if (waveNumber == 18 && i == 5) yield return new WaitForSeconds(1f);
            if (waveNumber == 18 && i == 6) yield return new WaitForSeconds(1f);
            // wave 19
            if (waveNumber == 19 && i == 2) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 19 && i == 3) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 19 && i == 4) yield return new WaitForSeconds(1f);
            if (waveNumber == 19 && i == 6) yield return new WaitForSeconds(1f);
            if (waveNumber == 19 && i == 7) yield return new WaitForSeconds(1f);
            if (waveNumber == 19 && i == 9) yield return new WaitForSeconds(2f);
            // wave 20
            if (waveNumber == 20 && i == 4) yield return new WaitForSeconds(1.5f);
            if (waveNumber == 20 && i == 5) yield return new WaitForSeconds(1.5f);
            if (waveNumber == 20 && i == 7) yield return new WaitForSeconds(1f);
            if (waveNumber == 20 && i == 8) yield return new WaitForSeconds(2f);
            if (waveNumber == 20 && i == 10) yield return new WaitForSeconds(1.5f);
            if (waveNumber == 20 && i == 11) yield return new WaitForSeconds(1.5f);
            if (waveNumber == 20 && i == 13) yield return new WaitForSeconds(2f);
            // wave 21
            if (waveNumber == 21 && i >= 1 && i <= 4) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 21 && i == 5) yield return new WaitForSeconds(1f);
            if (waveNumber == 21 && i == 6) yield return new WaitForSeconds(2f);
            if (waveNumber == 21 && i == 7) yield return new WaitForSeconds(1f);
            if (waveNumber == 21 && i == 8) yield return new WaitForSeconds(2f);
            // wave 22
            if (waveNumber == 22 && i == 1) yield return new WaitForSeconds(2f);
            if (waveNumber == 22 && i >= 2 && i <= 3) yield return new WaitForSeconds(1f);
            // wave 23
            if (waveNumber == 23 && i == 2) yield return new WaitForSeconds(1f);
            if (waveNumber == 23 && i >= 4 && i <= 8) yield return new WaitForSeconds(0.2f);
            if (waveNumber == 23 && i >= 10 && i <= 11) yield return new WaitForSeconds(0.2f);
            if (waveNumber == 23 && i == 12) yield return new WaitForSeconds(1.1f);
            if (waveNumber == 23 && i == 13) yield return new WaitForSeconds(1.5f);
            if (waveNumber == 23 && i == 14) yield return new WaitForSeconds(1f);
            if (waveNumber == 23 && i >= 15 && i <= 18) yield return new WaitForSeconds(0.5f);
            // wave 24
            if (waveNumber == 24 && i == 1) yield return new WaitForSeconds(1f);
            if (waveNumber == 24 && i >= 3 && i <= 5) yield return new WaitForSeconds(2f);
            if (waveNumber == 24 && i == 6) yield return new WaitForSeconds(1f);
            if (waveNumber == 24 && i >= 7 && i <= 10) yield return new WaitForSeconds(0.5f);
            if (waveNumber == 24 && i == 12) yield return new WaitForSeconds(4f);

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
