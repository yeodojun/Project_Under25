using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // enemyType: 0 = 기본 적, 1 = Enemy_1, 2 = Enemy_2, 3 = Enemy_3, 4 = Enemy_4, 101 = Enemy_101
    public int enemyType = 0;
    public bool isFlipped = false; // Enemy_1이면 true로 설정
    private bool hasDamaged = false; 

    public int health = 10; // 기본 적 체력
    public float dropChance = 0.05f; // 업그레이드 아이템 드롭 확률
    public GameObject upgradeItemPrefab; // 업그레이드 아이템 프리팹
    public int scoreValue = 5; // 적 처치 시 획득 점수
    public Transform firePoint;          // 총알 발사 위치

    void Awake()
    {
        // 타입별 체력 설정 및 풀링 발사 코루틴 시작
        switch (enemyType)
        {
            case 1:
                health = 30; break;
            case 2:
                health = 100;
                StartCoroutine(EnemyAttackCycle("Fire", 1f, 1f, damage:1, 2, 1f));
                return;
            case 3:
                health = 20;
                return;
            case 4:
                health = 50;
                return;
            case 5:
                health = 250;
                StartCoroutine(EnemyAttackCycle("Gun", 1f, 0.5f, damage:1, 2, 1f));
                return;
            case 6:
                health = 30;
                StartCoroutine(EnemyAttackCycle("Boom", 1f, 10f, damage:1, 5, 1f));
                return;
            case 7:
                health = 70;
                StartCoroutine(EnemyAttackCycle("Gun", 1f, 0.3f, damage:1, 2, 1f));
                return;
            case 8:
                health = 350;
                return;
            case 101:
                health = 500;
                StartCoroutine(EnemyAttackCycle("Gun", 4f, 0f, damage:1, shotsPerCycle:2, offset:0.2f));
                return;
            case 102:
                health = 20;
                StartCoroutine(EnemyAttackCycle("Fire", 3f, 1f, damage:1));
                return;
        }
    }

    public void TakeDamage(int damage)
    {
        if (health <= 0) return;
        health -= damage;
        if (health <= 0) Die();
    }

    void Die()
    {
        ScoreManager.Instance.AddScore(scoreValue);
        if (Random.value < dropChance)
        {
            // 업그레이드 아이템도 풀링으로
            WeaponPool.Instance.SpawnWeapon("UpgradeItem", transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasDamaged)
        {
            if (enemyType != 11)
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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hasDamaged = false;
        }
    }

    private IEnumerator EnemyAttackCycle(
        string bulletType, // 총알 타입
        float interval,    // 한 사이클 주기(초)
        float initialDelay,// 첫 발사 전 대기 시간(초)
        int damage,        // 총알 데미지
        int shotsPerCycle = 1,// 사이클당 발사 횟수
        float offset = 0f, // 총알 사이 좌우 간격 shotsPerCycle >= 2 일 때 사용할 거
        bool isEnemy11 = false)// 
    {
        // 초기 대기
        if (initialDelay > 0f)
            yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            for (int i = 0; i < shotsPerCycle; i++)
            {
                // 좌우로 조금씩 벌리기
                Vector3 spawnPos = firePoint.position + new Vector3(offset * (i - (shotsPerCycle-1)/2f), 0f, 0f);
                SpawnPooledBullet(bulletType, spawnPos, damage, isEnemy11);
            }
            yield return new WaitForSeconds(interval);
        }
    }

    private void SpawnPooledBullet(string type, Vector3 pos, int damage, bool isEnemy11)
    {
        GameObject bullet = WeaponPool.Instance.SpawnWeapon(type, pos, Quaternion.identity);
        var eb = bullet.GetComponent<EnemyBullet>();
        eb.bulletType     = type;
        eb.damage         = damage;
        eb.isEnemy11Bullet = isEnemy11;
        // 방향이 필요한 타입이면 prefab에 설정된 direction을 사용하거나,
        // eb.direction에 별도 할당 코드 추가 가능
    }
}
