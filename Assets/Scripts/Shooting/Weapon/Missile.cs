using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed = 3f;   // 미사일의 초기 속도
    public int damage = 3;     // 미사일 데미지
    private bool hasHit = false;

    private Rigidbody2D rb;

    void Start()
    {
        gameObject.tag = "Missile";

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        // 중력 효과 없이 날아가도록 설정
        rb.gravityScale = 0f;

        // 미사일의 스폰 위치에 따라 발사 각도 결정
        float launchAngleDeg = 90f; // 기본값: 세로(90도)
        if (transform.position.x > 0)
        {
            launchAngleDeg = 60f; // 오른쪽 스폰: 60도
        }
        else if (transform.position.x < 0)
        {
            launchAngleDeg = 30f; // 왼쪽 스폰: 30도
        }
        float launchAngleRad = launchAngleDeg * Mathf.Deg2Rad;
        Vector2 initialVelocity = new Vector2(speed * Mathf.Cos(launchAngleRad), speed * Mathf.Sin(launchAngleRad));
        rb.linearVelocity = initialVelocity;
    }

    void Update()
    {
        // 미사일이 직선으로 날아가므로, 화면 경계를 벗어나면 제거합니다.
        if (transform.position.y >= 5.5f || transform.position.y <= -5.5f ||
            transform.position.x <= -3.2f || transform.position.x >= 3.2f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                hasHit = true;
                Destroy(gameObject);
            }
        }
    }
}
