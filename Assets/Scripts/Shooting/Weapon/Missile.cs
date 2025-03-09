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
        // 중력 효과 없이 직선 비행
        rb.gravityScale = 0f;

        // 미사일의 발사 각도 결정 (왼쪽일수록 각도 큼, 오른쪽일수록 각도 작음)
        float launchAngleDeg = 90f; // 기본값: 수직 (x=0 일 때)
        if (transform.position.x > 0)
        {
            // 오른쪽 스폰: 작게 위로 (30도)
            launchAngleDeg = 30f;
        }
        else if (transform.position.x < 0)
        {
            // 왼쪽 스폰: 크게 위로 (60도)
            launchAngleDeg = 60f;
        }

        // 각도를 라디안으로 변환 후 초기 속도 설정
        float launchAngleRad = launchAngleDeg * Mathf.Deg2Rad;
        Vector2 initialVelocity = new Vector2(
            speed * Mathf.Cos(launchAngleRad),
            speed * Mathf.Sin(launchAngleRad)
        );
        rb.linearVelocity = initialVelocity;

        // 미사일 스프라이트가 발사 각도에 맞춰 회전하도록 해주고 싶다면(옵션):
        // 미사일의 Z축 회전 = 발사각 - 90 (수직 기준)
        float zRotation = launchAngleDeg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, zRotation);
    }

    void Update()
    {
        // 화면 밖으로 나가면 제거
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
