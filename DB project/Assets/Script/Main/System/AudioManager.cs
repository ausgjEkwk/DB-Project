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

    private AudioSource activeSource;
    private AudioSource sfxSource;
    private Coroutine fadeCoroutine;

    private bool isBossActive = false;
    private bool isPlayerDead = false;
    private bool isPaused = false;

    private AudioListener audioListener;

    private void Awake()
    {
        if (Instance == null)
        {
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

        audioListener.enabled = SceneManager.GetActiveScene().name == "Main";
        if (SceneManager.GetActiveScene().name == "Main")
            PlayNormalBGM();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        audioListener.enabled = (scene.name == "Main");

        if (scene.name == "Menu") return;

        if (scene.name == "Main")
        {
            StopBGMWithFadeImmediate();
            isBossActive = false;
            isPlayerDead = false;
            PlayNormalBGM();
        }
    }

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
        isPaused = false;
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

    public void PlayPlayerAttackSFX(float volume = -1f)
    {
        if (StageClearUIManager.Instance != null && StageClearUIManager.Instance.IsShown) return;
        if (isPaused || isPlayerDead) return;

        float v = (volume < 0f) ? playerAttackVolume : Mathf.Clamp01(volume);
        if (playerAttackClip != null)
            sfxSource.PlayOneShot(playerAttackClip, v);
    }

    public void PlayPlayerHitSFX(float volume = -1f)
    {
        if (StageClearUIManager.Instance != null && StageClearUIManager.Instance.IsShown) return;
        if (isPaused || isPlayerDead) return;

        float v = (volume < 0f) ? playerHitVolume : Mathf.Clamp01(volume);
        if (playerHitClip != null)
            sfxSource.PlayOneShot(playerHitClip, v);
    }

    public void SetAttackVolume(float volume) => playerAttackVolume = Mathf.Clamp01(volume);
    public void SetHitVolume(float volume) => playerHitVolume = Mathf.Clamp01(volume);
    public void SetBGMVolume(float volume)
    {
        if (activeSource != null)
            activeSource.volume = Mathf.Clamp01(volume);
    }

    public void PauseAllAudio()
    {
        if (isPaused) return;
        isPaused = true;
        if (activeSource.isPlaying) activeSource.Pause();
        if (sfxSource.isPlaying) sfxSource.Pause();
    }

    public void ResumeAllAudio()
    {
        if (!isPaused) return;
        isPaused = false;
        activeSource.UnPause();
        sfxSource.UnPause();
    }
}
