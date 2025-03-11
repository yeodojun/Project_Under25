using UnityEngine;

public class BackGround : MonoBehaviour
{
    [Header("Scrolling Settings")]
    public float moveSpeed = 3f;   // 배경 이동 속도 (Inspector에서 조절 가능)
    public bool isScrolling = true; // true일 때만 배경이 움직임

    void Update()
    {
        // isScrolling이 false면 Update에서 아무것도 안 함 → 배경 정지
        if (!isScrolling) return;

        // 배경을 아래로 이동
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;

        // 특정 지점에 도달하면 위로 되돌려 배경을 반복
        if (transform.position.y < -10f)
        {
            transform.position += new Vector3(0, 20f, 0);
        }
    }

    // 외부에서 배경을 멈추게 하는 메서드
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
