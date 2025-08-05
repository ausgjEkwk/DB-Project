using System;
using UnityEngine;

public class EnemyMoveToTarget : MonoBehaviour
{
    public Vector2 targetPosition;
    public float moveSpeed = 2f;
    public GameObject itemPrefab;

    public event Action OnReachedTargetEvent;

    private bool hasReachedTarget = false;
    private bool isDestroyed = false;

    private Health health;

    void Awake()
    {
        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDeath += Die; // Health.cs에 있는 이벤트 연결
        }
    }

    void Update()
    {
        if (hasReachedTarget || isDestroyed) return;

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
        {
            hasReachedTarget = true;
            OnReachedTargetEvent?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return;

        // 몬스터 탄환에 해당하는 태그 목록
        string[] monsterBulletTags = { "Rbullet", "Bbullet", "Ybullet" };

        // 몬스터 탄환이면 피해 무시 (return)
        foreach (var tag in monsterBulletTags)
        {
            if (collision.CompareTag(tag))
            {
                // 몬스터 탄환은 무시
                return;
            }
        }

        // 플레이어 총알 등 그 외 탄환에 대해 처리
        if (collision.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);

            if (health != null)
            {
                health.TakeDamage(1);
            }
            else
            {
                Die();
            }
        }
    }

    public void DestroyByBoom()
    {
        if (isDestroyed) return;

        if (health != null)
        {
            health.TakeDamage(health.currentHealth); // 즉시 체력 0으로
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        if (itemPrefab != null)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
