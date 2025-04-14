using UnityEngine;
using System.Collections;

public class UpgradeItem : MonoBehaviour
{
    // 기존 fallSpeed는 사용하지 않고, 일반 낙하 속도를 별도로 관리
    public float normalFallSpeed = 1f; // 드리프트 후 정상 낙하 속도
    [SerializeField]
    private GameObject circlesprite;

    // 이미지 순서: 0=Gun, 1=Bomb, 2=Raser, 3=MissileLauncher
    public Sprite[] cycleSprites; // 반드시 4개의 스프라이트 할당 (인스펙터)
    private SpriteRenderer spriteRenderer;
    private int currentTypeIndex = 0;
    private float typeCycleTimer = 0f;
    private float typeCycleInterval = 5f; // 5초마다 타입 변경

    // 초기 움직임 설정
    private float driftTimer = 0f;            // 누적 드리프트 시간
    private float driftMoveDistance = 0.5f;   // 매 드리프트 이동 거리
    private bool driftPhaseOver = false;      // 드리프트 단계 종료 여부
    private bool isDrifting = false;

    // 3방향 (남, 동남, 서남) – normalized 벡터
    private readonly Vector3[] driftDirections = new Vector3[]
    {
    new Vector3(1,-1,0).normalized,
    new Vector3(0,-1,0).normalized,
    new Vector3(-1,-1,0).normalized
    };

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

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
        // 드리프트 단계: 드랍 후 처음 driftDuration 동안, 1초마다 3방향 중 무작위 이동
        if (!driftPhaseOver)
        {
            driftTimer += Time.deltaTime;
            if (!isDrifting)
            {
                int randIndex = Random.Range(0, driftDirections.Length);
                Vector3 targetPos = transform.position + driftDirections[randIndex] * driftMoveDistance;
                StartCoroutine(DriftMove(targetPos));
            }
        }

        // 아이템 타입 순환
        typeCycleTimer += Time.deltaTime;
        if (typeCycleTimer > 4)
        {
            StartCoroutine(BlinkCircleSprite());
        }
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

    private IEnumerator DriftMove(Vector3 targetPosition)
    {
        isDrifting = true;
        Vector3 startPos = transform.position;
        float elapsed = 0f;
        float duration = 0.5f; // 이동 시간

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        yield return new WaitForSeconds(3f);
        isDrifting = false;
    }

    private IEnumerator BlinkCircleSprite()
    {
        if (circlesprite == null) yield break;

        SpriteRenderer circleRenderer = circlesprite.GetComponent<SpriteRenderer>();
        circlesprite.SetActive(true);

        for (int i = 0; i < 5; i++)
        {
            circleRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            circleRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        circlesprite.SetActive(false);
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
                if (this != null && gameObject != null)
                {
                    WeaponPool.Instance.ReturnWeapon("UpgradeItem", gameObject);
                }
            }
        }
    }

}
