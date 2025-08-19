using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBGMManager : MonoBehaviour
{
    public AudioSource bgmSource; // Menu씬에서 재생할 BGM

    private void Awake()
    {
        // 씬 전환 시 파괴되지 않도록 유지
        DontDestroyOnLoad(gameObject);

        // 이미 BGM 오브젝트가 있다면 중복 제거
        var bgms = FindObjectsOfType<MenuBGMManager>();
        if (bgms.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        // 씬이 바뀔 때마다 확인
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu") // Menu씬일 때
        {
            if (!bgmSource.isPlaying)
                bgmSource.Play();
        }
        else // Menu씬이 아닐 때
        {
            if (bgmSource.isPlaying)
                bgmSource.Stop();
        }
    }
}
