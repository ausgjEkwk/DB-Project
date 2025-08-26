using System;
using UnityEngine;

public class EnemyMoveToTarget : MonoBehaviour
{
    public Vector2 targetPosition;     // ��ǥ ��ġ
    public float moveSpeed = 2f;       // �̵� �ӵ�
    public GameObject itemPrefab;      // ��� �� ������ ������ ������

    public event Action OnReachedTargetEvent; // ��ǥ ���� �� �߻��ϴ� �̺�Ʈ

    private bool hasReachedTarget = false;   // ��ǥ ���� ����
    private bool isDestroyed = false;        // �̹� ���ŵǾ����� ����

    private Health health;                   // Health ������Ʈ ����

    void Awake()
    {
        health = GetComponent<Health>();     // Health ������Ʈ ��������
        if (health != null)
        {
            health.OnDeath += Die;           // Health�� ��� �̺�Ʈ�� Die() ����
        }
    }

    void Update()
    {
        if (hasReachedTarget || isDestroyed) return; // �̹� �����߰ų� ���ŵǾ����� �̵����� ����

        // ��ǥ ��ġ�� �̵�
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // ��ǥ ���� ��
        if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
        {
            hasReachedTarget = true;                 // ���� �÷��� ����
            OnReachedTargetEvent?.Invoke();          // ���� �̺�Ʈ ȣ��
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return;

        // ���� źȯ �±� ���
        string[] monsterBulletTags = { "Rbullet", "Bbullet", "Ybullet" };

        // ���� źȯ�̸� ����
        foreach (var tag in monsterBulletTags)
        {
            if (collision.CompareTag(tag))
            {
                return; // ���� �Ѿ� �浹 ����
            }
        }

        // �÷��̾� �Ѿ� �浹 ó��
        if (collision.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);         // �Ѿ� ����

            if (health != null)
            {
                health.TakeDamage(1);              // ü�� 1 ����
            }
            else
            {
                Die();                              // Health ������ ��� ���
            }
        }
    }

    // ��ź(Boom)�� ���� ��� ����
    public void DestroyByBoom()
    {
        if (isDestroyed) return;

        if (health != null)
        {
            health.TakeDamage(health.currentHealth); // ü���� 0���� ����� ��� ó��
        }
        else
        {
            Die();                                   // Health ������ ��� ���
        }
    }

    // ���� ��� ó��
    private void Die()
    {
        if (isDestroyed) return;                    // �̹� ���ŵǾ����� ó�� �� ��
        isDestroyed = true;

        // ������ ����
        if (itemPrefab != null)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);                        // ���� ������Ʈ ����
    }
}
