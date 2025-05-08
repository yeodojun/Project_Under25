using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public enum ActiveWeapon { Gun, Raser, Missile }
    public ActiveWeapon currentWeapon = ActiveWeapon.Gun;
    private Coroutine raserCycleCoroutine = null;

    [Header("Shoot Transform & Timings")]
    [SerializeField] private Transform shootTransform;
    [SerializeField] private float gunShootInterval = 0.2f; // Gun 발사 간격
    private float lastGunShootTime = 0f;

    private const float autoUpgradeDelay = 22f;
    private float autoUpgradeTimerGun = 0f;
    private float autoUpgradeTimerRaser = 0f;
    // 피격시 사용
    public GameObject upgradeItemPrefab;

    // 각 무기는 수동으로 최대 3단계까지 업그레이드되며, 3단계 상태에서 타이머 경과 시 자동으로 4단계가 됩니다.
    private int gunLevel = 1;
    private int raserLevel = 1;
    private int missileLevel = 0;           // 0=없음, 1=1단계, 2=2단계
    // 미사일 관련 추가 필드
    [SerializeField] private GameObject missileLauncherPrefab; // 미사일 런처 프리팹 (인스펙터 할당)
    private GameObject missileLauncher1 = null; // Level 1 launcher
    private GameObject missileLauncher2 = null; // Level 2 launcher (반대편)
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
        new GunFireInfo(new Vector3(0.3f, 5f, 0f), "Beam"),
        new GunFireInfo(new Vector3(-0.3f, 5f, 0f), "Beam")
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

    // ㅡ Missile(미사일) 발사 패턴 ㅡ


    [Header("Player Movement & Other Settings")]
    public bool canShoot = true;
    public int health = 3;
    public float invincibilityDuration = 0.5f;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;
    private Vector3 targetPosition;
    public float moveSpeed = 5f;
    private float originalMoveSpeed;
    public GameObject gameOverPanel;


    [SerializeField] private float collisionCheckRadius = 0.2f;
    [SerializeField] private LayerMask wallLayer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetPosition = transform.position;
        originalMoveSpeed = moveSpeed;
    }

    void Update()
    {
        HandleMovement();
        ScoreHealth();

        if (currentWeapon == ActiveWeapon.Gun && canShoot)
        {
            if (gunLevel == 1)
            {
                if (Time.time - lastGunShootTime > gunShootInterval)
                {
                    FireGun();
                    lastGunShootTime = Time.time;
                }
            }
            else if (gunLevel == 2)
            {
                gunShootInterval = 0.18f;
                if (Time.time - lastGunShootTime > gunShootInterval)
                {
                    FireGun();
                    lastGunShootTime = Time.time;
                }
            }
            else if (gunLevel == 3)
            {
                gunShootInterval = 0.15f;
                if (Time.time - lastGunShootTime > gunShootInterval)
                {
                    FireGun();
                    lastGunShootTime = Time.time;
                }
                autoUpgradeTimerGun += Time.deltaTime;
                if (autoUpgradeTimerGun >= autoUpgradeDelay)
                {
                    AutoUpgradeGunToMax();
                    autoUpgradeTimerGun = 0f;
                }
            }
            else
            {
                gunShootInterval = 0.1f;
                if (Time.time - lastGunShootTime > gunShootInterval)
                {
                    FireGun();
                    lastGunShootTime = Time.time;
                }
            }
        }
        else if (currentWeapon == ActiveWeapon.Raser && canShoot)
        {
            if (raserLevel < 4)
            {
                // 예를 들어, 코루틴이 한 번도 실행되지 않았다면 시작
                if (raserCycleCoroutine == null)
                {
                    raserCycleCoroutine = StartCoroutine(RaserCycle());
                }
            }
            else
            {
                // raserLevel 4인 경우는 기존처럼 지속발사 처리
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
        }
        else
        {
            // 무기가 Raser가 아니거나 발사가 불가능할 때, 레이저 코루틴이 있다면 정지
            if (raserCycleCoroutine != null)
            {
                StopCoroutine(raserCycleCoroutine);
                raserCycleCoroutine = null;
            }
        }
        if (missileLevel > 0 && canShoot)
        {
            if (missileLauncher1 == null)
            {
                missileLauncher1 = Pool.Instance.SpawnWeapon("MissileLauncher", shootTransform.position, Quaternion.identity);
                MissileLauncher launcher = missileLauncher1.GetComponent<MissileLauncher>();
                if (launcher != null)
                {
                    launcher.Initialize(this, 1); // 오른쪽 launcher, side = 1
                }
            }
            if (missileLevel >= 2 && missileLauncher2 == null)
            {
                missileLauncher2 = Pool.Instance.SpawnWeapon("MissileLauncher", shootTransform.position, Quaternion.identity);
                MissileLauncher launcher2 = missileLauncher2.GetComponent<MissileLauncher>();
                if (launcher2 != null)
                {
                    launcher2.Initialize(this, -1); // 왼쪽 launcher, side = -1
                }
            }
        }
    }

    void HandleMovement()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = transform.position.z;
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("Wall"))
            {
                // 터치한 위치가 Wall이라면 무시
                return;
            }
            targetPosition = mousePos;
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
            Pool.Instance.SpawnWeapon(info.weaponType, spawnPos, rotation);
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
                persistentUBeam = Pool.Instance.SpawnWeapon("UBeam", shootTransform.position, shootTransform.rotation);
                Weapon w = persistentUBeam.GetComponent<Weapon>();
                if (w != null)
                    w.followPlayer = true;
                    w.beamLifetime = 999f;
            }
            if (persistentBBeam == null)
            {
                persistentBBeam = Pool.Instance.SpawnWeapon("BBeam", shootTransform.position, shootTransform.rotation);
                Weapon w = persistentBBeam.GetComponent<Weapon>();
                if (w != null)
                    w.followPlayer = true;
                    w.beamLifetime = 999f;
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
                GameObject laserObj = Pool.Instance.SpawnWeapon(info.weaponType, spawnPos, baseRotation);
                // 모든 레이저(Beam, BBeam 등)를 플레이어 따라다니게 설정
                Weapon w = laserObj.GetComponent<Weapon>();
                if (w != null)
                {
                    w.followPlayer = true;
                    if (raserLevel == 1)
                        w.beamLifetime = 1f;
                    else if (raserLevel == 2)
                        w.beamLifetime = 3f;
                    else if (raserLevel == 3)
                        w.beamLifetime = 3.5f;
                }
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

    IEnumerator RaserCycle()
    { // 레이저 레벨에 따라 대기 시간과 발사 시간을 설정합니다. 
        float waitTime = 0f, shootDuration = 0f;
        if (raserLevel == 1) { waitTime = 3f; shootDuration = 1f; }
        else if (raserLevel == 2) { waitTime = 4.5f; shootDuration = 3f; }
        else if (raserLevel == 3) { waitTime = 8f; shootDuration = 5f; }
        else{yield break;}
        while (currentWeapon == ActiveWeapon.Raser && raserLevel < 4)
        {
            yield return new WaitForSeconds(waitTime);

            // 발사 시작 : FireRaser()를 호출할 때마다 스폰되는 레이저 오브젝트의 beamLifetime을 shootDuration으로 설정
            FireRaser();  // 아래 FireRaser() 수정 참고

            // 발사 시간 동안(예, beamLifetime이 shootDuration인 동안) 발사 상태 유지
            yield return new WaitForSeconds(shootDuration);
        }
    }

    // ── 수동 업그레이드 (UpgradeItem 호출용): 현재 무기 타입이 요청과 같으면 해당 무기의 레벨을 1~3까지만 올림
    // 서로 다른 무기 타입이면 무기를 전환하고 현재 저장된 레벨을 그대로 사용
    public void UpgradeWeapon(ActiveWeapon requestedWeapon)
    {
        if (requestedWeapon == ActiveWeapon.Missile)
        {
            // Missile 업그레이드는 다른 무기와 같이 사용할 수 있도록 하며,
            // 현재 무기를 전환하지 않고 missileLevel만 증가
            if (missileLevel < 2)
            {
                missileLevel++;
                ScoreManager.Instance.AddScore(5);
                Debug.Log("Missile launcher upgraded to level " + missileLevel);
            }
            else
            {
                ScoreManager.Instance.AddScore(15);
            }
            return;
        }

        if (currentWeapon == requestedWeapon) // 현재 무기가 업그레이드 권 무기와 같은 경우
        {
            if (requestedWeapon == ActiveWeapon.Gun)
            {
                if (gunLevel < 3)
                {
                    gunLevel++;
                    ScoreManager.Instance.AddScore(5);
                    Debug.Log("Gun upgraded via item to level " + gunLevel);
                }
                else
                {
                    ScoreManager.Instance.AddScore(15);
                }
            }
            else if (requestedWeapon == ActiveWeapon.Raser) // ActiveWeapon.Raser
            {
                if (raserLevel < 3)
                {
                    raserLevel++;
                    ScoreManager.Instance.AddScore(5);
                    Debug.Log("Raser upgraded via item to level " + raserLevel);
                }
                else
                {
                    ScoreManager.Instance.AddScore(15);
                }
            }
        }
        else // 현재 무기가 업그레이드 권 무기와 다를 경우, 무기 전환
        {
            // 전환 시, 현재 Raser persistent 오브젝트가 있다면 반환하고 null로 초기화, 현재 무기가 레이저였던 경우 레이저 제거 지속되는 애들이라 따로 지워줘야 함
            if (currentWeapon == ActiveWeapon.Raser)
            {
                if (persistentUBeam != null)
                {
                    Pool.Instance.ReturnWeapon("UBeam", persistentUBeam);
                    persistentUBeam = null;
                }
                if (persistentBBeam != null)
                {
                    Pool.Instance.ReturnWeapon("BBeam", persistentBBeam);
                    persistentBBeam = null;
                }
            }
            currentWeapon = requestedWeapon; // 대입
            Debug.Log("Switched weapon to " + currentWeapon.ToString() + " at level " +
                      (currentWeapon == ActiveWeapon.Gun ? gunLevel : raserLevel));
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
    public void ScoreHealth()
    {
        ScoreManager.Instance.Health();
    }

    public void RecoverHealth()
    {
        health++;
        Debug.Log("체력 회복, 현재 체력: " + health);

    }

    public void TakeDamage(int damage)
    {
        if (missileLevel > 0)
        {
            // 우선 왼쪽 런처부터 제거, 그 다음 오른쪽 런처
            if (missileLauncher2 != null)
            {
                Pool.Instance.ReturnWeapon("MissileLauncher", missileLauncher2);
                missileLauncher2 = null;
            }
            else if (missileLauncher1 != null)
            {
                Pool.Instance.ReturnWeapon("MissileLauncher", missileLauncher1);
                missileLauncher1 = null;
            }
            missileLevel--;
            Debug.Log("" + missileLevel);
            return;
        }
        if (isInvincible)
            return;
        health -= damage;
        Debug.Log("플레이어 체력: " + health);
        ScoreManager.Instance.fixHealth(health);
        if (health <= 0)
        {
            Debug.Log("플레이어 사망!");
            GameOver();
        }
        else
        {
            // 피격 시 무기 초기화 및 리스폰 처리 (피격 후 0.2초 후 실행) 무기 드랍은 추가 예정 리소스 매우 필요
            StartCoroutine(ResetWeaponsAndRespawn());
        }
    }


    private IEnumerator ResetWeaponsAndRespawn()
    {
        canShoot = false;
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }
        Vector3 dropPosition = transform.position;
        int gunItemCount = gunLevel - 1;
        int raserItemCount = raserLevel - 1;
        for (int i = 1; i <= gunItemCount; i++)
        {
            Pool.Instance.SpawnWeapon("UpgradeItem", dropPosition, Quaternion.identity);
        }
        // persistent 무기(레이저)와 미사일 런쳐가 있다면 반환
        if (persistentUBeam != null)
        {
            for (int i = 0; i < raserItemCount; i++)
            {
                Pool.Instance.SpawnWeapon("UpgradeItem", dropPosition, Quaternion.identity);
            }
            Pool.Instance.ReturnWeapon("UBeam", persistentUBeam);
            persistentUBeam = null;
        }
        if (persistentBBeam != null)
        {
            for (int i = 0; i < raserItemCount; i++)
            {
                Pool.Instance.SpawnWeapon("UpgradeItem", dropPosition, Quaternion.identity);
            }
            Pool.Instance.SpawnWeapon("UpgradeItem", dropPosition, Quaternion.identity);
            Pool.Instance.ReturnWeapon("BBeam", persistentBBeam);
            persistentBBeam = null;
        }
        if (missileLauncher1 != null)
        {
            Pool.Instance.ReturnWeapon("MissileLauncher", missileLauncher1);
            missileLauncher1 = null;
        }
        if (missileLauncher2 != null)
        {
            Pool.Instance.ReturnWeapon("MissileLauncher", missileLauncher2);
            missileLauncher2 = null;
        }

        // 플레이어 사라짐
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(1f); // setActive(false)로 했다가 그 후 코드가 작동을 안해서 그냥 스프라이트를 꺼버림

        // 플레이어 리스폰: 지정 위치 (0, -2.5)로 이동
        transform.position = new Vector3(0f, -2.5f, 0);

        // 무기 상태 초기화
        currentWeapon = ActiveWeapon.Gun;
        gunLevel = 1;
        raserLevel = 1;
        missileLevel = 0;

        // 자동 업그레이드 타이머 초기화
        autoUpgradeTimerGun = 0f;
        autoUpgradeTimerRaser = 0f;

        // 플레이어 재활성화
        spriteRenderer.enabled = true;

        yield return StartCoroutine(Invincibility());
        if (playerCollider != null)
        {
            playerCollider.enabled = true;
        }
        canShoot = true;
    }

    public void ApplySpeedReduction(float reductionPercent, float duration = 1f)
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

    // 얘가 중복떠서 데미지가 2씩 들어가던거였음 Enemy.cs에서 이미 사용 중
    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && enemy.enemyType != 4)
                TakeDamage(1);
        }
    }*/
}
