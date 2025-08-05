using UnityEngine;
using System.Collections.Generic;

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

    // 편리하게 외부에서 간단히 확인 가능하도록 프로퍼티 추가
    public static bool IsStopped => Instance != null && Instance.IsTimeStopped;

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

        // 배경 복구
        BackgroundScroll bg = FindObjectOfType<BackgroundScroll>();
        if (bg != null) bg.enabled = true;

        // 흑백 UI 끄기
        if (grayscaleOverlayUI != null)
            grayscaleOverlayUI.SetActive(false);
    }
}
