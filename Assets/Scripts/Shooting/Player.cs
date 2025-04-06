using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Transform shootTransform;
    [SerializeField]
    private float gunShootInterval = 0.2f; // 총 발사 속도
    private float lastGunShootTime;

    private int gunLevel = 1;
    private float autoUpgradeTimer = 0f;
    private const float autoUpgradeDelay = 22f;
    private struct GunFireInfo
    {
        public Vector3 offset;
        public string weaponType; // 예: "Bullet" 또는 "Laser"
        public GunFireInfo(Vector3 offset, string weaponType)
        {
            this.offset = offset;
            this.weaponType = weaponType;
        }
    }

    // 미리 계산된 발사 패턴 (Vector 연산 최소화)
    private static readonly GunFireInfo[] level1Pattern = new GunFireInfo[]
    {
        new GunFireInfo(Vector3.zero, "Bullet")
    };
    private static readonly GunFireInfo[] level2Pattern = new GunFireInfo[]
    {
        new GunFireInfo(new Vector3(-0.2f, -0.1f, 0f), "Bullet"),
        new GunFireInfo(new Vector3(0.2f, 0.1f, 0f), "Bullet")
    };
    private static readonly GunFireInfo[] level3Pattern = new GunFireInfo[]
    {
        new GunFireInfo(new Vector3(-3f, -0.1f, 0f), "Bullet"),
        new GunFireInfo(Vector3.zero, "Bullet1"), // 가운데는 다른 총알 발사
        new GunFireInfo(new Vector3(3f, -0.1f, 0f), "Bullet")
    };
    private static readonly GunFireInfo[] level4Pattern = new GunFireInfo[]
    {
        new GunFireInfo(new Vector3(-5f, -0.2f, 0f), "Bullet"),
        new GunFireInfo(new Vector3(-3f, -0.1f, 0f), "Bullet1"),
        new GunFireInfo(Vector3.zero, "Bullet1"),
        new GunFireInfo(new Vector3(3f, -0.1f, 0f), "Bullet1"),
        new GunFireInfo(new Vector3(5f, -0.2f, 0f), "Bullet")
    };
    public bool canShoot = true;  // true일 때만 총알/미사일 발사
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

    [SerializeField]
    private float collisionCheckRadius = 0.2f;

    [SerializeField]
    private LayerMask wallLayer;


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

        // 자동 업그레이드 타이머: gunLevel이 3일 때 22초 경과하면 자동으로 4단계로 업그레이드
        if (gunLevel == 3)
        {
            autoUpgradeTimer += Time.deltaTime;
            if (autoUpgradeTimer >= autoUpgradeDelay)
            {
                UpgradeGun(); // 레벨 3 → 4
                autoUpgradeTimer = 0f;
            }
        }
        else
        {
            autoUpgradeTimer = 0f;
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
            Vector3 nextPosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // 다음 위치에 벽 레이어에 해당하는 오브젝트와 충돌하는지 검사
            Collider2D wallCollision = Physics2D.OverlapCircle(nextPosition, collisionCheckRadius, wallLayer);
            if (wallCollision != null)
            {
                // 벽과 충돌하면 이동하지 않음
                return;
            }
            transform.position = nextPosition;
        }
    }

    void Shoot()
    {
        if (Time.time - lastGunShootTime > gunShootInterval)
        {
            FireGun();
            lastGunShootTime = Time.time;
        }
    }

    void FireGun()
    {
        GunFireInfo[] pattern = GetGunPattern();
        for (int i = 0; i < pattern.Length; i++)
        {
            GunFireInfo info = pattern[i];
            Vector3 spawnPos = shootTransform.position + info.offset;
            // 기본 회전은 identity, 단 gunLevel 4일 때 양옆 총알은 회전 적용
            Quaternion rotation = Quaternion.identity;
            if (gunLevel == 4)
            {
                if (i == 0)
                {
                    rotation = Quaternion.Euler(0f, 0f, -15f);
                }
                else if (i == pattern.Length - 1)
                {
                    rotation = Quaternion.Euler(0f, 0f, 15f);
                }
            }
            WeaponPool.Instance.SpawnWeapon(info.weaponType, spawnPos, rotation);
        }
    }

    GunFireInfo[] GetGunPattern()
    {
        if (gunLevel == 1)
            return level1Pattern;
        else if (gunLevel == 2)
            return level2Pattern;
        else if (gunLevel == 3)
            return level3Pattern;
        else
            return level4Pattern;
    }

    // 총알 업그레이드 (gunLevel 최대 5)
    public void UpgradeGun()
    {
        if (gunLevel < 3)
        {
            gunLevel++;
            Debug.Log("Gun upgraded via item to level " + gunLevel);
        }
        else if (gunLevel == 3)
        {
            gunLevel = 4;
            Debug.Log("Gun auto upgraded to max level " + gunLevel);
        }
        else
        {
            Debug.Log("Gun is already at maximum level.");
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


}
