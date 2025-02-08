using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f; // 총알 속도
    public int damage = 1; // 총알 데미지
    private bool hasHit = false; // 이미 충돌했는지 여부

    void Start()
    {
        gameObject.tag = "Bullet"; // 태그 추가
    }

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;

        // 화면 밖으로 나가면 제거
        if (transform.position.y >= 5.5f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return; // 이미 충돌했으면 무시

        if (other.CompareTag("Enemy")) // 적과 충돌
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // 적에게 데미지 전달
                hasHit = true; // 충돌 처리 완료
                Destroy(gameObject); // 총알 제거
            }
        }
    }
}
