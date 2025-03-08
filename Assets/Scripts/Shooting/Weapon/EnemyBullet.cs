using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 4f; // 총알 속도
    public int damage = 1; // 총알 데미지
    internal bool isEnemy4Bullet;

    void Update()
    {
        transform.position += Vector3.down * speed * Time.deltaTime; // 아래로 이동

        // 화면 밖으로 나가면 제거
        if (transform.position.y <= -5.5f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 태그가 맞는지 확인
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                if (isEnemy4Bullet)
                {
                    // Enemy_4 총알: 플레이어에게 데미지를 주지 않고, 이동 속도를 30% 감소시키는 효과 적용.
                    player.ApplySpeedReduction(0.3f); // 이거 Player 스크립트에서 사용
                }
                else
                {
                    // 일반 적 총알: 데미지 적용
                    player.TakeDamage(damage);
                }
                Destroy(gameObject); // 충돌 후 총알 제거
            }
        }
    }
}
