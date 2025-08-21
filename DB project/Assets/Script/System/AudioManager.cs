using UnityEngine;
using System.Collections;

// AudioManager : 게임 전체 BGM 및 SFX 관리
// - BGM: 일반, 보스, 플레이어 사망 지원
// - SFX: 플레이어 공격, 피격 지원 (볼륨 개별 조절 가능)
// - 페이드인/페이드아웃, Singleton 구조

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
    public AudioClip playerAttackClip;   // 플레이어 공격 효과음
    [Range(0f, 1f)] public float playerAttackVolume = 1f; // 공격 볼륨 (개별)

    public AudioClip playerHitClip;      // 플레이어 피격 효과음
    [Range(0f, 1f)] public float playerHitVolume = 1f;    // 피격 볼륨 (개별)

    [Header("Fade Settings")]
    public float fadeDuration = 1f;      // BGM 페이드 인/아웃 시간
    public float delayBetweenFades = 1f; // BGM 전환 시 대기 시간

    private AudioSource activeSource;    // BGM 전용 AudioSource
    private AudioSource sfxSource;       // SFX 전용 AudioSource
    private Coroutine fadeCoroutine;     // BGM 페이드 코루틴 저장

    private bool isBossActive = false;   // 보스 등장 여부
    private bool isPlayerDead = false;   // 플레이어 사망 여부

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 🔹 기존 씬 AudioListener 제거 (중복 방지)
            AudioListener[] listeners = FindObjectsOfType<AudioListener>();
            foreach (var listener in listeners)
            {
                if (listener.gameObject != gameObject)
                    Destroy(listener);
            }

            // BGM용 AudioSource 초기화
            activeSource = gameObject.AddComponent<AudioSource>();
            activeSource.loop = true;

            // SFX용 AudioSource 초기화
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        // AudioClip 미리 로드
        normalBGM?.LoadAudioData();
        bossBGM?.LoadAudioData();
        playerDeathBGM?.LoadAudioData();

        // NormalBGM 자동 재생
        if (normalBGM != null)
        {
            activeSource.clip = normalBGM;
            activeSource.volume = 0f;
            activeSource.loop = true;
            activeSource.Play();
            StartCoroutine(FadeIn(activeSource, normalVolume, fadeDuration)); // 페이드 인
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
    #endregion

    #region SFX Control
    // 플레이어 공격 SFX
    public void PlayPlayerAttackSFX(float volume = -1f)
    {
        float v = (volume < 0f) ? playerAttackVolume : Mathf.Clamp01(volume);
        if (playerAttackClip != null)
            sfxSource.PlayOneShot(playerAttackClip, v);
    }

    // 플레이어 피격 SFX
    public void PlayPlayerHitSFX(float volume = -1f)
    {
        float v = (volume < 0f) ? playerHitVolume : Mathf.Clamp01(volume);
        if (playerHitClip != null)
            sfxSource.PlayOneShot(playerHitClip, v);
    }

    // 런타임 볼륨 조절
    public void SetAttackVolume(float volume)
    {
        playerAttackVolume = Mathf.Clamp01(volume);
    }

    public void SetHitVolume(float volume)
    {
        playerHitVolume = Mathf.Clamp01(volume);
    }

    public void SetBGMVolume(float volume)
    {
        activeSource.volume = Mathf.Clamp01(volume);
    }
    #endregion
}
