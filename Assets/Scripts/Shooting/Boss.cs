using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Boss Settings")]
    public float maxHP = 10000f;
    private float currentHP;

    private bool phase2Started = false;  // (HP 9000 이하)
    private bool phase3Started = false;  // (HP 7500 이하)
    private bool phase4Started = false;  // (HP 5000 이하)
    private bool phase5Started = false;  // (HP 2500 이하)
    private bool phase6Started = false;  // HP 500 이하
    private bool isInvincible = false;   // 무적 여부

    [Header("Appear Settings")]
    public float appearTime = 3f;    // 3초 동안 등장 (z=-5→0, 스케일 0→원래)
    public float startZ = -5f;
    public float endZ = 0f;
    private Vector3 originalScale;
    private Coroutine p23Routine;
    public bool forceStopPattern = false;
    public float CurrentHP { get { return currentHP; } }

    [Header("References")]
    public Player player;  // 플레이어 참조 (Inspector에서 할당)

    void Awake()
    {
        currentHP = maxHP;
        // 보스 원래 스케일 저장
        originalScale = transform.localScale;

        // 초기: z=-5, 스케일(0,0,0)
        Vector3 pos = transform.position;
        pos.z = startZ;
        transform.position = pos;

        transform.localScale = Vector3.zero;
    }

    void Start()
    {
        if (player == null)
        {
            player = Object.FindAnyObjectByType<Player>();
        }
        // 1) 처음 등장 (z전진 + 스케일 0→원래)
        StartCoroutine(StartBossSequence());
    }

    // -------------------------
    // (A) 처음 보스 등장 시퀀스
    // -------------------------
    private IEnumerator StartBossSequence()
    {
        // 플레이어 공격 비활성화
        if (player != null)
            player.canShoot = false;

        float elapsed = 0f;
        Vector3 startScale = Vector3.zero;

        while (elapsed < appearTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / appearTime);

            // z: -5 → 0
            float currentZ = Mathf.Lerp(startZ, endZ, t);
            Vector3 pos = transform.position;
            pos.z = currentZ;
            transform.position = pos;

            // 스케일: (0,0,0) → originalScale
            transform.localScale = Vector3.Lerp(startScale, originalScale, t);

            yield return null;
        }
        // 보정
        Vector3 finalPos = transform.position;
        finalPos.z = endZ;
        transform.position = finalPos;
        transform.localScale = originalScale;

        // 플레이어 공격 다시 가능
        if (player != null)
            player.canShoot = true;

        // 기본 패턴(P_23) 시작
        p23Routine = StartCoroutine(PatternManager.Instance.ExecutePattern(gameObject, new string[] { "P_23B" }));
    }

    // -------------------------
    // (B) 보스 데미지 처리
    // -------------------------
    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        currentHP -= damage;
        Debug.Log($"보스 HP: {currentHP}");

        // Phase2 (HP 9000 이하)
        if (!phase2Started && currentHP <= 9000f)
        {
            phase2Started = true;
            if (p23Routine != null)
            {
                StopCoroutine(p23Routine);
                p23Routine = null;
                Debug.Log("P_23 패턴 중단됨");
            }
            StartCoroutine(Phase2_MovePattern());
        }

        // Phase3 (HP 7500 이하)
        if (!phase3Started && currentHP <= 7500f)
        {
            phase3Started = true;
            // 혹시 남아 있는 코루틴이 있다면 중단
            if (p23Routine != null)
            {
                forceStopPattern = true;
                StopCoroutine(p23Routine);
                p23Routine = null;
            }
            forceStopPattern = false;
            StartCoroutine(Phase3_InvinciblePattern());
        }

        // Phase4 (HP 5000 이하)
        if (!phase4Started && currentHP <= 5000f)
        {
            phase4Started = true;
            StartCoroutine(Phase4_ReverseAndShrink());
        }

        // Phase5 (HP 2500 이하)
        if (!phase5Started && currentHP <= 2500f)
        {
            phase5Started = true;
            StartCoroutine(Phase5_ShrinkMoreAndPattern());
        }

        // Phase6: HP 500 이하
        if (!phase6Started && currentHP <= 500f)
        {
            phase6Started = true;
            StartCoroutine(Phase6_FinalPattern());
        }

        // 사망 처리
        if (currentHP <= 0)
        {
            Debug.Log("보스 사망!");
            Destroy(gameObject);
        }
    }

    // -------------------------
    // Phase2 (HP 9000 이하)
    // -------------------------
    private IEnumerator Phase2_MovePattern()
    {
        Debug.Log("보스 Phase2 시작 (기존 로직)");
        int repeatCount = 5;
        for (int i = 0; i < repeatCount; i++)
        {
            // (3, 2.5)로 1초 이동, 3초 대기
            yield return StartCoroutine(MoveToPosition(new Vector3(3f, 2.5f, transform.position.z), 1f));
            yield return new WaitForSeconds(3f);

            // (-3, 2.5)로 1초 이동, 3초 대기
            yield return StartCoroutine(MoveToPosition(new Vector3(-3f, 2.5f, transform.position.z), 1f));
            yield return new WaitForSeconds(3f);

            // (0, 2.5)로 1초 이동, 5초 대기
            yield return StartCoroutine(MoveToPosition(new Vector3(0f, 2.5f, transform.position.z), 1f));
            yield return new WaitForSeconds(5f);

            if (phase3Started) yield break;
        }
    }

    // -------------------------
    // Phase3 (HP 7500 이하)
    // -------------------------
    private IEnumerator Phase3_InvinciblePattern()
    {
        Debug.Log("보스 Phase3 시작: 무적 & P_25R_boss 단 한 번 수행");
        isInvincible = true;

        // 보스 전용 P_25R_boss 패턴 1회 수행
        yield return StartCoroutine(PatternManager.Instance.ExecutePattern(gameObject, new string[] { "P_25R_boss" }));

        // (0, 2.5)로 1초 이동
        yield return StartCoroutine(MoveToPosition(new Vector3(0f, 2.5f, transform.position.z), 1f));

        // 필요한 경우 무적 해제 처리
        isInvincible = false;
        Debug.Log("보스 Phase3 완료");
    }


    // -------------------------
    // Phase4 (HP 5000 이하)
    // -------------------------
    private IEnumerator Phase4_ReverseAndShrink()
    {
        Debug.Log("보스 Phase4 시작: 배경 위로 스크롤, 플레이어/보스/총알 1/4 크기로, 패턴 5회");

        // 1) 배경 반대로 스크롤
        //    예: BackGround 스크립트 찾아서 moveSpeed 음수 → 양수, 혹은 reverseScrolling = true 등
        var bgs = Object.FindObjectsByType<BackGround>(FindObjectsSortMode.None);
        foreach (var bg in bgs)
        {
            // 예: bg.moveSpeed = -Mathf.Abs(bg.moveSpeed);
            // 또는 bg.SetReverse(true);
            bg.moveSpeed = Mathf.Abs(bg.moveSpeed); // (기존이 3f 아래로라면, 3f 위로)
        }

        // 2) 플레이어 / 보스 / 총알 크기 1/4
        if (player != null)
        {
            // 플레이어 크기
            player.transform.localScale *= 0.25f;

            // 총알, 미사일 프리팹도 축소(Instantiate 시점에 반영)
            player.SetBulletScale(0.25f);
            // ↑ 예: Player.cs에 public void SetBulletScale(float factor) { gunProjectile.transform.localScale = new Vector3(factor, factor, 1f); missileProjectile.transform.localScale = ... }
        }
        // 보스도 1/4
        transform.localScale *= 0.25f;

        // 3) 이동 패턴 (5회 반복)
        //    0.75,3 → -0.75,3 → 0,3 → 1.75,1 → 2.75,1 (각각 0.3초 이동 + 4초 대기)
        int repeatCount = 5;
        for (int i = 0; i < repeatCount; i++)
        {
            yield return StartCoroutine(MoveToPosition(new Vector3(0.75f, 3f, transform.position.z), 0.3f));
            yield return new WaitForSeconds(4f);

            yield return StartCoroutine(MoveToPosition(new Vector3(-0.75f, 3f, transform.position.z), 0.3f));
            yield return new WaitForSeconds(4f);

            yield return StartCoroutine(MoveToPosition(new Vector3(0f, 3f, transform.position.z), 0.3f));
            yield return new WaitForSeconds(4f);

            yield return StartCoroutine(MoveToPosition(new Vector3(1.75f, 1f, transform.position.z), 0.3f));
            yield return new WaitForSeconds(4f);

            yield return StartCoroutine(MoveToPosition(new Vector3(2.75f, 1f, transform.position.z), 0.3f));
            yield return new WaitForSeconds(4f);

            // 만약 이 도중에 HP가 2500 이하로 떨어져 Phase5가 시작되면 중단 가능
            if (phase5Started) yield break;
        }
    }

    // -------------------------
    // Phase5 (HP 2500 이하)
    // -------------------------
    private IEnumerator Phase5_ShrinkMoreAndPattern()
    {
        Debug.Log("보스 Phase5 시작: 보스 1/4 추가 축소, 새 패턴");

        // 보스 크기 다시 1/4 (지금 상태에서 또 1/4이면 결과적으로 이전의 1/16)
        transform.localScale *= 0.25f;

        // 패턴 진행
        // 1) (0,2.5)로 0.3초 이동, 3초 대기
        yield return StartCoroutine(MoveToPosition(new Vector3(0f, 2.5f, transform.position.z), 0.3f));
        yield return new WaitForSeconds(3f);

        // 2) P_16 2번 수행
        //    PatternManager에 ExecutePattern으로 예: "P_16", "P_16"
        yield return StartCoroutine(PatternManager.Instance.ExecutePattern(
            gameObject,
            new string[] { "P_16", "P_16" }
        ));

        // 3) (-1,2.5)로 0.3초 이동, 3초 대기
        yield return StartCoroutine(MoveToPosition(new Vector3(-1f, 2.5f, transform.position.z), 0.3f));
        yield return new WaitForSeconds(3f);

        // 4) P_17 2번
        yield return StartCoroutine(PatternManager.Instance.ExecutePattern(
            gameObject,
            new string[] { "P_17", "P_17" }
        ));

        // 5) P_0 4번
        yield return StartCoroutine(PatternManager.Instance.ExecutePattern(
            gameObject,
            new string[] { "P_0", "P_0", "P_0", "P_0" }
        ));

        // 6) (1,3.5)로 0.3초 이동, 2초 대기
        yield return StartCoroutine(MoveToPosition(new Vector3(1f, 3.5f, transform.position.z), 0.3f));
        yield return new WaitForSeconds(2f);

        // 7) P_20 수행
        yield return StartCoroutine(PatternManager.Instance.ExecutePattern(
            gameObject,
            new string[] { "P_20" }
        ));

        Debug.Log("보스 Phase5 완료");
    }

    // -------------------------
    // Phase6 (HP 500 이하)
    // -------------------------
    private IEnumerator Phase6_FinalPattern()
    {
        Debug.Log("보스 Phase6 시작: HP 500 이하");

        // 1) (0, 0.5)로 0.3초 동안 이동
        yield return StartCoroutine(MoveToPosition(new Vector3(0f, 0.5f, transform.position.z), 0.3f));

        // 2) P_6 패턴 3번 수행
        yield return StartCoroutine(
            PatternManager.Instance.ExecutePattern(gameObject, new string[] { "P_6", "P_6", "P_6" })
        );

        // 3) P_7 6번 → P_6 6번을 10회 반복
        for (int i = 0; i < 10; i++)
        {
            // P_7 x6
            string[] p7_6 = new string[6];
            for (int j = 0; j < 6; j++)
                p7_6[j] = "P_7";
            yield return StartCoroutine(PatternManager.Instance.ExecutePattern(gameObject, p7_6));

            // P_6 x6
            string[] p6_6 = new string[6];
            for (int j = 0; j < 6; j++)
                p6_6[j] = "P_6";
            yield return StartCoroutine(PatternManager.Instance.ExecutePattern(gameObject, p6_6));
        }

        Debug.Log("보스 Phase6 완료");
    }

    // -------------------------
    // (E) 보조: 선형 이동
    // -------------------------
    private IEnumerator MoveToPosition(Vector3 targetPos, float duration)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }
}
