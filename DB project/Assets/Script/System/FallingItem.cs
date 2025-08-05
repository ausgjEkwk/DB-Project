using UnityEngine;

public class FallingItem : MonoBehaviour
{
    public float fallSpeed = 2f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 시간 정지 중이면 움직이지 않음
        if (TimeStop.IsStopped)
            return;

        // 아래로 이동
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // 화면 아래로 벗어나면 삭제
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
        if (viewPos.y < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Heal(1);
            }
            Destroy(gameObject);
        }
    }
}
