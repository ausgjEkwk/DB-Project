using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public GameObject bossHealthUIPrefab;  // Inspector에서 UI 프리팹 연결

    private BossHealthUI healthUIInstance;

    void Start()
    {
        currentHealth = maxHealth;

        if (bossHealthUIPrefab != null)
        {
            GameObject uiObj = Instantiate(bossHealthUIPrefab);

            // 씬의 Canvas 찾아서 UI를 그 하위로 넣기 (안 넣으면 화면에 안 보임)
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas != null)
            {
                uiObj.transform.SetParent(canvas.transform, false);
            }
            else
            {
                Debug.LogWarning("씬에 Canvas 오브젝트가 없습니다!");
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
        Debug.Log("보스가 사망했습니다!");
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
