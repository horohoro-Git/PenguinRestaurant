using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // ***** Additive 모드로 씬 로드 (중복 로드 방지)
    public void LoadSceneAdditive(string sceneName)
    {
        if (IsSceneLoaded(sceneName))
        {
            Debug.LogWarning($"씬 '{sceneName}'은 이미 로드되어 있습니다.");
            return;
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        Debug.Log($"씬 '{sceneName}'을 Additive 모드로 로드했습니다.");
    }

    // ***** 씬 언로드 (이미 언로드된 경우 방지)
    public void UnloadScene(string sceneName)
    {
        if (!IsSceneLoaded(sceneName))
        {
            Debug.LogWarning($"씬 '{sceneName}'은 이미 언로드되었습니다.");
            return;
        }

        SceneManager.UnloadSceneAsync(sceneName);
        Debug.Log($"씬 '{sceneName}'을 언로드했습니다.");
    }

    // ***** 씬 로드 상태 확인 메서드
    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
            {
                return true; // 씬이 이미 로드됨
            }
        }
        return false; // 씬이 로드되지 않음
    }
}
