using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    public float speed = 5f;    // 기본 이동 속도
    public int damage = 1;      // 기본 데미지
    private bool hasHit = false; // 충돌 처리 여부

    // weaponType에 따라 "Bullet", "Beam", "BBeam", "UBeam" 등으로 구분합니다.
    public string weaponType = "Bullet";

    // 레이저 무기(Beam, BBeam)의 지속 시간 관리용 타이머
    private float lifetimeTimer = 0f;
    public float beamLifetime = 0.5f; // 예: 0.5초
                                      // 스폰 시 플레이어와의 상대 오프셋을 저장 (레이저용)
    private Vector3 initialRelativeOffset;
    private bool offsetInitialized = false;


    // 레이저 무기 중, UBeam와 level4에서 BBeam이 플레이어를 따라다니게 하고 싶으므로
    // followPlayer가 true이면 매 프레임 플레이어 위치를 업데이트합니다.
    public bool followPlayer = false;

    void Start()
    {
        gameObject.tag = weaponType;
    }

    void OnEnable()
    {
        hasHit = false;
        lifetimeTimer = 0f;
        if (weaponType == "UBeam" || weaponType == "Bullet1")
            damage = 3;
        else
            damage = 1;

        // 만약 레이저(Beam, BBeam)라면 스폰 시 플레이어와의 상대 오프셋을 저장
        if (weaponType == "Beam" || weaponType == "BBeam")
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                initialRelativeOffset = transform.position - playerObj.transform.position;
                offsetInitialized = true;
            }
        }
    }


    void Update()
    {
        if (weaponType == "Bullet")
        {
            // Bullet은 자신이 가진 transform.up 방향으로 이동
            transform.position += transform.up * speed * Time.deltaTime;
            if (transform.position.y >= 10f)
            {
                WeaponPool.Instance.ReturnWeapon("Bullet", gameObject);
            }
        }
        else if (weaponType == "Bullet1")
        {
            transform.position += transform.up * speed * 2 * Time.deltaTime;
            if (transform.position.y >= 10f)
            {
                WeaponPool.Instance.ReturnWeapon("Bullet1", gameObject);
            }
        }
        else if (weaponType == "Beam" || weaponType == "BBeam")
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null && offsetInitialized)
            {
                transform.position = playerObj.transform.position + initialRelativeOffset;
            }
            lifetimeTimer += Time.deltaTime;
            if (lifetimeTimer >= beamLifetime)
            {
                WeaponPool.Instance.ReturnWeapon(weaponType, gameObject);
            }
        }
        // UBeam와 followPlayer가 true인 BBeam는 플레이어를 따라 다니게 함
        else if (weaponType == "UBeam")
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                // 플레이어 위치에서 위쪽 1유닛 오프셋 (필요에 따라 수정)
                transform.position = playerObj.transform.position + new Vector3(0f, 5f, 0f);
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Bullet 계열 무기는 충돌 시 한번 데미지를 주고 반환
        if (weaponType == "Bullet" || weaponType == "Bullet1")
        {
            if (hasHit) return;
            if (other.CompareTag("Enemy"))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    hasHit = true;
                    WeaponPool.Instance.ReturnWeapon(weaponType, gameObject);
                }
            }
            else
            {
                Boss boss = other.GetComponent<Boss>();
                if (boss != null)
                {
                    boss.TakeDamage(damage);
                    hasHit = true;
                    WeaponPool.Instance.ReturnWeapon(weaponType, gameObject);
                }
            }
        }
        // 레이저 계열 무기는 충돌해도 반환되지 않고 초당 데미지 제한은 별도 처리 (아래 OnTriggerStay2D로 구현)
        else if (weaponType == "Beam" || weaponType == "BBeam" || weaponType == "UBeam")
        {
            if (other.CompareTag("Enemy"))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // OnTriggerStay2D에서 데미지 주도록 처리하므로 여기서는 별도 반환하지 않음.
                    enemy.TakeDamage(damage);
                }
            }
            else
            {
                Boss boss = other.GetComponent<Boss>();
                if (boss != null)
                {
                    boss.TakeDamage(damage);
                }
            }
        }
    }

    // OnTriggerStay2D를 활용하여, 레이저 무기는 한 번 데미지를 준 후 0.2초간 재데미지를 막습니다.
    private static Dictionary<int, float> beamDamageCooldown = new Dictionary<int, float>();
    private const float beamCooldownDuration = 0.2f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (weaponType == "Beam" || weaponType == "BBeam" || weaponType == "UBeam")
        {
            if (other.CompareTag("Enemy"))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    int enemyID = enemy.GetInstanceID();
                    float lastTime = 0f;
                    beamDamageCooldown.TryGetValue(enemyID, out lastTime);
                    if (Time.time - lastTime >= beamCooldownDuration)
                    {
                        enemy.TakeDamage(damage);
                        beamDamageCooldown[enemyID] = Time.time;
                    }
                }
            }
            // 보스에 대해서도 동일하게 가능하게 할 수 있지만, 보스는 일반적으로 단발성 데미지를 주므로 생략할 수 있음.
        }
    }
}
