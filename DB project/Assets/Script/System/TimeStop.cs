using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeStop : MonoBehaviour
{
    public static TimeStop Instance { get; private set; }  // 싱글톤
    public GameObject grayscaleOverlayUI;                  // 흑백 UI 오버레이

    private Animator playerAnimator;
    private float originalPlayerAnimatorSpeed = 1f;
    private PlayerShooter playerShooter;

    private List<Animator> supportAnimators = new List<Animator>();
    private List<float> supportAnimatorSpeeds = new List<float>();

    public bool IsTimeStopped { get; private set; } = false;                 // 현재 시간정지 상태
    public static bool IsStopped => Instance != null && Instance.IsTimeStopped; // 전역 접근용

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않음
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드 후 UI 오브젝트 연결
        if (this != null)
        {
            StartCoroutine(AssignGrayscaleOverlay());
        }
    }

    private IEnumerator AssignGrayscaleOverlay()
    {
        // 씬 초기화 대기
        yield return null;
        yield return null;

        // EffectCanvas 하위 GrayscaleOverlay 검색
        GameObject canvasObj = GameObject.Find("EffectCanvas");
        if (canvasObj != null)
        {
            Transform overlay = canvasObj.transform.Find("GrayscaleOverlay");
            if (overlay != null)
            {
                grayscaleOverlayUI = overlay.gameObject;
                grayscaleOverlayUI.SetActive(false); // 초기 비활성화
            }
        }
    }

    // 시간정지 시작
    public void StartTimeStop()
    {
        IsTimeStopped = true;

        // --- 플레이어 정지 ---
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.enabled = false;
            playerAnimator = player.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                originalPlayerAnimatorSpeed = playerAnimator.speed;
                playerAnimator.speed = 0f; // 애니메이션 정지
            }

            playerShooter = player.GetComponent<PlayerShooter>();
            if (playerShooter != null)
                playerShooter.enabled = false; // 발사 정지
        }

        // --- SupportShooter 정지 ---
        supportAnimators.Clear();
        supportAnimatorSpeeds.Clear();
        SupportShooter[] shooters = FindObjectsOfType<SupportShooter>();
        foreach (SupportShooter shooter in shooters)
        {
            Animator anim = shooter.GetComponent<Animator>();
            if (anim != null)
            {
                supportAnimators.Add(anim);
                supportAnimatorSpeeds.Add(anim.speed);
                anim.speed = 0f;
            }
            shooter.enabled = false;
        }

        // --- BossSpecial 칼 이동 정지 ---
        BossSpecial bossSpecial = FindObjectOfType<BossSpecial>();
        if (bossSpecial != null)
            bossSpecial.PauseBullets();

        // --- 배경 스크롤 정지 ---
        BackgroundScroll bg = FindObjectOfType<BackgroundScroll>();
        if (bg != null) bg.enabled = false;

        // --- 흑백 UI 활성화 ---
        if (grayscaleOverlayUI != null)
            grayscaleOverlayUI.SetActive(true);

        // --- 모든 BGM / 효과음 일시정지 ---
        if (AudioManager.Instance != null)
            AudioManager.Instance.PauseAllAudio();
    }

    // 시간정지 종료
    public void EndTimeStop()
    {
        IsTimeStopped = false;

        // --- 플레이어 복구 ---
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.enabled = true;
            if (playerAnimator != null)
                playerAnimator.speed = originalPlayerAnimatorSpeed;
            if (playerShooter != null)
                playerShooter.enabled = true;
        }

        // --- SupportShooter 복구 ---
        SupportShooter[] shooters = FindObjectsOfType<SupportShooter>();
        for (int i = 0; i < shooters.Length; i++)
        {
            shooters[i].enabled = true;
            Animator anim = shooters[i].GetComponent<Animator>();
            if (anim != null && i < supportAnimatorSpeeds.Count)
                anim.speed = supportAnimatorSpeeds[i];
        }

        // --- BossSpecial 칼 이동 재개 ---
        BossSpecial bossSpecial = FindObjectOfType<BossSpecial>();
        if (bossSpecial != null)
            bossSpecial.ResumeBullets();

        // --- 배경 스크롤 복구 ---
        BackgroundScroll bg = FindObjectOfType<BackgroundScroll>();
        if (bg != null) bg.enabled = true;

        // --- 흑백 UI 비활성화 ---
        if (grayscaleOverlayUI != null)
            grayscaleOverlayUI.SetActive(false);

        // --- BGM / 효과음 재개 ---
        if (AudioManager.Instance != null)
            AudioManager.Instance.ResumeAllAudio();
    }
}
