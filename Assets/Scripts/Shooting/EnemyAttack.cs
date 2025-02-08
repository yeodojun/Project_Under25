using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject enemyBulletPrefab; // 적 총알 프리팹
    public Transform firePoint; // 총알 발사 위치
    public float minAttackDelay = 3f; // 최소 공격 딜레이
    public float maxAttackDelay = 5f; // 최대 공격 딜레이

    void Start()
    {
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        while (true)
        {
            float attackDelay = Random.Range(minAttackDelay, maxAttackDelay); // 3~5초 사이 랜덤 딜레이
            yield return new WaitForSeconds(attackDelay);

            if (firePoint != null)
            {
                Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.identity); // 총알 발사
            }
        }
    }
}
