using UnityEngine;

public class SupportShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    public Transform player;       // 플레이어 위치 따라다니기용
    public Vector3 followOffset;   // 플레이어 기준 상대 위치

    public float rotationSpeed = 360f;  // 초당 360도 회전

    void Awake()
    {
        if (player == null)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                player = playerController.transform;
            }
        }

        // PlayerShooter에 자기 자신 등록
        PlayerShooter playerShooter = FindObjectOfType<PlayerShooter>();
        if (playerShooter != null)
        {
            playerShooter.RegisterSupportShooter(this);
        }
    }

    void Update()
    {
        if (player != null)
        {
            if (followOffset == Vector3.zero)
            {
                // 위치 고정: Instantiate 시점 위치 유지 (따라가지 않음)
                // 아무것도 안 해도 됨
            }
            else
            {
                // 플레이어 위치 기준 상대 위치로 따라다님
                transform.position = player.position + followOffset;
            }
        }

        // Z축 기준으로 계속 회전 (1초에 1바퀴)
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    public void Fire()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.up * bulletSpeed;
        }
    }
}