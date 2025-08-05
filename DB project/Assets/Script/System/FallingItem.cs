using System.Collections;
using System.Collections.Generic;
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
        // 아래로 이동
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // 화면 아래로 벗어나면 제거
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
                playerHealth.Heal(1);  // 최대체력+1, 현재체력+1 및 UI 자동 반영
            }
            Destroy(gameObject);
        }
    }
}