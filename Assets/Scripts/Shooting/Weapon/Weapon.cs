using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject projectilePrefab; // 발사체 프리팹
    public Transform shootPoint; // 발사 위치
    public int bulletCount = 1; // 총알 개수
    public float shootAngle = 10f; // 미사일 포물선 각도 (미사일일 경우)

    public void Shoot()
    {
        if (bulletCount > 1)
        {
            float angleStep = 10f; // 총알 퍼지는 정도
            float startAngle = -(bulletCount - 1) * angleStep / 2;

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = startAngle + (i * angleStep);
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                Instantiate(projectilePrefab, shootPoint.position, rotation);
            }
        }
        else
        {
            Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        }
    }
}
