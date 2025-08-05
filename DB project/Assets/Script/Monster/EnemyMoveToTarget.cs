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
            health.OnDeath += Die; // Health.cs�� �ִ� �̺�Ʈ ����
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

        // ���� źȯ�� �ش��ϴ� �±� ���
        string[] monsterBulletTags = { "Rbullet", "Bbullet", "Ybullet" };

        // ���� źȯ�̸� ���� ���� (return)
        foreach (var tag in monsterBulletTags)
        {
            if (collision.CompareTag(tag))
            {
                // ���� źȯ�� ����
                return;
            }
        }

        // �÷��̾� �Ѿ� �� �� �� źȯ�� ���� ó��
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
            health.TakeDamage(health.currentHealth); // ��� ü�� 0����
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
