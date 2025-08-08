using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public GameObject bossHealthUIPrefab;  // Inspector���� UI ������ ����

    private BossHealthUI healthUIInstance;

    void Start()
    {
        currentHealth = maxHealth;

        if (bossHealthUIPrefab != null)
        {
            GameObject uiObj = Instantiate(bossHealthUIPrefab);

            // ���� Canvas ã�Ƽ� UI�� �� ������ �ֱ� (�� ������ ȭ�鿡 �� ����)
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas != null)
            {
                uiObj.transform.SetParent(canvas.transform, false);
            }
            else
            {
                Debug.LogWarning("���� Canvas ������Ʈ�� �����ϴ�!");
            }

            healthUIInstance = uiObj.GetComponent<BossHealthUI>();
        }

        if (healthUIInstance != null)
            healthUIInstance.SetHealthPercent(1f);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (healthUIInstance != null)
            healthUIInstance.SetHealthPercent((float)currentHealth / maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("������ ����߽��ϴ�!");
        Destroy(gameObject);

        if (healthUIInstance != null)
            Destroy(healthUIInstance.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
    }
}
