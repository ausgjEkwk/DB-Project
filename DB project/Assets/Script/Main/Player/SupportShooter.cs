using UnityEngine;

public class SupportShooter : MonoBehaviour
{
    public GameObject bulletPrefab;       // 서포트 총알 프리팹
    public Transform firePoint;           // 총알 발사 위치
    public float bulletSpeed = 10f;       // 총알 속도

    public Transform player;              // 플레이어 위치 따라다니기용
    public Vector3 followOffset;          // 플레이어 기준 상대 위치

    public float rotationSpeed = 360f;    // 초당 360도 회전 (1초에 1바퀴)

    void Awake()
    {
        if (player == null)               // 플레이어가 할당되어 있지 않으면
        {
            PlayerController playerController = FindObjectOfType<PlayerController>(); // 씬에서 PlayerController 검색
            if (playerController != null)
            {
                player = playerController.transform; // 플레이어 Transform 할당
            }
        }

        // PlayerShooter에 자기 자신 등록
        PlayerShooter playerShooter = FindObjectOfType<PlayerShooter>();
        if (playerShooter != null)
        {
            playerShooter.RegisterSupportShooter(this); // 총알 발사 시 서포트가 함께 발사되도록 등록
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
        if (bulletPrefab != null && firePoint != null)   // 총알과 발사 위치가 존재하면
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity); // 총알 생성
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.up * bulletSpeed;       // 위 방향으로 발사
        }
    }
}
