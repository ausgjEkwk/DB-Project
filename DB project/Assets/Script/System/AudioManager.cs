using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM Clips")]
    public AudioClip normalBGM;
    public AudioClip bossBGM;
    public AudioClip playerDeathBGM;

    private AudioSource audioSource;
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
            return;
        }
    }

    private void Start()
    {
        PlayNormalBGM();
    }

    public void PlayNormalBGM()
    {
        if (audioSource.clip == normalBGM) return;
        audioSource.clip = normalBGM;
        audioSource.Play();
        isBossActive = false;
        isPlayerDead = false;
    }

    public void PlayBossBGM()
    {
        if (audioSource.clip == bossBGM) return;
        audioSource.clip = bossBGM;
        audioSource.Play();
        isBossActive = true;
    }

    public void PlayPlayerDeathBGM()
    {
        if (audioSource.clip == playerDeathBGM) return;
        audioSource.clip = playerDeathBGM;
        audioSource.Play();
        isPlayerDead = true;
    }

    // 자동 감지용 호출 함수
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
