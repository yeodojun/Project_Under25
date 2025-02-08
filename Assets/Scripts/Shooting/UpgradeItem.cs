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
        if (other.CompareTag("Player")) // 플레이어가 아이템 먹음
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.UpgradeWeapon(); // 무기 업그레이드
                Destroy(gameObject);
            }
        }
    }
}
