using Unity.Mathematics;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    // Explosion, ShotGun, Circle, Rampage, Sniper, Canon
    public string weaponType;
    public Vector3 direction;

    private bool hasExploded = false;

    void OnEnable()
    {
        hasExploded = false;
    }

    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        float baseAngle = UnityEngine.Random.Range(0f, 360f); // 기준 각도 설정

        for (int i = 0; i < 4; i++)
        {
            float angle = baseAngle + i * 90f; // 십자 방향으로 90도씩 회전
            Vector3 rotatedDir = Quaternion.Euler(0, 0, angle) * Vector3.up;

            GameObject bullet = Pool.Instance.SpawnWeapon("Dust", transform.position, Quaternion.identity);

            if (bullet != null)
            {
                float randomZRotation = UnityEngine.Random.Range(0f, 360f);
                bullet.transform.rotation = Quaternion.Euler(0, 0, randomZRotation);

                EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
                bulletScript.bulletType = "Dust";
                bulletScript.direction = rotatedDir.normalized;
                bullet.SetActive(true);
            }
        }

        weaponType = "";
    }

}
