using System.Collections.Generic;
using UnityEngine;



public class PatternManager : MonoBehaviour
{
    public static PatternManager Instance;

    private Dictionary<string, Vector2[]> patterns = new Dictionary<string, Vector2[]>
    {
        { "N_0", new Vector2[] { new Vector2(0, -1) } }, // 아래로 이동
        { "N_1", new Vector2[] { new Vector2(0, 1) } },  // 위로 이동
        { "N_2", new Vector2[] { new Vector2(1, 0) } },  // 오른쪽 이동
        { "N_3", new Vector2[] { new Vector2(-1, 0) } }, // 왼쪽 이동
        { "N_4", new Vector2[] { new Vector2(1, -1) } }, // 대각선 오른쪽 아래
        { "N_5", new Vector2[] { new Vector2(-1, -1) } }, // 대각선 왼쪽 아래
        { "N_6", new Vector2[] { new Vector2(1, 1) } }, // 대각선 오른쪽 위
        { "N_7", new Vector2[] { new Vector2(-1, 1) } } // 대각선 왼쪽 위
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
        Debug.Log("PatternManager 인스턴스 생성 완료");
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
}
