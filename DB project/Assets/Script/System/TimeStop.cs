using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TimeStop : MonoBehaviour
{
    public static TimeStop Instance { get; private set; }
    public GameObject grayscaleOverlayUI; // 흑백 UI 오버레이

    private Animator playerAnimator;
    private float originalPlayerAnimatorSpeed = 1f;
    private PlayerShooter playerShooter;
    private List<Animator> supportAnimators = new List<Animator>();
    private List<float> supportAnimatorSpeeds = new List<float>();

    public bool IsTimeStopped { get; private set; } = false;
    public static bool IsStopped => Instance != null && Instance.IsTimeStopped;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(AssignGrayscaleOverlay());
    }

    private IEnumerator AssignGrayscaleOverlay()
    {
        // 씬이 완전히 초기화될 때까지 2프레임 대기
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
                grayscaleOverlayUI.SetActive(false); // 초기화
            }
        }
    }

    public void StartTimeStop()
    {
        IsTimeStopped = true;

        // 플레이어 정지
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.enabled = false;
            playerAnimator = player.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                originalPlayerAnimatorSpeed = playerAnimator.speed;
                playerAnimator.speed = 0f;
            }

            playerShooter = player.GetComponent<PlayerShooter>();
            if (playerShooter != null)
                playerShooter.enabled = false;
        }

        // SupportShooter 정지
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

        // BossSpecial 칼 이동 일시정지 호출
        BossSpecial bossSpecial = FindObjectOfType<BossSpecial>();
        if (bossSpecial != null)
            bossSpecial.PauseBullets();

        // 배경 정지
        BackgroundScroll bg = FindObjectOfType<BackgroundScroll>();
        if (bg != null) bg.enabled = false;

        // 흑백 UI 켜기
        if (grayscaleOverlayUI != null)
            grayscaleOverlayUI.SetActive(true);
    }

    public void EndTimeStop()
    {
        IsTimeStopped = false;

        // 플레이어 복구
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.enabled = true;
            if (playerAnimator != null)
                playerAnimator.speed = originalPlayerAnimatorSpeed;
            if (playerShooter != null)
                playerShooter.enabled = true;
        }

        // SupportShooter 복구
        SupportShooter[] shooters = FindObjectsOfType<SupportShooter>();
        for (int i = 0; i < shooters.Length; i++)
        {
            shooters[i].enabled = true;
            Animator anim = shooters[i].GetComponent<Animator>();
            if (anim != null && i < supportAnimatorSpeeds.Count)
                anim.speed = supportAnimatorSpeeds[i];
        }

        // BossSpecial 칼 이동 재개 호출
        BossSpecial bossSpecial = FindObjectOfType<BossSpecial>();
        if (bossSpecial != null)
            bossSpecial.ResumeBullets();

        // 배경 복구
        BackgroundScroll bg = FindObjectOfType<BackgroundScroll>();
        if (bg != null) bg.enabled = true;

        // 흑백 UI 끄기
        if (grayscaleOverlayUI != null)
            grayscaleOverlayUI.SetActive(false);
    }
}
