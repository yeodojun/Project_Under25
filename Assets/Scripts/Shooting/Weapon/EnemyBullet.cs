using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class EnemyBullet : MonoBehaviour
{
    // "Fire", "Dust", "Gun", "Boom","LaserGun", "Sniper", "Scream" 설정해야함
    public string bulletType;
    public float speed = 1f; // 총알 속도
    public int damage = 1; // 총알 데미지
    internal bool isEnemy11Bullet;
    private float lifetime = 0f;
    public Vector3 direction;
    public Vector3 direction1;

    void OnEnable()
    {
        lifetime = 0f;

        if (bulletType == "Gun" || bulletType == "LaserGun" || bulletType == "Scream" || bulletType == "Fire")
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                direction = (player.transform.position - transform.position).normalized;
            }
        }
    }


    void Update()
    {
        if (bulletType == "Fire")
        {
            // Gun 총알: 0.8의 속도로 아래로 이동
            transform.position += direction * speed * Time.deltaTime;
            // y가 -10 이하이면 반환
            if (transform.position.x >= 6f || transform.position.x <= -6f ||
                transform.position.y >= 10f || transform.position.y <= -10f)
            {
                Pool.Instance.ReturnWeapon(bulletType, gameObject);
            }
        }
        else if (bulletType == "Dust")
        {
            transform.position += direction * speed * Time.deltaTime;
            // y가 -10 이하이면 반환
            if (transform.position.x >= 6f || transform.position.x <= -6f ||
                transform.position.y >= 10f || transform.position.y <= -10f)
            {
                Pool.Instance.ReturnWeapon(bulletType, gameObject);
            }
        }
        else if (bulletType == "Gun")
        {
            // Gun 총알: 1의 속도로 아래로 이동
            transform.position += direction * speed * Time.deltaTime;
            // y가 -10 이하이면 반환
            if (transform.position.x >= 6f || transform.position.x <= -6f ||
                transform.position.y >= 10f || transform.position.y <= -10f)
            {
                Pool.Instance.ReturnWeapon(bulletType, gameObject);
            }
        }
        else if (bulletType == "Boom")
        {
            // Gun 총알: 1의 속도로 아래로 이동
            transform.position += Vector3.down * speed * Time.deltaTime;
            // y가 -10 이하이면 반환
            if (transform.position.y <= -10f)
            {
                Pool.Instance.ReturnWeapon(bulletType, gameObject);
            }
        }
        else if (bulletType == "LaserGun")
        {
            // Gun 총알: 1의 속도로 아래로 이동
            transform.position += direction * speed * Time.deltaTime;
            // y가 -10 이하이면 반환
            if (transform.position.x >= 6f || transform.position.x <= -6f ||
                transform.position.y >= 10f || transform.position.y <= -10f)
            {
                Pool.Instance.ReturnWeapon(bulletType, gameObject);
            }
        }
        else if (bulletType == "Scream")
        {
            // ScreamWave 총알: 0.1의 속도로 이동 (플레이어를 향한 방향)
            transform.position += direction * speed * Time.deltaTime;
            lifetime += Time.deltaTime;
            // 1초 후에 반환
            if (lifetime >= 1f)
            {
                Pool.Instance.ReturnWeapon(bulletType, gameObject);
            }
        }
        else if (bulletType == "Laser")
        {
            lifetime += Time.deltaTime;
            // 2초 후에 반환
            if (lifetime >= 2f)
            {
                Pool.Instance.ReturnWeapon(bulletType, gameObject);
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
                if (bulletType != "Scream")
                {
                    player.TakeDamage(damage);
                }
                Pool.Instance.ReturnWeapon(bulletType, gameObject);
            }
        }
    }
}
