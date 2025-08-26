using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBGMManager : MonoBehaviour
{
    public static MenuBGMManager Instance { get; private set; }

    [Header("BGM")]
    public AudioSource bgmSource; // Menu 씬 BGM

    [Header("SFX - Select")]
    public AudioSource selectSource; // 선택 효과음 전용
    public AudioClip selectClip;     // 선택 효과음
    [Range(0f, 1f)]
    public float selectVolume = 1f;  // 선택 효과음 볼륨

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
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
        AudioListener listener = GetComponent<AudioListener>();
        if (scene.name == "Menu")
        {
            bgmSource.enabled = true;
            if (!bgmSource.isPlaying) bgmSource.Play();
            if (listener != null) listener.enabled = true;
        }
        else
        {
            if (bgmSource.isPlaying) bgmSource.Stop();
            if (listener != null) listener.enabled = false;
        }
    }

    // 선택 효과음만 재생 (Z/Enter 눌렀을 때)
    public void PlaySelectSFX()
    {
        if (selectSource != null && selectClip != null)
            selectSource.PlayOneShot(selectClip, selectVolume);
    }
}
