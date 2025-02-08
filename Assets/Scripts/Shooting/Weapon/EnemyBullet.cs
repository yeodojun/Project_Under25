using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 4f; // 총알 속도
    public int damage = 1; // 총알 데미지

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
                player.TakeDamage(damage); // 플레이어에게 데미지 전달
                Destroy(gameObject); // 총알 제거
            }
        }
    }
}
