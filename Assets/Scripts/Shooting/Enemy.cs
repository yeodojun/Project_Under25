using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // enemyType: 0 = 기본 적, 1 = Enemy_1, 2 = Enemy_2
    public int enemyType = 0;
    
    // 기존 Enemy_1 관련 플래그 (enemyType 1일 때 사용)
    public bool isEnemy1 = false;
    public bool isFlipped = false; // Enemy_1이면 true로 설정

    public int health = 5; // 기본 적 체력
    public float dropChance = 0.1f; // 업그레이드 아이템 드롭 확률
    public GameObject upgradeItemPrefab; // 업그레이드 아이템 프리팹

    public int scoreValue = 10; // 적 처치 시 획득 점수
    private bool isDead = false; // 적이 이미 죽었는지 확인

    // Enemy_2 전용 필드 (enemyType == 2)
    public GameObject enemyBulletPrefab; // 적 총알 프리팹 (Inspector에 할당)
    public Transform firePoint;          // 총알 발사 위치 (Inspector에 할당)

    void Awake()
    {
        // enemyType에 따라 초기화
        if (enemyType == 1)
        {
            isEnemy1 = true;
            health = 20;
            EnemyAttack attack = GetComponent<EnemyAttack>();
            if (attack != null)
            {
                attack.enabled = false;
            }
        }
        else if (enemyType == 2)
        {
            health = 50;
            EnemyAttack attack = GetComponent<EnemyAttack>();
            if (attack != null)
            {
                attack.enabled = false;
            }
            StartCoroutine(Enemy2Attack());
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // 이미 죽은 경우 처리 방지

        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return; // 이미 죽었으면 처리하지 않음
        isDead = true; // 죽음 상태 플래그 설정

        // 점수 추가
        ScoreManager.Instance.AddScore(scoreValue);

        // 10% 확률로 업그레이드 아이템 드롭
        if (Random.value < dropChance && upgradeItemPrefab != null)
        {
            Instantiate(upgradeItemPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject); // 적 제거
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Missile")) // 미사일과 충돌
        {
            TakeDamage(3); // 미사일 데미지 적용
            Destroy(other.gameObject); // 미사일 제거
        }
    }

    // Enemy_2 전용 공격 코루틴: 5초마다 총알 발사, 총알의 데미지는 2로 설정
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
                {
                    eb.damage = 2;
                }
            }
        }
    }
}
