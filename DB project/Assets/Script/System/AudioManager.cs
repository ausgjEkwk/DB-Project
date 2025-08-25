using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM Clips")]
    public AudioClip normalBGM;
    [Range(0f, 1f)] public float normalVolume = 1f;

    public AudioClip bossBGM;
    [Range(0f, 1f)] public float bossVolume = 1f;

    public AudioClip playerDeathBGM;
    [Range(0f, 1f)] public float playerDeathVolume = 1f;

    [Header("SFX Clips")]
    public AudioClip playerAttackClip;
    [Range(0f, 1f)] public float playerAttackVolume = 1f;

    public AudioClip playerHitClip;
    [Range(0f, 1f)] public float playerHitVolume = 1f;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;
    public float delayBetweenFades = 1f;

    private AudioSource activeSource;
    private AudioSource sfxSource;
    private Coroutine fadeCoroutine;

    private bool isBossActive = false;
    private bool isPlayerDead = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // AudioListener 중복 방지
            AudioListener[] listeners = FindObjectsOfType<AudioListener>();
            foreach (var listener in listeners)
            {
                if (listener.gameObject != gameObject)
                    Destroy(listener);
            }

            activeSource = gameObject.AddComponent<AudioSource>();
            activeSource.loop = true;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        normalBGM?.LoadAudioData();
        bossBGM?.LoadAudioData();
        playerDeathBGM?.LoadAudioData();

        if (SceneManager.GetActiveScene().name == "Main")
        {
            PlayNormalBGM();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Menu씬에서는 아무것도 하지 않음 → MenuBGMManager 전담
        if (scene.name == "Menu") return;

        // Main씬 진입 시
        if (scene.name == "Main")
        {
            // 이전 Boss/PlayerDeath 상태도 초기화 후 NormalBGM
            StopBGMWithFadeImmediate();
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
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
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
            source.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }
        source.volume = 0f;
        source.Stop();
    }

    private IEnumerator SwitchBGM(AudioClip newClip, float targetVolume, bool loop)
    {
        if (activeSource.isPlaying)
        {
            yield return FadeOut(activeSource, fadeDuration);
            yield return new WaitForSecondsRealtime(delayBetweenFades);
        }

        activeSource.clip = newClip;
        activeSource.loop = loop;
        activeSource.volume = 0f;
        activeSource.Play();
        yield return FadeIn(activeSource, targetVolume, fadeDuration);
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
        fadeCoroutine = StartCoroutine(FadeOut(activeSource, fadeDuration));
    }

    private void StopBGMWithFadeImmediate()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        activeSource.Stop();
        activeSource.volume = 0f;
    }

    public void RetryReset()
    {
        StopBGMWithFadeImmediate();
        isPlayerDead = false;
        isBossActive = false;
        PlayNormalBGM();
    }

    public void BossAppeared()
    {
        if (!isBossActive && !isPlayerDead) PlayBossBGM();
    }

    public void PlayerDied()
    {
        if (!isPlayerDead) PlayPlayerDeathBGM();
    }
    #endregion

    #region SFX Control
    public void PlayPlayerAttackSFX(float volume = -1f)
    {
        float v = (volume < 0f) ? playerAttackVolume : Mathf.Clamp01(volume);
        if (playerAttackClip != null)
            sfxSource.PlayOneShot(playerAttackClip, v);
    }

    public void PlayPlayerHitSFX(float volume = -1f)
    {
        float v = (volume < 0f) ? playerHitVolume : Mathf.Clamp01(volume);
        if (playerHitClip != null)
            sfxSource.PlayOneShot(playerHitClip, v);
    }

    public void SetAttackVolume(float volume) => playerAttackVolume = Mathf.Clamp01(volume);
    public void SetHitVolume(float volume) => playerHitVolume = Mathf.Clamp01(volume);
    public void SetBGMVolume(float volume) => activeSource.volume = Mathf.Clamp01(volume);
    #endregion
}
