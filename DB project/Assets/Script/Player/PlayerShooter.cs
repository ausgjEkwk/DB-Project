using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float fireRate = 0.25f;

    public GameObject boomPrefab;
    public Vector3 boomSpawnPosition = Vector3.zero;

    private float fireCooldown = 0f;
    private List<SupportShooter> supportShooters = new List<SupportShooter>();

    // === 폭탄 관련 ===
    private int boomCount = 1;
    private int maxBoomCount = 3;
    private float boomRechargeTime = 10f;
    private float boomRechargeTimer = 0f;

    private BoomUIManager boomUIManager;

    void Start()
    {
        boomUIManager = FindObjectOfType<BoomUIManager>();
        if (boomUIManager != null)
        {
            boomUIManager.UpdateBoomUI(boomCount);
        }
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (Input.GetKey(KeyCode.Z) && fireCooldown <= 0f)
        {
            Fire();
            fireCooldown = fireRate;

            foreach (SupportShooter support in supportShooters)
            {
                if (support != null)
                {
                    support.Fire();
                }
            }
        }

        // 폭탄 사용 (X키)
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (FindObjectOfType<GameOverUIManager>()?.IsShown == true)
                return; // 게임오버 시 입력 무시

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

    void Fire()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.up * bulletSpeed;

            // 공격 효과음 재생
            AudioManager.Instance?.PlayPlayerAttackSFX();
        }
    }

    public void RegisterSupportShooter(SupportShooter support)
    {
        if (!supportShooters.Contains(support))
        {
            supportShooters.Add(support);
        }
    }
}
