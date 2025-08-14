// BossHealth.cs
using System.Collections;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public float HealthPercent => (float)currentHealth / maxHealth;

    public GameObject bossHealthUIPrefab;

    private BossHealthUI healthUIInstance;

    private bool specialStarted = false;

    private BossMovement bossMovement; // BossMovement 참조

    [Header("Death Movement Settings")]
    public float xMoveDuration = 1f;    // X축 이동 시간
    public float yRiseDuration = 1f;    // Y축 상승 시간
    public float yRiseAmount = 3f;      // 상승 거리

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        bossMovement = GetComponent<BossMovement>();

        if (bossHealthUIPrefab != null)
        {
            GameObject uiObj = Instantiate(bossHealthUIPrefab);
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas != null)
                uiObj.transform.SetParent(canvas.transform, false);
            else
                Debug.LogWarning("씬에 Canvas 오브젝트가 없습니다!");

            healthUIInstance = uiObj.GetComponent<BossHealthUI>();
        }

        if (healthUIInstance != null)
            healthUIInstance.SetHealthPercent(1f);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (healthUIInstance != null)
            healthUIInstance.SetHealthPercent(HealthPercent);

        // 특수패턴 자동 시작
        if (!specialStarted && HealthPercent < 0.5f)
        {
            specialStarted = true;

            BossSpecial bossSpecial = GetComponent<BossSpecial>();
            if (bossSpecial != null)
            {
                bossSpecial.TryStartSpecial(() =>
                {
                    specialStarted = false;
                });
            }
        }

        // 체력 0이면 사망 처리
        if (currentHealth <= 0)
        {
            isDead = true;

            if (bossMovement != null)
            {
                bossMovement.StopAllCoroutines(); // 공격 루프 종료
                bossMovement.ClearPatternSwords(); // 모든 패턴 칼 제거
            }

            StartCoroutine(DeathSequence());
        }
    }

    IEnumerator DeathSequence()
    {
        // 1️⃣ X축 0으로 이동
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(0f, startPos.y, startPos.z);
        float elapsed = 0f;

        while (elapsed < xMoveDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / xMoveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;

        // 2️⃣ Y축 상승
        startPos = transform.position;
        targetPos = startPos + Vector3.up * yRiseAmount;
        elapsed = 0f;

        while (elapsed < yRiseDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / yRiseDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

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
