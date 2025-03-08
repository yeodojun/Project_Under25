using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // enemyType: 0 = 기본 적, 1 = Enemy_1, 2 = Enemy_2, 3 = Enemy_3, 4 = Enemy_4
    public int enemyType = 0;
    
    // 기존 Enemy_1 관련 플래그 (enemyType 1일 때 사용)
    public bool isEnemy1 = false;
    public bool isFlipped = false; // Enemy_1이면 true로 설정

    public int health = 5; // 기본 적 체력
    public float dropChance = 0.05f; // 업그레이드 아이템 드롭 확률
    public GameObject upgradeItemPrefab; // 업그레이드 아이템 프리팹
    public int scoreValue = 10; // 적 처치 시 획득 점수

    private bool isDead = false; // 적이 이미 죽었는지 확인

    // 공용 필드: 적 총알 발사 관련 (Enemy_2,3,4용)
    public GameObject enemyBulletPrefab; // 총알 프리팹
    public Transform firePoint;          // 총알 발사 위치

    void Awake()
    {
        // enemyType에 따라 초기화
        if (enemyType == 1)
        {
            isEnemy1 = true;
            health = 20;
            // 기존의 EnemyAttack 컴포넌트 비활성화
            EnemyAttack attack = GetComponent<EnemyAttack>();
            if (attack != null)
                attack.enabled = false;
        }
        else if (enemyType == 2)
        {
            health = 50;
            EnemyAttack attack = GetComponent<EnemyAttack>();
            if (attack != null)
                attack.enabled = false;
            StartCoroutine(Enemy2Attack());
        }
        else if (enemyType == 3)
        {
            // Enemy_3: 체력 30, 공격력 1
            health = 30;
            EnemyAttack attack = GetComponent<EnemyAttack>();
            if (attack != null)
                attack.enabled = false;
            StartCoroutine(Enemy3Attack());
        }
        else if (enemyType == 4)
        {
            // Enemy_4: 플레이어 공격에 면역
            // 기본 체력 (기본값 5 또는 Inspector에서 지정한 값) 그대로 사용
            // 단, 플레이어의 공격에 의한 데미지를 무시
            StartCoroutine(Enemy4Attack());
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // 이미 죽은 경우 무시
        // Enemy_4는 데미지를 받지 않음
        if (enemyType == 4)
            return;
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        ScoreManager.Instance.AddScore(scoreValue);
        if (Random.value < dropChance && upgradeItemPrefab != null)
        {
            Instantiate(upgradeItemPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 미사일과 충돌 시 데미지 처리 (Enemy_4는 이미 데미지 무시)
        if (other.CompareTag("Missile"))
        {
            TakeDamage(3);
            Destroy(other.gameObject);
        }
        // 플레이어와의 충돌: 기본적으로 플레이어는 데미지를 받지만
        // Enemy_4와 충돌 시에는 플레이어에게 데미지를 주지 않음.
        if (other.CompareTag("Player"))
        {
            if (enemyType != 4)
            {
                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage(1);
                }
            }
            // Enemy_4: 플레이어와 닿아도 데미지 없음.
        }
    }

    // Enemy_2 공격: 5초마다 firePoint에서 총알 발사, 총알 데미지 2
    private IEnumerator Enemy2Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            if (firePoint != null && enemyBulletPrefab != null)
            {
                GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.identity);
                EnemyBullet eb = bullet.GetComponent<EnemyBullet>();
                if (eb != null)
                    eb.damage = 2;
            }
        }
    }

    // Enemy_3 공격: 5초마다 firePoint에서 총알 발사, 총알 데미지 1
    private IEnumerator Enemy3Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            if (firePoint != null && enemyBulletPrefab != null)
            {
                GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.identity);
                EnemyBullet eb = bullet.GetComponent<EnemyBullet>();
                if (eb != null)
                    eb.damage = 1;
            }
        }
    }

    // Enemy_4 공격: 3초마다 firePoint에서 총알 발사
    // 이 총알은 일반적으로 데미지를 주지 않고, 대신 플레이어의 이동 속도를 30% 감소시키도록 처리되어야 함.
    private IEnumerator Enemy4Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            if (firePoint != null && enemyBulletPrefab != null)
            {
                GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.identity);
                // assume EnemyBullet.cs has a boolean flag "isEnemy4Bullet" that, when true,
                // causes the bullet to not deal damage but reduce player's move speed by 30%.
                EnemyBullet eb = bullet.GetComponent<EnemyBullet>();
                if (eb != null)
                    eb.isEnemy4Bullet = true;
            }
        }
    }
}
