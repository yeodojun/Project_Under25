using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 새 적(Enemy_1) 여부를 결정하는 플래그
    // 이 값이 true이면 체력을 20으로 설정하고, 총 발사 기능(EnemyAttack)을 비활성화합니다.
    public bool isEnemy1 = false;

    public int health = 5; // 기본 적 체력
    public float dropChance = 0.1f; // 업그레이드 아이템 드롭 확률
    public GameObject upgradeItemPrefab; // 업그레이드 아이템 프리팹

    public int scoreValue = 10; // 적 처치 시 획득 점수
    private bool isDead = false; // 적이 이미 죽었는지 확인

    void Awake()
    {
        // Enemy_1이면 체력을 20으로 설정하고, EnemyAttack 컴포넌트(총 발사 관련)를 비활성화
        if (isEnemy1)
        {
            health = 20;
            EnemyAttack attack = GetComponent<EnemyAttack>();
            if (attack != null)
            {
                attack.enabled = false;
            }
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
}
