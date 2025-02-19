using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab; // 적 프리팹
    public float waveDelay = 5f; // 웨이브 간 딜레이
    private int currentWave = 1; // 현재 웨이브
    private bool waveInProgress = false; // 웨이브 진행 여부
    private string currentpattern = "";

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

                switch (currentWave) { // 패턴 확률
                    case 1:
                        yield return StartCoroutine(SpawnPattern("N_0", 10));
                        break;
                    case 2:
                        yield return StartCoroutine(SpawnPattern("N_1", 10));
                        break;
                    case 3:
                        yield return StartCoroutine(SpawnPattern("N_2", 10));
                        break;
                    case 4:
                        yield return StartCoroutine(SpawnPattern(GetRandomPattern("N_0", "N_1", "N_2"), 10));
                        break;
                    case 5:
                        yield return StartCoroutine(SpawnPattern(GetRandomPattern("N_0", "N_1", "N_2", "N_3", "N_4"), 10));
                        break;
                }

                Debug.Log($"Wave {currentWave} complete. Waiting {waveDelay} seconds before next wave...");
                // wave 종료 시 트리거

                yield return new WaitForSeconds(waveDelay);

                currentWave++;
                waveInProgress = false;

                if (currentWave > 5) { // wave가 5 도달 시 끝
                    Debug.Log("All waves completed!");
                    break;
                }
            }

            yield return null;
        }
    }

    IEnumerator SpawnPattern(string pattern, int enemyCount) { // 패턴 불러 오기
        switch (pattern)
        {
            case "N_0":
                currentpattern = "N_0";
                Debug.Log($"Pattern is {currentpattern}");
                yield return StartCoroutine(Pattern_N0(enemyCount));
                break;
            case "N_1":
                currentpattern = "N_1";
                Debug.Log($"Pattern is {currentpattern}");
                yield return StartCoroutine(Pattern_N1(enemyCount));
                break;
            case "N_2":
                currentpattern = "N_2";
                Debug.Log($"Pattern is {currentpattern}");
                yield return StartCoroutine(Pattern_N2(enemyCount));
                break;
            case "N_3":
                currentpattern = "N_98";
                Debug.Log($"Pattern is {currentpattern}");
                yield return StartCoroutine(Pattern_N98(enemyCount));
                break;
            case "N_4":
                currentpattern = "N_99";
                Debug.Log($"Pattern is {currentpattern}");
                yield return StartCoroutine(Pattern_N99(enemyCount));
                break;
            case "N_5":
                yield return StartCoroutine(Pattern_N5(enemyCount));
                break;
        }
    }

    string GetRandomPattern(params string[] patterns) { // 4,5웨이브 랜덤 패턴
        int index = Random.Range(0, patterns.Length);
        return patterns[index];
    }

    IEnumerator Pattern_N0(int enemyCount) // 랜덤 위치에서 한마리씩 내려오게
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-2f, 2f), 5.5f, 0); // Y 좌표 유지
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            StartCoroutine(MoveDownAndDestroy(enemy));

            yield return new WaitForSeconds(0.7f); // 스폰 간격
        }

        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return null;
        }
    }

    IEnumerator Pattern_N1(int enemyCount) // 좌에서 우로
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = new Vector3(-3f, 3.8f, 0); // X=-3에서 생성
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            StartCoroutine(MoveLeftRight(enemy)); // 생성 후 즉시 이동

            yield return new WaitForSeconds(0.3f); // 스폰 간격
        }

        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return null;
        }
    }

    IEnumerator Pattern_N2(int enemyCount) // 우에서 좌로
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = new Vector3(3f, 3.8f, 0); // X=3에서 생성
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            StartCoroutine(MoveRightLeft(enemy)); // 생성 후 즉시 이동

            yield return new WaitForSeconds(0.3f); // 스폰 간격
        }

        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return null;
        }
    }

    IEnumerator Pattern_N98(int enemyCount) // 시계 방향 회전
    {
        float rotationSpeed = 72f; // 기본 회전 속도 (초당 90도)

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = new Vector3(0, 5.5f, 0); // X=0, Y=5.5에서 생성
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            StartCoroutine(MoveInEllipseWithSpeed(enemy, false, rotationSpeed)); // 생성 후 이동 시작

            yield return new WaitForSeconds(0.5f); // 스폰 간격
        }

        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return null;
        }
    }


    IEnumerator Pattern_N99(int enemyCount) // 반시계 방향
    {
        float rotationSpeed = 72f; // 기본 회전 속도 (초당 90도)

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = new Vector3(0, 5.5f, 0); // X=0, Y=5.5에서 생성
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            StartCoroutine(MoveInEllipseWithSpeed(enemy, true, rotationSpeed)); // 생성 후 이동 시작

            yield return new WaitForSeconds(0.5f); // 스폰 간격
        }

        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return null;
        }
    }

    IEnumerator Pattern_N5(int enemyCount) { // 이 다음부터는 패턴 추가 예정 오류 나서 N_4로 대체
        float rotationSpeed = 72f; // 기본 회전 속도 (초당 90도)

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = new Vector3(0, 5.5f, 0); // X=0, Y=5.5에서 생성
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            StartCoroutine(MoveInEllipseWithSpeed(enemy, true, rotationSpeed)); // 생성 후 이동 시작

            yield return new WaitForSeconds(0.5f); // 스폰 간격
        }

        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return null;
        }
    }


    IEnumerator MoveDownAndDestroy(GameObject enemy) // N_0에서 사용
    {
        while (enemy != null && enemy.transform.position.y > -7f)
        {
            enemy.transform.position += Vector3.down * 2f * Time.deltaTime; // 속도
            yield return null; // 반환 값 없음 걍 -7 되면 while문 빠져나오기
        }

        if (enemy != null) Destroy(enemy); // -7 이하로 나오면 이거 실행 몹 없애기
    }

    IEnumerator MoveInEllipseWithSpeed(GameObject enemy, bool clockwise, float rotationSpeed) { // N_98, N_99에서 사용
        float angle = 0f;

        while (enemy != null)
        {
            angle += clockwise ? rotationSpeed * Time.deltaTime : -rotationSpeed * Time.deltaTime;

            float x = 2f * Mathf.Cos(angle * Mathf.Deg2Rad); // 타원의 X 좌표
            float y = 2.5f * Mathf.Sin(angle * Mathf.Deg2Rad) + 2.5f; // 타원의 Y 좌표
            // 감이 안와서 gpt 사용 이 새끼 존나 똑똑함 sin, cos 사용
            enemy.transform.position = new Vector3(x, y, 0);

            yield return null;
        }
    }


    IEnumerator MoveLeftRight(GameObject enemy) { // N_1에서 사용
        float speed = 2f; // 이동 속도
        float direction = 1f; // 오른쪽 이동 시작
        float leftLimit = -3f; // 좌측 끝
        float rightLimit = 3f; // 우측 끝

        while (enemy != null) // 반복
        {
            enemy.transform.position += new Vector3(direction * speed * Time.deltaTime, 0, 0);

            // 오른쪽 끝에 도달하면 방향 전환
            if (enemy.transform.position.x >= rightLimit)
            {
                direction = -1f; // 왼쪽으로 이동
            }

            // 왼쪽 끝에 도달하면 방향 전환
            if (enemy.transform.position.x <= leftLimit)
            {
                direction = 1f; // 오른쪽으로 이동
            }

            yield return null;
        }
    }

    IEnumerator MoveRightLeft(GameObject enemy) { // N_2에서 사용
        float speed = 2f; // 이동 속도
        float direction = -1f; // 왼쪽 이동 시작
        float leftLimit = -3f; // 좌측 끝
        float rightLimit = 3f; // 우측 끝

        while (enemy != null)
        {
            enemy.transform.position += new Vector3(direction * speed * Time.deltaTime, 0, 0);

            // 왼쪽 끝에 도달하면 방향 전환
            if (enemy.transform.position.x <= leftLimit)
            {
                direction = 1f; // 오른쪽으로 이동
            }

            // 오른쪽 끝에 도달하면 방향 전환
            if (enemy.transform.position.x >= rightLimit)
            {
                direction = -1f; // 왼쪽으로 이동
            }

            yield return null;
        }
    }
}
