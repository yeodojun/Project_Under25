using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class Pool : MonoBehaviour
{
    public static Pool Instance;

    // 각 무기 유형의 풀 정보를 담는 클래스
    [System.Serializable]
    public class WeaponPoolItem
    {
        [Tooltip("무기 식별자 (Player : Bullet, Bullet1, Beam, BBeam, UBeam, MissileLauncher, Missile, UpgradeItem)")]
        // (Enemy : Fire, Dust, Gun, Boom, LaserGun, Scream, Laser)
        public string weaponType;
        [Tooltip("해당 무기의 프리팹")]
        public GameObject prefab;
        [Tooltip("풀에 미리 생성해 둘 오브젝트 개수")]
        public int poolSize = 30;
        [HideInInspector]
        public Queue<GameObject> pool;
    }
    [System.Serializable]
    public class EnemyPoolItem
    {
        [Tooltip("적 (Enemy_N)")]
        public string enemyTypeName;
        public GameObject prefab;
        public int poolSize = 20;
        [HideInInspector]
        public Queue<GameObject> pool;
    }
    public EnemyPoolItem[] enemyPoolItems;

    // Inspector에서 설정할 수 있도록 배열로 관리
    public WeaponPoolItem[] poolItems;

    void Awake()
    {
        Instance = this;

        // 각 무기 유형별로 풀 초기화
        foreach (WeaponPoolItem item in poolItems)
        {
            item.pool = new Queue<GameObject>();
            for (int i = 0; i < item.poolSize; i++)
            {
                GameObject obj = Instantiate(item.prefab);
                obj.SetActive(false);
                item.pool.Enqueue(obj);
            }
        }
        // 각 적 유형별로 풀 초기화
        foreach (EnemyPoolItem item in enemyPoolItems)
        {
            item.pool = new Queue<GameObject>();
            for (int i = 0; i < item.poolSize; i++)
            {
                GameObject obj = Instantiate(item.prefab);
                obj.SetActive(false);
                item.pool.Enqueue(obj);
            }
        }

    }

    // 특정 무기 유형(weaponType)의 오브젝트를 풀에서 가져와 활성화
    public GameObject SpawnWeapon(string weaponType, Vector3 position, Quaternion rotation)
    {
        foreach (WeaponPoolItem item in poolItems)
        {
            if (item.weaponType == weaponType)
            {
                GameObject weapon;
                if (item.pool.Count > 0)
                {
                    weapon = item.pool.Dequeue();
                    weapon.transform.position = position;
                    weapon.transform.rotation = rotation;
                    weapon.SetActive(true);
                }
                else
                {
                    // 풀에 남은 오브젝트가 없으면 새로 생성
                    weapon = Instantiate(item.prefab, position, rotation);
                }
                return weapon;
            }
        }
        Debug.LogError("Weapon type not found in pool: " + weaponType);
        return null;
    }

    // 사용이 끝난 무기를 다시 풀에 반환하여 재사용
    public void ReturnWeapon(string weaponType, GameObject weapon)
    {
        foreach (WeaponPoolItem item in poolItems)
        {
            if (item.weaponType == weaponType)
            {
                weapon.SetActive(false);
                item.pool.Enqueue(weapon);
                return;
            }
        }
        Debug.LogError("Weapon type not found in pool: " + weaponType);
        Destroy(weapon);
    }

    // 적 소환
    public GameObject SpawnEnemy(string enemyTypeName, Vector3 position, Quaternion rotation)
    {
        foreach (EnemyPoolItem item in enemyPoolItems)
        {
            if (item.enemyTypeName == enemyTypeName)
            {
                GameObject enemy;
                if (item.pool.Count > 0)
                {
                    enemy = item.pool.Dequeue();
                }
                else
                {
                    enemy = Instantiate(item.prefab);
                }
                enemy.transform.position = position;
                enemy.transform.rotation = rotation;
                enemy.SetActive(true);
                return enemy;
            }
        }
        Debug.LogError("Enemy type not found in pool: " + enemyTypeName);
        return null;
    }

    // 적 반환
    public void ReturnEnemy(string enemyTypeName, GameObject enemy)
    {
        foreach (EnemyPoolItem item in enemyPoolItems)
        {
            if (item.enemyTypeName == enemyTypeName)
            {
                enemy.SetActive(false);
                item.pool.Enqueue(enemy);
                return;
            }
        }
        Debug.LogError("Enemy type not found in pool: " + enemyTypeName);
        Destroy(enemy);
    }
    public void ReturnEnemy(GameObject enemy)
    {
        string typeName = enemy.name.Replace("(Clone)", "").Trim();
        ReturnEnemy(typeName, enemy);
    }

}
