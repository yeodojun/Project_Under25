// 초기 드랍 25퍼는 아님 그냥 게임 타이머로 게임 진행 5초마다 드랍되는 아이템이 달라짐 그냥 착시효과로 그렇게 넣음

using UnityEngine;

public class UpgradeItem : MonoBehaviour
{
    // [기존 fallSpeed는 사용하지 않고, 일반 낙하 속도를 별도로 관리]
    public float normalFallSpeed = 1f; // 드리프트 후 정상 낙하 속도

    // ▷ 이미지/타입 순환을 위한 설정 (순서: 0=Gun, 1=Bomb, 2=Raser, 3=MissileLauncher)
    public Sprite[] cycleSprites; // 반드시 4개의 스프라이트 할당 (인스펙터)
    private SpriteRenderer spriteRenderer;
    private int currentTypeIndex = 0;
    private float typeCycleTimer = 0f;
    private float typeCycleInterval = 5f; // 5초마다 타입 변화

    // ▷ 초기 드리프트(움직임) 설정
    private float driftDuration = 4f;       // 드리프트 지속 시간 (초)
    private float driftInterval = 0.5f;       // 0.5초마다 드리프트 이동
    private float driftTimer = 0f;            // 누적 드리프트 시간
    private float driftMoveTimer = 0f;        // 드리프트 이동 타이머
    private float driftMoveDistance = 0.1f;   // 매드리프트 이동 거리
    private bool driftPhaseOver = false;      // 드리프트 단계 종료 여부

    // 8방향 (동, 서, 남, 북, 동북, 동남, 서북, 서남) – normalized 벡터
    private readonly Vector3[] driftDirections = new Vector3[]
    {
        new Vector3(0,1,0),
        new Vector3(1,0,0),
        new Vector3(0,-1,0),
        new Vector3(-1,0,0),
        (new Vector3(1,1,0)).normalized,
        (new Vector3(1,-1,0)).normalized,
        (new Vector3(-1,1,0)).normalized,
        (new Vector3(-1,-1,0)).normalized
    };

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 초기 타입 설정 (cycleSprites[0] = Gun 이미지)
        if (cycleSprites != null && cycleSprites.Length >= 4)
        {
            spriteRenderer.sprite = cycleSprites[currentTypeIndex];
        }
    }

    void Update()
    {
        // ▷ 드리프트 단계: 드랍 후 처음 driftDuration 동안, 0.5초마다 8방향 중 무작위 이동
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
            // ▷ 드리프트 단계 종료 후에는 아래로 일정 속도로 낙하
            transform.position += Vector3.down * normalFallSpeed * Time.deltaTime;
        }

        // ▷ 타입(아이템) 순환: 5초마다 다음 타입으로 변경 (순환 순서: Gun -> Bomb -> Raser -> MissileLauncher)
        typeCycleTimer += Time.deltaTime;
        if (typeCycleTimer >= typeCycleInterval)
        {
            typeCycleTimer = 0f;
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
                // 타입에 따라 플레이어의 UpgradeWeapon()를 호출 (Bomb의 경우는 단순 로그 처리)
                // 순서: 0=Gun, 1=Bomb, 2=Raser, 3=MissileLauncher
                if (currentTypeIndex == 0)
                {
                    player.UpgradeWeapon(Player.ActiveWeapon.Gun);
                    Debug.Log("Gun upgrade item acquired!");
                }
                else if (currentTypeIndex == 1)
                {
                    Debug.Log("Bomb upgrade item acquired!");
                    // player.UpgradeBomb() 등 추가 처리 예정
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
