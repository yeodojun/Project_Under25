using UnityEngine;

public class UpgradeItem : MonoBehaviour
{
    // 기존 fallSpeed는 사용하지 않고, 일반 낙하 속도를 별도로 관리
    public float normalFallSpeed = 1f; // 드리프트 후 정상 낙하 속도

    // 이미지 순서: 0=Gun, 1=Bomb, 2=Raser, 3=MissileLauncher
    public Sprite[] cycleSprites; // 반드시 4개의 스프라이트 할당 (인스펙터)
    private SpriteRenderer spriteRenderer;
    private int currentTypeIndex = 0;
    private float typeCycleTimer = 0f;
    private float typeCycleInterval = 5f; // 5초마다 타입 변경

    // 초기 움직임 설정
    private float driftDuration = 4f;       // 드리프트 지속 시간 (초)
    private float driftInterval = 0.5f;       // 0.5초마다 드리프트 이동
    private float driftTimer = 0f;            // 누적 드리프트 시간
    private float driftMoveTimer = 0f;        // 드리프트 이동 타이머
    private float driftMoveDistance = 0.3f;   // 매 드리프트 이동 거리
    private bool driftPhaseOver = false;      // 드리프트 단계 종료 여부

    // 8방향 (남, 동남, 서남) – normalized 벡터
    private readonly Vector3[] driftDirections = new Vector3[]
    {
    (new Vector3(1,-1,0)).normalized,
    (new Vector3(0,-1,0)).normalized,
    (new Vector3(-1,-1,0)).normalized
    };
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 초기 타입을 0~3 사이의 난수로 설정 (각각 25% 확률)
        if (cycleSprites != null && cycleSprites.Length >= 4)
        {
            currentTypeIndex = Random.Range(0, 4);
            spriteRenderer.sprite = cycleSprites[currentTypeIndex];
        }
    }

    void Update()
    {
        // 드리프트 단계: 드랍 후 처음 driftDuration 동안, 0.5초마다 8방향 중 무작위 이동
        if (!driftPhaseOver)
        {
            driftTimer += Time.deltaTime;
            driftMoveTimer += Time.deltaTime;
            if (driftMoveTimer >= driftInterval)
            {
                int randIndex = Random.Range(0, driftDirections.Length);
                transform.position += driftDirections[randIndex] * driftMoveDistance;
                driftMoveTimer = 0f;
            }
            if (driftTimer >= driftDuration)
            {
                driftPhaseOver = true;
            }
        }
        else
        {
            // 드리프트 단계 종료 후에는 아래로 일정 속도로 낙하, 한 사이클 종료
            transform.position += Vector3.down * normalFallSpeed * Time.deltaTime;
        }

        // 아이템 타입 순환: 드랍 후 5초마다 순차적으로 타입 변경
        typeCycleTimer += Time.deltaTime;
        if (typeCycleTimer >= typeCycleInterval)
        {
            typeCycleTimer = 0f;
            // 순서대로 변경: 현재 타입의 다음 타입으로 변경 (0→1→2→3→0)
            currentTypeIndex = (currentTypeIndex + 1) % 4;
            if (cycleSprites != null && cycleSprites.Length >= 4)
            {
                spriteRenderer.sprite = cycleSprites[currentTypeIndex];
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                // currentTypeIndex에 따라 플레이어의 UpgradeWeapon()를 호출
                // 순서: 0=Gun, 1=Bomb, 2=Raser, 3=MissileLauncher
                if (currentTypeIndex == 0)
                {
                    player.UpgradeWeapon(Player.ActiveWeapon.Gun);
                    Debug.Log("Gun upgrade item acquired!");
                }
                else if (currentTypeIndex == 1)
                {
                    Debug.Log("Bomb!");
                    // Bomb 추가 예정
                }
                else if (currentTypeIndex == 2)
                {
                    player.UpgradeWeapon(Player.ActiveWeapon.Raser);
                    Debug.Log("Raser upgrade item acquired!");
                }
                else if (currentTypeIndex == 3)
                {
                    player.UpgradeWeapon(Player.ActiveWeapon.Missile);
                    Debug.Log("Missile launcher upgrade item acquired!");
                }
                Destroy(gameObject);
            }
        }
    }

}
