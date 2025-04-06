using UnityEngine;

public class UpgradeItem : MonoBehaviour
{
    public float fallSpeed = 2f; // 아이템 떨어지는 속도

    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        // 화면 아래로 나가면 제거
        if (transform.position.y <= -5.5f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                // 0 ~ 1 사이의 랜덤값 생성
                float rand = Random.value;
                if (rand < 0.4f)
                {
                    // 총알 업그레이드 40%
                    player.UpgradeGun();
                    Debug.Log("총알 업그레이드 획득!");
                }
                else if (rand < 0.6f)
                {
                    // 미사일 업그레이드 20%
                    Debug.Log("미사일 업그레이드 획득!");
                }
                else if (rand < 0.8f)
                {
                    // 체력 회복 20% (플레이어 체력이 3 미만일 때만 회복)
                    player.RecoverHealth();
                    Debug.Log("체력 회복 아이템 획득!");
                }
                else
                {
                    // 쉴드 아이템 20%
                    player.ActivateShield();
                    Debug.Log("쉴드 아이템 획득!");
                }
                Destroy(gameObject);
            }
        }
    }
}
