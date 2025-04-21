using UnityEngine;
using System.Collections;

public class MissileLauncher : MonoBehaviour
{
    public float rotationSpeed = 90f; // 초당 회전 속도 (°)
    public float radius = 0.7f;         // 플레이어로부터의 거리
    private static float globalAngle = 0f;
    private int sideSign = 1;           // 1: 오른쪽, -1: 왼쪽 런처 결정
    private Player player;

    private float fireDelay = 0.3f;     // 발사 딜레이 (초)
    private float lastFireTime = 0f;

    // Initialize 함수: Player 참조와 side(1 또는 -1) 전달.
    public void Initialize(Player playerRef, int side)
    {
        player = playerRef;
        sideSign = side;
    }

    void Update()
    {
        if (player == null) return;

        // 모든 런처가 공유하는 globalAngle을 업데이트 (반시계 방향)
        globalAngle += rotationSpeed * Time.deltaTime;
        globalAngle %= 360f;

        // 각 launcher의 effectiveAngle 계산: 오른쪽은 globalAngle, 왼쪽은 globalAngle + 180°
        float effectiveAngle = globalAngle + (sideSign == 1 ? 0f : 180f);
        Vector3 offset = new Vector3(Mathf.Cos(effectiveAngle * Mathf.Deg2Rad), Mathf.Sin(effectiveAngle * Mathf.Deg2Rad), 0f) * radius;
        transform.position = player.transform.position + offset;

        // (옵션) launcher가 플레이어를 바라보도록 하려면 아래 줄을 활성화 (단, 미사일 발사 방향은 고정)
        // transform.up = (player.transform.position - transform.position).normalized;

        // 발사 조건: fireDelay마다, 화면 내 적이 하나라도 있으면 미사일을 발사
        if (Time.time - lastFireTime >= fireDelay && IsAnyEnemyOnScreen())
        {
            // 미사일은 항상 Quaternion.identity (월드 up 방향)으로 발사
            Pool.Instance.SpawnWeapon("Missile", transform.position, Quaternion.identity);
            lastFireTime = Time.time;
        }
    }

    // 새 API 사용: 화면 내 적이 하나라도 있으면 true 반환
    bool IsAnyEnemyOnScreen()
    {
        Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        return enemies.Length > 0;
    }
}
