using UnityEngine;
using System.Collections;

public class BAudioManager : MonoBehaviour
{
    public static BAudioManager Instance { get; private set; }

    [Header("BGM")]
    public AudioClip bossBGM;
    [Range(0f, 1f)] public float bgmVolume = 1f;

    [Header("Player SFX")]
    public AudioClip playerAttackSFX;
    [Range(0f, 1f)] public float attackVolume = 1f;

    public AudioClip playerHitSFX;
    [Range(0f, 1f)] public float hitVolume = 1f;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
    }

    #region BGM Control
    public void PlayBossBGM(float fadeTime = 1f)
    {
        if (bossBGM == null) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToBGM(bossBGM, bgmVolume, fadeTime));
    }

    private IEnumerator FadeToBGM(AudioClip newClip, float targetVolume, float duration)
    {
        float startVolume = bgmSource.volume;
        float t = 0f;

        if (bgmSource.isPlaying)
        {
            // 기존 BGM 페이드 아웃
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
                yield return null;
            }
            bgmSource.Stop();
        }

        bgmSource.clip = newClip;
        bgmSource.volume = 0f;
        bgmSource.Play();

        t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(0f, targetVolume, t / duration);
            yield return null;
        }
        bgmSource.volume = targetVolume;
    }

    public void StopBGM()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        bgmSource.Stop();
    }
    #endregion

    #region SFX Control
    public void PlayerAttack()
    {
        if (playerAttackSFX == null) return;
        sfxSource.PlayOneShot(playerAttackSFX, attackVolume);
    }

    public void PlayerHit()
    {
        if (playerHitSFX == null) return;
        sfxSource.PlayOneShot(playerHitSFX, hitVolume);
    }
    #endregion

    #region Volume Control
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        if (bgmSource.isPlaying)
            bgmSource.volume = bgmVolume;
    }

    public void SetAttackVolume(float volume) => attackVolume = Mathf.Clamp01(volume);
    public void SetHitVolume(float volume) => hitVolume = Mathf.Clamp01(volume);
    #endregion
}
