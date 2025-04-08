using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum ActiveWeapon { Gun, Raser }
    public ActiveWeapon currentWeapon = ActiveWeapon.Gun;

    [Header("Shoot Transform & Timings")]
    [SerializeField] private Transform shootTransform;
    [SerializeField] private float gunShootInterval = 0.2f; // Gun 발사 간격
    private float lastGunShootTime = 0f;

    [SerializeField] private float raserShootInterval = 1f; // Raser 발사 간격 (raserLevel < 4)
    private float lastRaserShootTime = 0f;

    private const float autoUpgradeDelay = 22f;
    private float autoUpgradeTimerGun = 0f;
    private float autoUpgradeTimerRaser = 0f;

    // 각 무기는 수동으로 최대 3단계까지 업그레이드되며, 3단계 상태에서 타이머 경과 시 자동으로 4단계가 됩니다.
    private int gunLevel = 1;
    private int raserLevel = 1;

    private GameObject persistentUBeam = null;
    private GameObject persistentBBeam = null;

    // GunFireInfo 구조체: 발사 시 사용할 오프셋과 무기 타입을 정의
    private struct GunFireInfo
    {
        public Vector3 offset;
        public string weaponType;
        public GunFireInfo(Vector3 offset, string weaponType)
        {
            this.offset = offset;
            this.weaponType = weaponType;
        }
    }

    // ── Gun 발사 패턴 ──
    private static readonly GunFireInfo[] level1Pattern = new GunFireInfo[]
    {
        new GunFireInfo(Vector3.zero, "Bullet")
    };
    private static readonly GunFireInfo[] level2Pattern = new GunFireInfo[]
    {
        new GunFireInfo(new Vector3(-0.2f, -0.1f, 0f), "Bullet"),
        new GunFireInfo(new Vector3(0.2f, -0.1f, 0f), "Bullet")
    };
    private static readonly GunFireInfo[] level3Pattern = new GunFireInfo[]
    {
        new GunFireInfo(new Vector3(-0.3f, -0.1f, 0f), "Bullet"),
        new GunFireInfo(Vector3.zero, "Bullet1"), // 원래 "Bullet1"이었으나 모두 "Bullet"으로 사용
        new GunFireInfo(new Vector3(0.3f, -0.1f, 0f), "Bullet")
    };
    private static readonly GunFireInfo[] level4Pattern = new GunFireInfo[]
    {
        new GunFireInfo(new Vector3(-0.5f, -0.2f, 0f), "Bullet"),
        new GunFireInfo(new Vector3(-0.3f, -0.1f, 0f), "Bullet1"),
        new GunFireInfo(Vector3.zero, "Bullet1"),
        new GunFireInfo(new Vector3(0.3f, -0.1f, 0f), "Bullet1"),
        new GunFireInfo(new Vector3(0.5f, -0.2f, 0f), "Bullet")
    };

    // ── Raser(레이저) 발사 패턴 ──
    // raserLevel 1: Beam 1개
    private static readonly GunFireInfo[] raserLevel1Pattern = new GunFireInfo[]
    {
        new GunFireInfo(new Vector3(0f,5f,0f), "Beam")
    };
    // raserLevel 2: Beam 2개 → 좌표 (0.5,5,0)와 (-0.5,5,0)
    private static readonly GunFireInfo[] raserLevel2Pattern = new GunFireInfo[]
    {
        new GunFireInfo(new Vector3(0.5f, 5f, 0f), "Beam"),
        new GunFireInfo(new Vector3(-0.5f, 5f, 0f), "Beam")
    };
    // raserLevel 3: Beam 1 + BBeam 1, 모두 (0,5,0)
    private static readonly GunFireInfo[] raserLevel3Pattern = new GunFireInfo[]
    {
        new GunFireInfo(new Vector3(0f,5f,0f), "Beam"),
        new GunFireInfo(new Vector3(0f,5f,0f), "BBeam")
    };
    // raserLevel 4: UBeam 1 + BBeam 1, 모두 (0,5,0) → 지속 발사 (쿨다운 없이)
    private static readonly GunFireInfo[] raserLevel4Pattern = new GunFireInfo[]
    {
        new GunFireInfo(new Vector3(0f,5f,0f), "UBeam"),
        new GunFireInfo(new Vector3(0f,5f,0f), "BBeam")
    };

    [Header("Player Movement & Other Settings")]
    public bool canShoot = true;
    public int health = 3;
    public float invincibilityDuration = 0.5f;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;
    private Vector3 targetPosition;
    private bool isTouching = false;
    public float moveSpeed = 5f;
    private float originalMoveSpeed;
    public GameObject gameOverPanel;

    // 쉴드 관련
    public GameObject shieldImage;
    private bool isShieldActive = false;
    private Coroutine shieldCoroutine;

    [SerializeField] private float collisionCheckRadius = 0.2f;
    [SerializeField] private LayerMask wallLayer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetPosition = transform.position;
        originalMoveSpeed = moveSpeed;
        if (shieldImage != null)
            shieldImage.SetActive(false);
    }

    void Update()
    {
        HandleMovement();

        if (currentWeapon == ActiveWeapon.Gun && canShoot)
        {
            if (Time.time - lastGunShootTime > gunShootInterval)
            {
                FireGun();
                lastGunShootTime = Time.time;
            }
            if (gunLevel == 3)
            {
                autoUpgradeTimerGun += Time.deltaTime;
                if (autoUpgradeTimerGun >= autoUpgradeDelay)
                {
                    AutoUpgradeGunToMax();
                    autoUpgradeTimerGun = 0f;
                }
            }
            else autoUpgradeTimerGun = 0f;
        }
        else if (currentWeapon == ActiveWeapon.Raser && canShoot)
        {
            if (raserLevel < 4)
            {
                if (Time.time - lastRaserShootTime > raserShootInterval)
                {
                    FireRaser();
                    lastRaserShootTime = Time.time;
                }
            }
            else
            {
                // raserLevel 4: 계속 발사 (쿨다운 없음)
                FireRaser();
            }
            if (raserLevel == 3)
            {
                autoUpgradeTimerRaser += Time.deltaTime;
                if (autoUpgradeTimerRaser >= autoUpgradeDelay)
                {
                    AutoUpgradeRaserToMax();
                    autoUpgradeTimerRaser = 0f;
                }
            }
            else autoUpgradeTimerRaser = 0f;
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
            Collider2D wallCollision = Physics2D.OverlapCircle(nextPosition, collisionCheckRadius, wallLayer);
            if (wallCollision != null)
                return;
            transform.position = nextPosition;
        }
    }

    // ── Gun 발사 ──
    void FireGun()
    {
        GunFireInfo[] pattern = GetGunPattern();
        Quaternion baseRotation = shootTransform.rotation;
        for (int i = 0; i < pattern.Length; i++)
        {
            GunFireInfo info = pattern[i];
            Vector3 spawnPos = shootTransform.position + info.offset;
            Quaternion rotation = baseRotation;
            if (gunLevel == 4)
            {
                // 4단계일 때, 좌측(인덱스 0)는 -15°, 우측(마지막 인덱스)은 +15° 상대 회전 적용
                if (i == 0)
                    rotation = baseRotation * Quaternion.Euler(0f, 0f, 15f);
                else if (i == pattern.Length - 1)
                    rotation = baseRotation * Quaternion.Euler(0f, 0f, -15f);
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

    // ── Raser 발사 ──
    void FireRaser()
    {
        if (raserLevel == 4)
        {
            if (persistentUBeam == null)
            {
                persistentUBeam = WeaponPool.Instance.SpawnWeapon("UBeam", shootTransform.position, shootTransform.rotation);
                Weapon w = persistentUBeam.GetComponent<Weapon>();
                if (w != null)
                    w.followPlayer = true;
            }
            if (persistentBBeam == null)
            {
                persistentBBeam = WeaponPool.Instance.SpawnWeapon("BBeam", shootTransform.position, shootTransform.rotation);
                Weapon w = persistentBBeam.GetComponent<Weapon>();
                if (w != null)
                    w.followPlayer = true;
            }
        }
        else
        {
            GunFireInfo[] pattern = GetRaserPattern();
            Quaternion baseRotation = shootTransform.rotation;
            for (int i = 0; i < pattern.Length; i++)
            {
                GunFireInfo info = pattern[i];
                Vector3 spawnPos = shootTransform.position + info.offset;
                GameObject laserObj = WeaponPool.Instance.SpawnWeapon(info.weaponType, spawnPos, baseRotation);
                // 모든 레이저(Beam, BBeam 등)를 플레이어 따라다니게 설정
                Weapon w = laserObj.GetComponent<Weapon>();
                if (w != null)
                    w.followPlayer = true;
            }
        }
    }




    GunFireInfo[] GetRaserPattern()
    {
        if (raserLevel == 1)
            return raserLevel1Pattern;
        else if (raserLevel == 2)
            return raserLevel2Pattern;
        else if (raserLevel == 3)
            return raserLevel3Pattern;
        else
            return raserLevel4Pattern;
    }

    // ── 수동 업그레이드 (UpgradeItem 호출용): 현재 무기 타입이 요청과 같으면 해당 무기의 레벨을 1~3까지만 올림,
    // 서로 다른 무기 타입이면 무기를 전환하고 현재 저장된 레벨을 그대로 사용함.
    public void UpgradeWeapon(ActiveWeapon requestedWeapon)
    {
        if (currentWeapon == requestedWeapon)
        {
            if (requestedWeapon == ActiveWeapon.Gun)
            {
                if (gunLevel < 3)
                {
                    gunLevel++;
                    Debug.Log("Gun upgraded via item to level " + gunLevel);
                }
                else
                {
                    Debug.Log("Gun is at maximum manual level (3).");
                }
            }
            else // ActiveWeapon.Raser
            {
                if (raserLevel < 3)
                {
                    raserLevel++;
                    Debug.Log("Raser upgraded via item to level " + raserLevel);
                }
                else
                {
                    Debug.Log("Raser is at maximum manual level (3).");
                }
            }
        }
        else if (currentWeapon != requestedWeapon)
        {
            // 전환 시, 현재 Raser persistent 오브젝트가 있다면 반환하고 null로 초기화
            if (currentWeapon == ActiveWeapon.Raser)
            {
                if (persistentUBeam != null)
                {
                    WeaponPool.Instance.ReturnWeapon("UBeam", persistentUBeam);
                    persistentUBeam = null;
                }
                if (persistentBBeam != null)
                {
                    WeaponPool.Instance.ReturnWeapon("BBeam", persistentBBeam);
                    persistentBBeam = null;
                }
            }
            currentWeapon = requestedWeapon;
            Debug.Log("Switched weapon to " + currentWeapon.ToString() + " at level " +
                      (currentWeapon == ActiveWeapon.Gun ? gunLevel : raserLevel));
            
            // 먹은 무기 레벨 업
            if (requestedWeapon == ActiveWeapon.Gun)
            {
                if (gunLevel < 3)
                    gunLevel++;
            }
            else if (requestedWeapon == ActiveWeapon.Raser)
            {
                if (raserLevel < 3)
                    raserLevel++;
            }
        }
    }


    // ── 자동 업그레이드 (내부 타이머에 의해 3단계 상태에서 호출되어 4단계로 전환) ──
    private void AutoUpgradeGunToMax()
    {
        if (gunLevel == 3)
        {
            gunLevel = 4;
            Debug.Log("Gun auto upgraded to max level " + gunLevel);
        }
    }
    private void AutoUpgradeRaserToMax()
    {
        if (raserLevel == 3)
        {
            raserLevel = 4;
            Debug.Log("Raser auto upgraded to max level " + raserLevel);
        }
    }

    public void RecoverHealth()
    {
        if (health < 3)
        {
            health++;
            Debug.Log("체력 회복, 현재 체력: " + health);
        }
        else
        {
            Debug.Log("체력이 이미 최대입니다.");
        }
    }

    public void ActivateShield()
    {
        if (isShieldActive)
        {
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

    public void TakeDamage(int damage)
    {
        if (isInvincible)
            return;
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
        Debug.Log("플레이어 체력: " + health);
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
