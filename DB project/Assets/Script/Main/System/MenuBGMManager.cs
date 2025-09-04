using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBGMManager : MonoBehaviour
{
    public static MenuBGMManager Instance { get; private set; }

    [Header("BGM")]
    public AudioSource bgmSource;

    [Header("SFX - Select")]
    public AudioSource selectSource;
    public AudioClip selectClip;
    [Range(0f, 1f)] public float selectVolume = 1f;

    private AudioListener audioListener;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioListener = GetComponent<AudioListener>();
        if (audioListener == null)
            audioListener = gameObject.AddComponent<AudioListener>();

        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        foreach (var listener in listeners)
        {
            if (listener != audioListener)
                listener.enabled = false;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
        {
            if (!bgmSource.isPlaying) bgmSource.Play();
            if (audioListener != null) audioListener.enabled = true;
        }
        else
        {
            if (bgmSource.isPlaying) bgmSource.Stop();
            if (audioListener != null) audioListener.enabled = false;
        }
    }

    public void PlaySelectSFX()
    {
        if (selectSource != null && selectClip != null)
            selectSource.PlayOneShot(selectClip, selectVolume);
    }
}
