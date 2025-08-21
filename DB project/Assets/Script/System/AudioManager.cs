using UnityEngine;
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

    [Header("Fade Settings")]
    public float fadeDuration = 1f;
    public float delayBetweenFades = 1f;

    private AudioSource activeSource;
    private Coroutine fadeCoroutine;

    private bool isBossActive = false;
    private bool isPlayerDead = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            activeSource = gameObject.AddComponent<AudioSource>();
            activeSource.loop = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // AudioClip 사전 로드
        if (normalBGM != null) normalBGM.LoadAudioData();
        if (bossBGM != null) bossBGM.LoadAudioData();
        if (playerDeathBGM != null) playerDeathBGM.LoadAudioData();

        // NormalBGM 즉시 시작
        if (normalBGM != null)
        {
            activeSource.clip = normalBGM;
            activeSource.volume = 0f;
            activeSource.loop = true;
            activeSource.Play();
            StartCoroutine(FadeIn(activeSource, normalVolume, fadeDuration));
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

    private IEnumerator SwitchBGM(AudioClip newClip, float targetVolume, bool loop)
    {
        // 기존 곡 페이드 아웃
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

    public void StopBGMWithFade()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOut(activeSource, fadeDuration));
    }

    public void RetryReset()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        activeSource.Stop();
        activeSource.clip = null;

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
}
