using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBGMManager : MonoBehaviour
{
    public AudioSource bgmSource; // Menu������ ����� BGM

    private void Awake()
    {
        var existing = FindObjectsOfType<MenuBGMManager>();
        if (existing.Length > 1)
        {
            Destroy(gameObject); // �̹� �����ϸ� ����
            return;
        }

        DontDestroyOnLoad(gameObject);
    }


    private void OnEnable()
    {
        // ���� �ٲ� ������ Ȯ��
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
