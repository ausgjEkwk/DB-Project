using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBGMManager : MonoBehaviour
{
    public AudioSource bgmSource; // Menu������ ����� BGM

    private void Awake()
    {
        // �� ��ȯ �� �ı����� �ʵ��� ����
        DontDestroyOnLoad(gameObject);

        // �̹� BGM ������Ʈ�� �ִٸ� �ߺ� ����
        var bgms = FindObjectsOfType<MenuBGMManager>();
        if (bgms.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
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
