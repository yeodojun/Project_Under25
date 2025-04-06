using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f; // 총알 속도
    public int damage = 1;   // 총알 데미지
    private bool hasHit = false; // 이미 충돌했는지 여부

    void Start()
    {
        // 태그는 한 번만 설정해도 되지만, 재사용 시에도 보장되도록 OnEnable()에서 설정할 수 있습니다.
        gameObject.tag = "Bullet";
    }

    void OnEnable()
    {
        // 총알 재사용 시 상태 초기화
        hasHit = false;
    }

    void Update()
    {
        // 총알 이동
        transform.position += Vector3.up * speed * Time.deltaTime;

        // 화면 상단(예: y >= 10)으로 나가면 풀로 반환
        if (transform.position.y >= 10f)
        {
            WeaponPool.Instance.ReturnWeapon("Bullet", gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return; // 이미 충돌 처리되었으면 무시

        // 적과 충돌 시 처리
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                hasHit = true;
                WeaponPool.Instance.ReturnWeapon("Bullet", gameObject);
            }
        }

        // 보스와 충돌 시 처리
        Boss boss = other.GetComponent<Boss>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            hasHit = true;
            WeaponPool.Instance.ReturnWeapon("Bullet", gameObject);
        }
    }
}
