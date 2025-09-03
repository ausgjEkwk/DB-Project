using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; } // 싱글톤 인스턴스

    [Header("BGM Clips")]
    public AudioClip normalBGM;             // 일반 스테이지 BGM
    [Range(0f, 1f)] public float normalVolume = 1f;

    public AudioClip bossBGM;               // 보스 등장 시 BGM
    [Range(0f, 1f)] public float bossVolume = 1f;

    public AudioClip playerDeathBGM;        // 플레이어 사망 시 BGM
    [Range(0f, 1f)] public float playerDeathVolume = 1f;

    [Header("SFX Clips")]
    public AudioClip playerAttackClip;      // 플레이어 공격 효과음
    [Range(0f, 1f)] public float playerAttackVolume = 1f;

    public AudioClip playerHitClip;         // 플레이어 피격 효과음
    [Range(0f, 1f)] public float playerHitVolume = 1f;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;         // BGM 페이드 시간
    public float delayBetweenFades = 1f;    // BGM 전환 시 딜레이

    // OptionPanel과 연결 가능하게 public 프로퍼티 추가
    public float ActiveBGMVolume => activeSource != null ? activeSource.volume : 1f;
    public float NormalBGMVolume
    {
        get => normalVolume;
        set
        {
            normalVolume = Mathf.Clamp01(value);
            if (!isBossActive && activeSource != null)
                activeSource.volume = normalVolume;
        }
    }
    public float BossBGMVolume
    {
        get => bossVolume;
        set
        {
            bossVolume = Mathf.Clamp01(value);
            if (isBossActive && activeSource != null)
                activeSource.volume = bossVolume;
        }
    }
    public float AttackSFXVolume
    {
        get => playerAttackVolume;
        set => playerAttackVolume = Mathf.Clamp01(value);
    }
    public float HitSFXVolume
    {
        get => playerHitVolume;
        set => playerHitVolume = Mathf.Clamp01(value);
    }

    private AudioSource activeSource;       // BGM 재생용 AudioSource
    private AudioSource sfxSource;          // SFX 재생용 AudioSource
    private Coroutine fadeCoroutine;        // BGM 페이드 코루틴 참조

    private bool isBossActive = false;      // 보스 BGM 상태
    private bool isPlayerDead = false;      // 플레이어 사망 상태
    private bool isPaused = false;          // 일시정지 상태

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지

            // AudioListener 중복 제거
            AudioListener[] listeners = FindObjectsOfType<AudioListener>();
            foreach (var listener in listeners)
            {
                if (listener.gameObject != gameObject)
                    Destroy(listener);
            }

            // AudioSource 생성
            activeSource = gameObject.AddComponent<AudioSource>();
            activeSource.loop = true;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 시 이벤트 등록
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        // 오디오 데이터 미리 로드
        normalBGM?.LoadAudioData();
        bossBGM?.LoadAudioData();
        playerDeathBGM?.LoadAudioData();

        if (SceneManager.GetActiveScene().name == "Main")
        {
            PlayNormalBGM(); // Main씬 시작 시 일반 BGM 재생
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu") return; // Menu씬은 별도 관리

        if (scene.name == "Main")
        {
            StopBGMWithFadeImmediate(); // 이전 BGM 즉시 정지
            isBossActive = false;
            isPlayerDead = false;
            PlayNormalBGM();
        }
    }

    #region BGM Fade Coroutines
    private IEnumerator FadeIn(AudioSource source, float targetVolume, float duration)
    {
        float startVolume = source.volume;
        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration); // 점진적 볼륨 증가
            yield return null;
        }
        source.volume = targetVolume;
    }

    private IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, time / duration); // 점진적 볼륨 감소
            yield return null;
        }
        source.volume = 0f;
        source.Stop(); // 완전히 정지
    }

    private IEnumerator SwitchBGM(AudioClip newClip, float targetVolume, bool loop)
    {
        if (activeSource.isPlaying)
        {
            yield return FadeOut(activeSource, fadeDuration); // 현재 BGM 페이드 아웃
            yield return new WaitForSecondsRealtime(delayBetweenFades); // 딜레이
        }

        activeSource.clip = newClip;
        activeSource.loop = loop;
        activeSource.volume = 0f;
        activeSource.Play();
        yield return FadeIn(activeSource, targetVolume, fadeDuration); // 새 BGM 페이드 인
    }
    #endregion

    #region BGM Control
    public void PlayNormalBGM()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(SwitchBGM(normalBGM, normalVolume, true));
        isBossActive = false;
        isPlayerDead = false;
    }

    public void PlayBossBGM()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(SwitchBGM(bossBGM, bossVolume, true));
        isBossActive = true;
    }

    public void PlayPlayerDeathBGM()
    {
        if (playerDeathBGM == null) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        activeSource.Stop();
        activeSource.clip = playerDeathBGM;
        activeSource.volume = playerDeathVolume;
        activeSource.loop = false;
        activeSource.Play();

        isPlayerDead = true;
    }

    public void StopBGMWithFade()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOut(activeSource, fadeDuration)); // 페이드 아웃
    }

    private void StopBGMWithFadeImmediate()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        activeSource.Stop(); // 즉시 정지
        activeSource.volume = 0f;
    }

    public void RetryReset()
    {
        StopBGMWithFadeImmediate();
        isPlayerDead = false;
        isBossActive = false;
        isPaused = false;
        PlayNormalBGM(); // 리트라이 시 초기화 후 일반 BGM 재생
    }

    public void BossAppeared()
    {
        if (!isBossActive && !isPlayerDead) PlayBossBGM(); // 보스 등장 시 BGM 전환
    }

    public void PlayerDied()
    {
        if (!isPlayerDead) PlayPlayerDeathBGM(); // 플레이어 사망 시 BGM 재생
    }
    #endregion

    #region SFX Control (StageClearUI 대응)
    public void PlayPlayerAttackSFX(float volume = -1f)
    {
        if (StageClearUIManager.Instance != null && StageClearUIManager.Instance.IsShown) return;
        if (isPaused) return;
        if (isPlayerDead) return;  // 🔹 플레이어 사망 시 공격 SFX 차단

        float v = (volume < 0f) ? playerAttackVolume : Mathf.Clamp01(volume);
        if (playerAttackClip != null)
            sfxSource.PlayOneShot(playerAttackClip, v);
    }

    public void PlayPlayerHitSFX(float volume = -1f)
    {
        if (StageClearUIManager.Instance != null && StageClearUIManager.Instance.IsShown) return;
        if (isPaused) return;
        if (isPlayerDead) return;  // 🔹 플레이어 사망 시 피격 SFX 차단

        float v = (volume < 0f) ? playerHitVolume : Mathf.Clamp01(volume);
        if (playerHitClip != null)
            sfxSource.PlayOneShot(playerHitClip, v);
    }

    public void SetAttackVolume(float volume) => playerAttackVolume = Mathf.Clamp01(volume); // 공격 볼륨 설정
    public void SetHitVolume(float volume) => playerHitVolume = Mathf.Clamp01(volume);       // 피격 볼륨 설정
    public void SetBGMVolume(float volume) => activeSource.volume = Mathf.Clamp01(volume);     // BGM 볼륨 설정
    #endregion

    #region Pause/Resume
    public void PauseAllAudio()
    {
        if (isPaused) return;
        isPaused = true;

        if (activeSource.isPlaying)
            activeSource.Pause();
        if (sfxSource.isPlaying)
            sfxSource.Pause();
    }

    public void ResumeAllAudio()
    {
        if (!isPaused) return;
        isPaused = false;

        activeSource.UnPause();
        sfxSource.UnPause();
    }
    #endregion
}
