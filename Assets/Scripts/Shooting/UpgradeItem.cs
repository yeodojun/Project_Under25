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
                /*if (rand < 0.2f)
                {
                    // 현재 Gun을 업그레이드하거나 Gun 모드로 전환 (수동 업그레이드는 최대 3까지만 작동)
                    player.UpgradeWeapon(Player.ActiveWeapon.Gun);
                    Debug.Log("Gun upgrade item acquired!");
                }
                else if (rand < 0.4f)
                {
                    // 현재 Raser를 업그레이드하거나 Raser 모드로 전환
                    player.UpgradeWeapon(Player.ActiveWeapon.Raser);
                    Debug.Log("Raser upgrade item acquired!");
                }*/
                if (rand < 0.6f)
                {
                    player.UpgradeWeapon(Player.ActiveWeapon.Missile);
                }
                else if (rand < 0.8f)
                {
                    player.RecoverHealth();
                    Debug.Log("Health recovery item acquired!");
                }
                else
                {
                    player.ActivateShield();
                    Debug.Log("Shield item acquired!");
                }
                Destroy(gameObject);
            }
        }
    }
}
