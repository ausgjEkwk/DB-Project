using UnityEngine;

public class FallingItem : MonoBehaviour
{
    public float fallSpeed = 2f; // 아이템 낙하 속도
    private Camera mainCamera;    // 메인 카메라 참조

    void Start()
    {
        mainCamera = Camera.main; // 씬에서 메인 카메라 가져오기
    }

    void Update()
    {
        // 시간 정지 중이면 아이템 움직이지 않음
        if (TimeStop.IsStopped)
            return;

        // 아이템을 아래로 이동
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // 화면 좌표로 변환
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

        // 화면 아래로 벗어나면 삭제
        if (viewPos.y < 0)
        {
            Destroy(gameObject);
        }
    }

    // 플레이어와 충돌했을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어 체력 회복
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Heal(1);
            }
            // 아이템 삭제
            Destroy(gameObject);
        }
    }
}
