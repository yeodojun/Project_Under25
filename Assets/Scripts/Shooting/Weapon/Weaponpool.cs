using System.Collections.Generic;
using UnityEngine;

public class WeaponPool : MonoBehaviour
{
    public static WeaponPool Instance;

    // 각 무기 유형의 풀 정보를 담는 클래스
    [System.Serializable]
    public class WeaponPoolItem
    {
        [Tooltip("무기 식별자 (Player : Bullet, Bullet1, Beam, BBeam, UBeam, MissileLauncher, Missile, UpgradeItem), (Enemy : Fire, Dust, Gun, Boom, LaserGun, Scream, Laser)")]
        public string weaponType;
        [Tooltip("해당 무기의 프리팹")]
        public GameObject prefab;
        [Tooltip("풀에 미리 생성해 둘 오브젝트 개수")]
        public int poolSize = 30;
        [HideInInspector]
        public Queue<GameObject> pool;
    }

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
}
