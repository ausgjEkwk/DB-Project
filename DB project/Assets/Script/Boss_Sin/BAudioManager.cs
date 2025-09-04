using UnityEngine;
using UnityEngine.SceneManagement;
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
    private AudioListener audioListener;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.volume = bgmVolume;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;

            audioListener = GetComponent<AudioListener>();
            if (audioListener == null)
                audioListener = gameObject.AddComponent<AudioListener>();

            AudioListener[] listeners = FindObjectsOfType<AudioListener>();
            foreach (var listener in listeners)
            {
                if (listener != audioListener)
                    listener.enabled = false;
            }

            if (bossBGM != null)
                bossBGM.LoadAudioData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        audioListener.enabled = (scene.name == "Boss");
        if (scene.name != "Boss" && bgmSource.isPlaying)
            bgmSource.Stop();
    }

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
}
