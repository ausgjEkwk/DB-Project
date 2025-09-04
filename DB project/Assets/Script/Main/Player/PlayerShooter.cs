using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public GameObject bulletPrefab;              // 플레이어 총알 프리팹
    public Transform firePoint;                  // 총알 발사 위치
    public float bulletSpeed = 10f;              // 총알 속도
    public float fireRate = 0.25f;               // 연속 발사 간격

    public GameObject boomPrefab;                // 폭탄 프리팹
    public Vector3 boomSpawnPosition = Vector3.zero; // 폭탄 생성 위치

    private float fireCooldown = 0f;             // 총알 발사 쿨다운
    private List<SupportShooter> supportShooters = new List<SupportShooter>(); // 등록된 서포트 리스트

    // === 폭탄 관련 ===
    private int boomCount = 1;                   // 현재 폭탄 개수
    private int maxBoomCount = 3;                // 최대 폭탄 개수
    private float boomRechargeTime = 10f;        // 폭탄 자동 충전 시간
    private float boomRechargeTimer = 0f;        // 폭탄 충전 타이머

    private BoomUIManager boomUIManager;         // 폭탄 UI 연동

    private AudioManager audioManager;
    private BAudioManager bAudioManager;

    private bool shootLocked = false; // 공격 차단 여부

    void Start()
    {
        boomUIManager = FindObjectOfType<BoomUIManager>();  // 씬에서 BoomUIManager 검색
        if (boomUIManager != null)
        {
            boomUIManager.UpdateBoomUI(boomCount);         // UI 초기화
        }

        // AudioManager/BossAudioManager 참조
        audioManager = AudioManager.Instance;
        bAudioManager = BAudioManager.Instance;
    }

    void Update()
    {
        if (shootLocked) return;  // 컷씬 중에는 공격 로직 무시

        fireCooldown -= Time.deltaTime;

        // ───────── 플레이어 공격 ─────────
        if (Input.GetKey(KeyCode.Z) && fireCooldown <= 0f)
        {
            Fire();
            fireCooldown = fireRate;

            foreach (SupportShooter support in supportShooters)
                support?.Fire();
        }

        // ───────── 폭탄 사용 ─────────
        if (Input.GetKeyDown(KeyCode.X))
        {
            if ((GamePauseUIManager.Instance != null && GamePauseUIManager.Instance.IsShown) ||
                (FindObjectOfType<GameOverUIManager>()?.IsShown == true))
                return;

            if (boomPrefab != null && boomCount > 0)
            {
                Instantiate(boomPrefab, boomSpawnPosition, Quaternion.identity);
                boomCount--;
                boomUIManager?.UpdateBoomUI(boomCount);
            }
        }

        // 폭탄 자동 충전
        if (boomCount < maxBoomCount)
        {
            boomRechargeTimer += Time.deltaTime;
            if (boomRechargeTimer >= boomRechargeTime)
            {
                boomRechargeTimer = 0f;
                boomCount++;
                boomUIManager?.UpdateBoomUI(boomCount);
            }
        }
    }

    // ───────── 외부에서 공격 차단 ─────────
    public void SetShootLock(bool lockShoot)
    {
        shootLocked = lockShoot;
    }

    // ───────── 총알 발사 함수 ─────────
    void Fire()
    {
        if (bulletPrefab != null && firePoint != null)   // 총알과 발사 위치가 존재하면
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity); // 총알 생성
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.up * bulletSpeed;      // 위 방향으로 이동

            // 공격 사운드 재생 (Main: AudioManager, Boss: BAudioManager)
            if (audioManager != null)
                audioManager.PlayPlayerAttackSFX();
            else if (bAudioManager != null)
                bAudioManager.PlayerAttack();
        }
    }

    // ───────── 서포트 등록 ─────────
    public void RegisterSupportShooter(SupportShooter support)
    {
        if (!supportShooters.Contains(support))
            supportShooters.Add(support);
    }
}
