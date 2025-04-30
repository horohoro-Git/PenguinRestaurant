using UnityEngine;

public class MainCanvasController : MonoBehaviour
{
    private static MainCanvasController instance; // ***** 중복 방지를 위한 static 변수

    private void Awake()
    {
        // ***** 중복된 mainCanvas가 생성되지 않도록 방지
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // ***** 씬 전환 시 오브젝트 유지
    }
}
