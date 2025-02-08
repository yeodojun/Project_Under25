using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab; // 적 프리팹
    public float spawnDelay = 0.3f; // 적 생성 간격
    public float fallSpeed = 2f; // 적이 내려오는 속도
    public float moveDuration = 1.0f; // 지정된 위치로 이동하는 시간

    private Transform[] enemyTransforms; // 적들의 Transform 배열
    private int enemyCount = 10; // 적 수 (딱 10마리만 소환)

    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        enemyTransforms = new Transform[enemyCount]; // 적 배열 초기화

        // 적 생성 (짧은 딜레이로 꼬리에 꼬리를 물게 소환)
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, new Vector3(0, 6.5f + (i * 0.5f), 0), Quaternion.Euler(0, 0, 180)); // 약간의 y 간격 추가
            enemyTransforms[i] = enemy.transform; // Transform 저장
            StartCoroutine(MoveDownAndSpread(enemyTransforms[i], i)); // 개별적으로 내려오고 퍼지게 함
            yield return new WaitForSeconds(spawnDelay); // 생성 간격
        }
    }

    IEnumerator MoveDownAndSpread(Transform enemy, int index)
    {
        // 적이 y=1.5까지 내려오기
        while (enemy != null && enemy.position.y > 1.5f)
        {
            enemy.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null;
        }

        // 적이 도달하면 바로 퍼짐
        if (enemy != null)
        {
            // 위 줄과 아래 줄 위치 계산
            Vector3[] upperRowPositions = { new Vector3(-1.5f, 3.7f, 0), new Vector3(-0.75f, 3.7f, 0), new Vector3(0, 3.7f, 0), new Vector3(0.75f, 3.7f, 0), new Vector3(1.5f, 3.7f, 0) };
            Vector3[] lowerRowPositions = { new Vector3(-1.5f, 2.0f, 0), new Vector3(-0.75f, 2.0f, 0), new Vector3(0, 2.0f, 0), new Vector3(0.75f, 2.0f, 0), new Vector3(1.5f, 2.0f, 0) };

            // 적을 위 줄이나 아래 줄로 이동
            Vector3 targetPosition = index < 5 ? upperRowPositions[index] : lowerRowPositions[index - 5];
            StartCoroutine(MoveToPosition(enemy, targetPosition));
        }
    }

    IEnumerator MoveToPosition(Transform enemy, Vector3 targetPosition)
    {
        Vector3 startPosition = enemy.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            enemy.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration); // 부드럽게 이동
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        enemy.position = targetPosition; // 최종 위치 고정
    }
}
