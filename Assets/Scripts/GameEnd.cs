using UnityEngine;

public class GameEnd : MonoBehaviour
{
    public void QuitGame()
    {
        // 에디터에서 실행 중일 때는 아래 코드로 정지
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서는 이걸로 종료
        Application.Quit();
#endif
    }
}
