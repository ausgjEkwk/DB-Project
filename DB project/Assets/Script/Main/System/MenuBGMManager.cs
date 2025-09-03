using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBGMManager : MonoBehaviour
{
    public static MenuBGMManager Instance { get; private set; } // 싱글톤 인스턴스

    [Header("BGM")]
    public AudioSource bgmSource; // Menu 씬에서 재생할 BGM

    [Header("SFX - Select")]
    public AudioSource selectSource; // 선택 효과음 전용 AudioSource
    public AudioClip selectClip;     // 선택 효과음 클립
    [Range(0f, 1f)]
    public float selectVolume = 1f;  // 선택 효과음 볼륨

    private void Awake()
    {
        // 싱글톤 유지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 기존 인스턴스 존재 시 제거
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 삭제 방지
    }

    private void OnEnable()
    {
        // 씬 로드 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 씬 로드 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때 호출
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioListener listener = GetComponent<AudioListener>();

        if (scene.name == "Menu")
        {
            // Menu 씬이면 BGM 활성화 및 재생
            bgmSource.enabled = true;
            if (!bgmSource.isPlaying) bgmSource.Play();

            // AudioListener 활성화
            if (listener != null) listener.enabled = true;
        }
        else
        {
            // Menu가 아닌 씬이면 BGM 정지
            if (bgmSource.isPlaying) bgmSource.Stop();

            // AudioListener 비활성화
            if (listener != null) listener.enabled = false;
        }
    }

    // 선택 효과음 재생 (Z 또는 Enter 입력 시)
    public void PlaySelectSFX()
    {
        if (selectSource != null && selectClip != null)
            selectSource.PlayOneShot(selectClip, selectVolume);
    }
}
