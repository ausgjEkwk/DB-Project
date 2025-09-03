using System.Collections;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Boss Settings")]
    public int maxHealth = 100;                // 보스 최대 체력
    private int currentHealth;                 // 현재 체력
    public float HealthPercent => (float)currentHealth / maxHealth; // 체력 비율 계산

    [Header("UI")]
    public GameObject bossHealthUIPrefab;      // 체력바 UI 프리팹
    private BossHealthUI healthUIInstance;     // 체력 UI 인스턴스

    [Header("Death Movement Settings")]
    public float xMoveDuration = 1f;           // 사망 시 X축 이동 시간
    public float yRiseDuration = 2f;           // 사망 시 Y축 상승 시간
    public float yRiseAmount = 4f;             // Y축 상승량 (보스가 화면 밖으로 올라갈 때)

    [Header("Player Cutscene Settings")]
    public float playerXMoveDuration = 1f;     // 플레이어 컷씬 X 이동 시간
    public float playerYMoveDuration = 1.5f;   // 플레이어 컷씬 Y 이동 시간
    public float playerYTarget = 7f;           // 플레이어 Y 이동 목표 위치

    private StageClearUIManager stageClearUIManager; // 스테이지 클리어 UI 관리
    private bool specialStarted = false;       // 보스 특수 패턴 시작 여부
    private bool isDead = false;               // 보스 사망 여부
    private BossMovement bossMovement;         // 보스 이동/패턴 관리

    void Start()
    {
        currentHealth = maxHealth;             // 초기 체력 설정
        bossMovement = GetComponent<BossMovement>();

        // Health UI 생성
        if (bossHealthUIPrefab != null)
        {
            GameObject uiObj = Instantiate(bossHealthUIPrefab);
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas != null)
                uiObj.transform.SetParent(canvas.transform, false); // Canvas 하위에 배치
            else
                Debug.LogWarning("씬에 Canvas 오브젝트가 없습니다!");
            healthUIInstance = uiObj.GetComponent<BossHealthUI>();
        }

        // 체력 UI 초기값 100%
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

                // selector 생성 (화살표 UI)
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

    // 보스 체력 감소 처리
    public void TakeDamage(int damage)
    {
        if (isDead) return;                     // 이미 사망 시 무시

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        // UI 업데이트
        if (healthUIInstance != null)
            healthUIInstance.SetHealthPercent(HealthPercent);

        // 체력 50% 이하 → 특수 패턴 시작
        if (!specialStarted && HealthPercent < 0.5f)
        {
            specialStarted = true;
            BossSpecial bossSpecial = GetComponent<BossSpecial>();
            if (bossSpecial != null)
                bossSpecial.TryStartSpecial(() => { specialStarted = false; });
        }

        // 체력 0 → 사망 처리
        if (currentHealth <= 0)
        {
            isDead = true;

            // 보스 사망 시점에서 즉시 모든 BossBullet 제거
            GameObject[] bullets = GameObject.FindGameObjectsWithTag("BossBullet");
            foreach (GameObject b in bullets)
                Destroy(b);

            // 보스 패턴 정지
            if (bossMovement != null)
            {
                bossMovement.StopAllCoroutines();
                bossMovement.ClearPatternSwords();
            }

            // 사망 시퀀스 시작
            StartCoroutine(DeathSequence());
        }
    }

    // 보스 사망 시 연출
    private IEnumerator DeathSequence()
    {
        // BGM 페이드 아웃
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopBGMWithFade();

        // 1. X축 중앙으로 이동
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

        // 2. Y축 상승 (화면 밖으로)
        Camera cam = Camera.main;
        float camTop = cam.transform.position.y + cam.orthographicSize + 2f; // 화면 위 + 여유

        Vector3 startPosY = transform.position;
        Vector3 targetPosY = new Vector3(startPosY.x, camTop, startPosY.z);

        elapsed = 0f;
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
        transform.position = targetPosY;

        Debug.Log("보스가 사망했습니다!");

        // 3. 플레이어 컷씬
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
                pc.DisableAreaLimit(true); // 플레이어 이동 제한 해제
            yield return StartCoroutine(MovePlayerToClearPosition(player));
        }

        // 4. StageClearUI 표시
        if (stageClearUIManager != null)
        {
            stageClearUIManager.ShowClearUI();
        }

        // 5. 보스 제거
        Die();
    }

    // 플레이어 컷씬 이동
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
            player.transform.position = Vector3.Lerp(startPos, targetY, elapsed / (playerYMoveDuration / 2f)); // 속도 2배
            elapsed += Time.deltaTime;
            yield return null;
        }
        player.transform.position = targetY;
    }

    // 보스 오브젝트 삭제
    private void Die()
    {
        Destroy(gameObject);
        if (healthUIInstance != null)
            Destroy(healthUIInstance.gameObject);
    }

    // 플레이어 총알 충돌 처리
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            TakeDamage(1);           // 데미지 1
            Destroy(collision.gameObject); // 총알 제거
        }
    }
}
