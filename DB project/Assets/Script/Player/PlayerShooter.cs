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

    void Start()
    {
        boomUIManager = FindObjectOfType<BoomUIManager>();  // 씬에서 BoomUIManager 검색
        if (boomUIManager != null)
        {
            boomUIManager.UpdateBoomUI(boomCount);         // UI 초기화
        }
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;                     // 쿨다운 감소

        // ───────── 플레이어 공격 ─────────
        if (Input.GetKey(KeyCode.Z) && fireCooldown <= 0f) // Z키 눌렀고 쿨다운 끝났으면
        {
            Fire();                                        // 총알 발사
            fireCooldown = fireRate;                       // 쿨다운 초기화

            foreach (SupportShooter support in supportShooters) // 서포트 발사
            {
                if (support != null)
                {
                    support.Fire();                        // 서포트 총알 발사
                }
            }
        }

        // ───────── 폭탄 사용 ─────────
        if (Input.GetKeyDown(KeyCode.X))                   // X키 눌렀을 때
        {
            if (FindObjectOfType<GameOverUIManager>()?.IsShown == true)
                return;                                   // 게임오버 시 입력 무시

            if (boomPrefab != null && boomCount > 0)      // 폭탄 프리팹 있고 개수 > 0이면
            {
                Instantiate(boomPrefab, boomSpawnPosition, Quaternion.identity); // 폭탄 생성
                boomCount--;                              // 폭탄 1 감소
                boomUIManager?.UpdateBoomUI(boomCount);  // UI 갱신
            }
        }

        // ───────── 폭탄 자동 충전 ─────────
        if (boomCount < maxBoomCount)                     // 최대 개수보다 적으면
        {
            boomRechargeTimer += Time.deltaTime;          // 충전 타이머 증가
            if (boomRechargeTimer >= boomRechargeTime)   // 충전 완료 시
            {
                boomRechargeTimer = 0f;                  // 타이머 초기화
                boomCount++;                             // 폭탄 1 증가
                boomUIManager?.UpdateBoomUI(boomCount); // UI 갱신
            }
        }
    }

    // ───────── 총알 발사 함수 ─────────
    void Fire()
    {
        if (bulletPrefab != null && firePoint != null)   // 총알과 발사 위치가 존재하면
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity); // 총알 생성
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.up * bulletSpeed;      // 위 방향으로 이동

            AudioManager.Instance?.PlayPlayerAttackSFX(); // 공격 사운드 재생
        }
    }

    // ───────── 서포트 등록 ─────────
    public void RegisterSupportShooter(SupportShooter support)
    {
        if (!supportShooters.Contains(support))          // 중복 등록 방지
        {
            supportShooters.Add(support);                // 리스트에 추가
        }
    }
}
