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

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    private bool isBossActive = false;
    private bool isPlayerDead = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayNormalBGM();
    }

    // 내부 BGM 페이드 전환
    private void PlayBGMWithFade(AudioClip newClip, float targetVolume, bool loop)
    {
        if (newClip == null) return;
        if (audioSource.clip == newClip) return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeBGM(newClip, targetVolume, loop));
    }

    private IEnumerator FadeBGM(AudioClip newClip, float targetVolume, bool loop)
    {
        float startVolume = audioSource.volume;
        float time = 0f;

        // 페이드 아웃
        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
            yield return null;
        }

        audioSource.clip = newClip;
        audioSource.loop = loop;
        audioSource.Play();

        // 페이드 인
        time = 0f;
        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, time / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    // 🔹 일반 BGM
    public void PlayNormalBGM()
    {
        PlayBGMWithFade(normalBGM, normalVolume, true);
        isBossActive = false;
        isPlayerDead = false;
    }

    // 🔹 보스 BGM
    public void PlayBossBGM()
    {
        PlayBGMWithFade(bossBGM, bossVolume, true);
        isBossActive = true;
    }

    // 🔹 플레이어 사망 BGM 즉시 재생
    public void PlayPlayerDeathBGM()
    {
        if (playerDeathBGM == null) return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        audioSource.Stop();
        audioSource.clip = playerDeathBGM;
        audioSource.volume = playerDeathVolume;
        audioSource.loop = false;
        audioSource.Play();

        isPlayerDead = true;
    }

    // 🔹 BGM 정지 (페이드 아웃)
    public void StopBGMWithFade()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutAndStop());
    }

    private IEnumerator FadeOutAndStop()
    {
        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = null;
        audioSource.volume = startVolume;
        isBossActive = false;
    }

    // 🔹 Retry 초기화
    public void RetryReset()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        audioSource.Stop();
        audioSource.clip = null;

        isPlayerDead = false;
        isBossActive = false;

        PlayNormalBGM();
    }

    // 🔹 외부 호출
    public void BossAppeared()
    {
        if (!isBossActive && !isPlayerDead)
            PlayBossBGM();
    }

    public void PlayerDied()
    {
        if (!isPlayerDead)
            PlayPlayerDeathBGM();
    }
}
