using System.Collections;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Boss Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    public float HealthPercent => (float)currentHealth / maxHealth;

    [Header("UI")]
    public GameObject bossHealthUIPrefab;
    private BossHealthUI healthUIInstance;

    [Header("Death Movement Settings")]
    public float xMoveDuration = 1f;
    public float yRiseDuration = 2f;
    public float yRiseAmount = 4f; // 보스 Y 상승량

    [Header("Player Cutscene Settings")]
    public float playerXMoveDuration = 1f;
    public float playerYMoveDuration = 1.5f;
    public float playerYTarget = 7f;

    private StageClearUIManager stageClearUIManager;
    private bool specialStarted = false;
    private bool isDead = false;
    private BossMovement bossMovement;

    void Start()
    {
        currentHealth = maxHealth;
        bossMovement = GetComponent<BossMovement>();

        // Health UI 생성
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

        // StageClearUIManager 생성/참조
        stageClearUIManager = FindObjectOfType<StageClearUIManager>();
        if (stageClearUIManager == null)
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas != null)
            {
                GameObject uiObj = new GameObject("StageClearUI");
                uiObj.transform.SetParent(canvas.transform, false);
                stageClearUIManager = uiObj.AddComponent<StageClearUIManager>();

                // clearPanel 생성
                GameObject panel = new GameObject("ClearPanel");
                panel.transform.SetParent(uiObj.transform, false);
                RectTransform rt = panel.AddComponent<RectTransform>();
                rt.sizeDelta = new Vector2(400, 200);
                stageClearUIManager.clearPanel = panel;

                // selector 생성
                GameObject selectorObj = new GameObject("Selector");
                selectorObj.transform.SetParent(panel.transform, false);
                RectTransform srt = selectorObj.AddComponent<RectTransform>();
                srt.anchoredPosition = Vector2.zero;
                stageClearUIManager.selector = srt;

                stageClearUIManager.clearPanel.SetActive(false);
            }
            else
            {
                Debug.LogWarning("StageClearUIManager 자동 생성 실패: Canvas 없음");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (healthUIInstance != null)
            healthUIInstance.SetHealthPercent(HealthPercent);

        if (!specialStarted && HealthPercent < 0.5f)
        {
            specialStarted = true;
            BossSpecial bossSpecial = GetComponent<BossSpecial>();
            if (bossSpecial != null)
                bossSpecial.TryStartSpecial(() => { specialStarted = false; });
        }

        if (currentHealth <= 0)
        {
            isDead = true;
            if (bossMovement != null)
            {
                bossMovement.StopAllCoroutines();
                bossMovement.ClearPatternSwords();
            }
            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DeathSequence()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopBGMWithFade();

        // 1. Boss X=0 이동
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

        // 2. Boss Y 상승 (화면 밖)
        Camera cam = Camera.main;
        float camTop = cam.transform.position.y + cam.orthographicSize + 2f; // 화면 위 + 여유

        Vector3 startPosY = transform.position; // X 이동 후 현재 위치 기준
        Vector3 targetPosY = new Vector3(startPosY.x, camTop, startPosY.z);

        elapsed = 0f; // Y 상승용 elapsed 초기화

        while (elapsed < yRiseDuration)
        {
            transform.position = Vector3.Lerp(startPosY, targetPosY, elapsed / yRiseDuration);
            elapsed += Time.deltaTime;

            // 상승 중 화면에 남아있는 BossBullet 제거
            GameObject[] bullets = GameObject.FindGameObjectsWithTag("BossBullet");
            foreach (GameObject b in bullets)
                Destroy(b);

            yield return null;
        }

        transform.position = targetPosY; // 마지막 위치 보정

        Debug.Log("보스가 사망했습니다!");

        // 3. Player 컷씬 (PlayArea 제한 해제)
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
                pc.DisableAreaLimit(true); // 제한 해제
            yield return StartCoroutine(MovePlayerToClearPosition(player));
        }

        // 4. StageClearUI 표시 (Player 화면 밖이면)
        if (stageClearUIManager != null)
        {
            stageClearUIManager.ShowClearUI();
        }

        // 5. Boss 제거
        Die();
    }

    private IEnumerator MovePlayerToClearPosition(GameObject player)
    {
        // X 이동
        Vector3 startPos = player.transform.position;
        Vector3 targetX = new Vector3(0f, startPos.y, startPos.z);
        float elapsed = 0f;
        while (elapsed < playerXMoveDuration)
        {
            player.transform.position = Vector3.Lerp(startPos, targetX, elapsed / playerXMoveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        player.transform.position = targetX;

        // Y 이동 (속도 2배)
        startPos = player.transform.position;
        Vector3 targetY = new Vector3(0f, playerYTarget, startPos.z);
        elapsed = 0f;
        while (elapsed < playerYMoveDuration)
        {
            player.transform.position = Vector3.Lerp(startPos, targetY, elapsed / (playerYMoveDuration / 2f)); // ← 여기 수정
            elapsed += Time.deltaTime;
            yield return null;
        }
        player.transform.position = targetY;
    }

    private void Die()
    {
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
