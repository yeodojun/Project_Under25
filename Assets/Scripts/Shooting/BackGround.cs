using UnityEngine;

public class BackGround : MonoBehaviour
{
    [Header("Scrolling Settings")]
    public float moveSpeed = 3f;   // 배경 이동 속도 (Inspector에서 조절 가능)
    public bool isScrolling = true; // true일 때만 배경이 움직임

    // Reverse 모드 관련 변수
    public bool isReverseMode = false;     // reverse mode 활성화 여부 (보스 체력 5000 이하일 때 활성화)
    public int reverseDirection = 1;         // 1: 위로 이동, -1: 아래로 이동

    void Update()
    {
        if (!isScrolling) return;

        if (!isReverseMode)
        {
            // 기본 동작: 아래로 이동
            transform.position += Vector3.down * moveSpeed * Time.deltaTime;
            // 위치가 -10 이하면 20만큼 올려서 반복
            if (transform.position.y < -10f)
            {
                transform.position += new Vector3(0, 20f, 0);
            }
        }
        else
        {
            // Reverse mode: reverseDirection에 따라 이동 (1이면 위로, -1이면 아래로)
            transform.position += Vector3.up * reverseDirection * moveSpeed * Time.deltaTime;

            // y좌표가 10 이상이면 방향을 아래로 전환
            if (reverseDirection == 1 && transform.position.y >= 10f)
            {
                reverseDirection = -1;
            }
            // y좌표가 -10 이하이면 방향을 위로 전환
            else if (reverseDirection == -1 && transform.position.y <= -10f)
            {
                reverseDirection = 1;
            }
        }
    }

    // 외부에서 배경을 멈추는 메서드
    public void StopScrolling()
    {
        isScrolling = false;
    }

    // 외부에서 배경을 다시 움직이게 하는 메서드
    public void ResumeScrolling()
    {
        isScrolling = true;
    }
}
