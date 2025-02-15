using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject gunProjectile; // 총알 프리팹
    [SerializeField]
    private GameObject missileProjectile; // 미사일 프리팹

    [SerializeField]
    private Transform shootTransform;

    [SerializeField]
    private float gunShootInterval = 0.2f; // 총 발사 속도
    private float lastGunShootTime;

    [SerializeField]
    private float missileShootInterval = 2.0f; // 미사일 발사 속도
    private float lastMissileShootTime;

    private int gunLevel = 1; // 기본 1단계
    private int missileLevel = 0; // 미사일 없음

    public int health = 3; // 플레이어 체력
    public float invincibilityDuration = 0.5f; // 무적 지속 시간
    private bool isInvincible = false; // 무적 상태 여부
    private SpriteRenderer spriteRenderer; // 플레이어의 스프라이트 렌더러 (플래시 효과용)

    private Vector3 targetPosition; // 목표 위치
    public float moveSpeed = 5f; // 이동 속도

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // 스프라이트 렌더러 가져오기
        targetPosition = transform.position; // 초기 목표 위치는 현재 위치
    }

    void Update()
    {
        HandleMovement(); // 클릭에 따른 이동 처리
        Shoot(); // 총알 및 미사일 발사
    }

    void HandleMovement()
    {
        // 클릭한 위치를 목표 위치로 설정
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭 (터치에서도 동작)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = new Vector3(
                Mathf.Clamp(mousePos.x, -1.75f, 1.75f), 
                Mathf.Clamp(mousePos.y, -4.5f, 4.5f), 
                transform.position.z
            );
        }

        // 목표 위치로 부드럽게 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void Shoot()
    {
        // 총 발사 (0.2초 간격)
        if (Time.time - lastGunShootTime > gunShootInterval)
        {
            FireGun();
            lastGunShootTime = Time.time;
        }

        // 미사일 발사 (2초 간격)
        if (missileLevel > 0 && Time.time - lastMissileShootTime > missileShootInterval)
        {
            FireMissile();
            lastMissileShootTime = Time.time;
        }
    }

    void FireGun()
    {
        float[] gunPositions = GetGunPositions();

        foreach (float xOffset in gunPositions)
        {
            Vector3 spawnPosition = shootTransform.position + new Vector3(xOffset, 0, 0);
            Instantiate(gunProjectile, spawnPosition, Quaternion.Euler(0, 0, 0)); // 위로 발사
        }
    }

    void FireMissile()
    {
        Vector3[] missilePositions = GetMissilePositions();

        foreach (Vector3 pos in missilePositions)
        {
            Instantiate(missileProjectile, shootTransform.position + pos, Quaternion.Euler(0, 0, 90)); // 위로 발사
        }
    }

    float[] GetGunPositions()
    {
        switch (gunLevel)
        {
            case 1: return new float[] { 0 };
            case 2: return new float[] { -0.2f, 0.2f };
            case 3: return new float[] { -0.4f, 0, 0.4f };
            case 4: return new float[] { -0.6f, -0.2f, 0.2f, 0.6f };
            case 5: return new float[] { -0.8f, -0.4f, 0, 0.4f, 0.8f };
            default: return new float[] { 0 };
        }
    }

    Vector3[] GetMissilePositions()
    {
        switch (missileLevel)
        {
            case 1: return new Vector3[] { new Vector3(0.9f, -0.5f, 0) };
            case 2: return new Vector3[] { new Vector3(-0.9f, -0.5f, 0), new Vector3(0.9f, -0.5f, 0) };
            default: return new Vector3[] { };
        }
    }

    public void UpgradeWeapon()
    {
        float rand = Random.value; // 0 ~ 1 사이 랜덤 값

        if (rand < 0.6f) // 60% 확률로 총 업그레이드
        {
            if (gunLevel < 5)
            {
                gunLevel++;
                Debug.Log("총 업그레이드! 현재 총알 개수: " + gunLevel);
            }
            else
            {
                Debug.Log("총이 최대 레벨입니다!");
            }
        }
        else // 40% 확률로 미사일 추가 (최대 2단계까지만)
        {
            if (missileLevel == 0)
            {
                missileLevel = 1;
                Debug.Log("미사일 획득!");
            }
            else if (missileLevel < 2)
            {
                missileLevel++;
                Debug.Log("미사일 업그레이드! 현재 미사일 개수: " + missileLevel);
            }
            else
            {
                Debug.Log("미사일이 최대 레벨입니다!");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return; // 무적 상태에서는 데미지 무시

        health -= damage;
        Debug.Log($"플레이어 체력: {health}");

        if (health <= 0)
        {
            Debug.Log("플레이어 사망!");
            // 게임 오버 처리
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(Invincibility()); // 무적 상태 활성화
        }
    }

    private IEnumerator Invincibility()
    {
        isInvincible = true;

        // 스프라이트 플래시 효과 (깜빡임)
        for (int i = 0; i < 5; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        isInvincible = false; // 무적 상태 해제
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // 적과 충돌
        {
            TakeDamage(1); // 체력 1 감소
        }
    }
}
