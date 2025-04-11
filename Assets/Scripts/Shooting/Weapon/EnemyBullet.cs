using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    // "Gun", "Sniper", "ScreamWave" 설정해야함
    public string bulletType;
    public float speed = 1f; // 총알 속도
    public int damage = 1; // 총알 데미지
    internal bool isEnemy4Bullet;
    private float lifetime = 0f;
    public Vector3 direction;

    void OnEnable()
    {
        // 총알이 활성화될 때마다 lifetime 초기화
        lifetime = 0f;
    }

    void Update()
    {
        if (bulletType == "Gun")
        {
            // Gun 총알: 1의 속도로 아래로 이동
            transform.position += Vector3.down * speed * Time.deltaTime;
            // y가 -10 이하이면 반환
            if (transform.position.y <= -10f)
            {
                WeaponPool.Instance.ReturnWeapon(bulletType, gameObject);
            }
        }
        else if (bulletType == "Sniper")
        {
            // Sniper 총알: direction 방향(플레이어를 향한 방향)으로 1.8의 속도로 이동
            transform.position += direction * speed * Time.deltaTime;
            // x가 6 또는 -6, y가 10 또는 -10 범위를 벗어나면 반환
            if (transform.position.x >= 6f || transform.position.x <= -6f ||
                transform.position.y >= 10f || transform.position.y <= -10f)
            {
                WeaponPool.Instance.ReturnWeapon(bulletType, gameObject);
            }
        }
        else if (bulletType == "ScreamWave")
        {
            // ScreamWave 총알: 0.8의 속도로 이동 (플레이어를 향한 방향)
            transform.position += direction * speed * Time.deltaTime;
            lifetime += Time.deltaTime;
            // 2초 후에 반환
            if (lifetime >= 2f)
            {
                WeaponPool.Instance.ReturnWeapon(bulletType, gameObject);
            }
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
                if (bulletType == "Gun" || bulletType == "Sniper")
                {
                    player.TakeDamage(damage);
                }
                WeaponPool.Instance.ReturnWeapon(bulletType, gameObject);
            }
        }
    }
}
