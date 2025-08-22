using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBGMManager : MonoBehaviour
{
    public AudioSource bgmSource; // Menu씬에서 재생할 BGM

    private void Awake()
    {
        var existing = FindObjectsOfType<MenuBGMManager>();
        if (existing.Length > 1)
        {
            Destroy(gameObject); // 이미 존재하면 삭제
            return;
        }

        DontDestroyOnLoad(gameObject);
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
        AudioListener listener = GetComponent<AudioListener>();
        if (scene.name == "Menu")
        {
            bgmSource.enabled = true;
            if (!bgmSource.isPlaying) bgmSource.Play();
            if (listener != null)
                listener.enabled = true;
        }
        else
        {
            if (bgmSource.isPlaying) bgmSource.Stop();
            if (listener != null)
                listener.enabled = false;
        }
    }


}
