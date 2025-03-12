using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    public bool canShoot = true;  // true일 때만 총알/미사일 발사

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
    private bool isTouching = false; // 터치 중인지 확인 
    public float moveSpeed = 5f; // 이동 속도
    private float originalMoveSpeed; // 원래 이동 속도 저장

    public GameObject gameOverPanel; // 게임 오버 패널

    // 쉴드 관련 필드
    public GameObject shieldImage;
    private bool isShieldActive = false;
    private Coroutine shieldCoroutine;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetPosition = transform.position;
        originalMoveSpeed = moveSpeed; // 원래 속도 저장

        // 쉴드 이미지가 있다면 비활성화
        if (shieldImage != null)
            shieldImage.SetActive(false);
    }

    void Update()
    {
        HandleMovement();
        if (canShoot)
        {
            Shoot();
        }
    }

    void HandleMovement()
    {
        if (Input.GetMouseButton(0))
        {
            isTouching = true;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = transform.position.z;
            targetPosition = mousePos;
        }
        else
        {
            isTouching = false;
        }
        if (Vector3.Distance(transform.position, targetPosition) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    void Shoot()
    {
        if (Time.time - lastGunShootTime > gunShootInterval)
        {
            FireGun();
            lastGunShootTime = Time.time;
        }
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
            Instantiate(gunProjectile, spawnPosition, Quaternion.identity);
        }
    }

    void FireMissile()
    {
        Vector3[] missilePositions = GetMissilePositions();
        foreach (Vector3 pos in missilePositions)
        {
            Instantiate(missileProjectile, shootTransform.position + pos, Quaternion.Euler(0, 0, 90));
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

    // 새로운 메서드들

    // 총알 업그레이드 (gunLevel 최대 5)
    public void UpgradeGun()
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

    // 미사일 업그레이드 (missileLevel 최대 2)
    public void UpgradeMissile()
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

    // 체력 회복 (health가 3 미만이면 1 회복)
    public void RecoverHealth()
    {
        if (health < 3)
        {
            health += 1;
            Debug.Log("체력 회복, 현재 체력: " + health);
        }
        else
        {
            Debug.Log("체력이 이미 최대입니다.");
        }
    }

    // 쉴드 활성화: 30초 동안 유지, 이미 있으면 시간을 30초로 리셋
    public void ActivateShield()
    {
        if (isShieldActive)
        {
            // 이미 쉴드가 활성화되어 있으면, 기존 코루틴을 중단하고 시간을 리셋
            if (shieldCoroutine != null)
                StopCoroutine(shieldCoroutine);
            shieldCoroutine = StartCoroutine(ShieldDuration());
            Debug.Log("쉴드 시간 리셋");
        }
        else
        {
            isShieldActive = true;
            if (shieldImage != null)
                shieldImage.SetActive(true);
            shieldCoroutine = StartCoroutine(ShieldDuration());
            Debug.Log("쉴드 활성화");
        }
    }

    private IEnumerator ShieldDuration()
    {
        yield return new WaitForSeconds(30f);
        isShieldActive = false;
        if (shieldImage != null)
            shieldImage.SetActive(false);
        Debug.Log("쉴드 종료");
    }

    // 피격 처리: 쉴드가 활성화되어 있으면 데미지를 받지 않고 쉴드를 제거
    public void TakeDamage(int damage)
    {
        if (isInvincible)
            return;
        // 쉴드가 활성화되어 있으면 피격 시 쉴드를 제거하고 데미지 무시
        if (isShieldActive)
        {
            isShieldActive = false;
            if (shieldImage != null)
                shieldImage.SetActive(false);
            if (shieldCoroutine != null)
            {
                StopCoroutine(shieldCoroutine);
                shieldCoroutine = null;
            }
            Debug.Log("쉴드가 공격을 막음");
            return;
        }
        health -= damage;
        Debug.Log($"플레이어 체력: {health}");
        if (health <= 0)
        {
            Debug.Log("플레이어 사망!");
            GameOver();
        }
        else
        {
            StartCoroutine(Invincibility());
        }
    }
    public void ApplySpeedReduction(float reductionPercent, float duration = 3f)
    {
        // 만약 이전에 실행 중인 효과가 있다면 중단
        StopCoroutine("RestoreSpeed");
        moveSpeed = originalMoveSpeed * (1 - reductionPercent);
        StartCoroutine(RestoreSpeed(duration));
    }

    private IEnumerator RestoreSpeed(float duration)
    {
        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed;
    }

    private void GameOver()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.CheckAndUpdateHighScore();

        if (GameOverPanelController.Instance != null)
            GameOverPanelController.Instance.ShowGameOverPanel();

        gameObject.SetActive(false);
    }

    private IEnumerator Invincibility()
    {
        isInvincible = true;
        for (int i = 0; i < 5; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        isInvincible = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && enemy.enemyType != 4)
                TakeDamage(1);
        }
    }
    public void SetBulletScale(float factor)
    {
        if (gunProjectile != null)
        {
            gunProjectile.transform.localScale = new Vector3(factor, factor, 1f);
        }
        if (missileProjectile != null)
        {
            missileProjectile.transform.localScale = new Vector3(factor, factor, 1f);
        }
    }

}
