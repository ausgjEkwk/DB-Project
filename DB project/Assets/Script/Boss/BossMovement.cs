using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float moveSpeedWithGhost = 1.5f;

    private Animator animator;

    public BossFlipController flipController;
    public GameObject ghostPrefab;
    public float ghostSpawnInterval = 0.05f;

    public BossPattern1 pattern1Script;
    public BossPattern2 pattern2Script;
    public BossPattern3 pattern3Script;

    public BossSpecial bossSpecialScript; // 연결 필수

    private bool hasMovedDown = false;
    private float ghostSpawnTimer = 0f;
    private bool pattern1Executed = false; // BossSpecial 종료 후 패턴1 실행 여부

    private List<GameObject> patternSwords = new List<GameObject>();

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator 컴포넌트가 없습니다!");
        if (flipController == null) Debug.LogError("BossFlipController가 할당되지 않았습니다!");
        if (bossSpecialScript == null) Debug.LogError("BossSpecial 스크립트가 연결되지 않았습니다!");
    }

    public void StartMovePattern()
    {
        StartCoroutine(StartMovePatternRoutine());
    }

    private IEnumerator StartMovePatternRoutine()
    {
        while (TimeStop.Instance == null || !TimeStop.Instance.gameObject.activeInHierarchy)
            yield return null;

        yield return null;

        yield return StartCoroutine(MoveSequence());
    }

    public void RegisterPatternSword(GameObject sword)
    {
        BossHealth bossHealth = GetComponent<BossHealth>();
        if (bossHealth != null && bossHealth.HealthPercent <= 0f)
        {
            Destroy(sword);
            return;
        }

        if ((bossSpecialScript != null && bossSpecialScript.IsRunning) ||
            (GamePauseUIManager.Instance != null && GamePauseUIManager.Instance.IsShown))
        {
            Destroy(sword);
            return;
        }

        if (!patternSwords.Contains(sword))
            patternSwords.Add(sword);
    }

    public void ClearPatternSwords()
    {
        foreach (var sword in patternSwords)
            if (sword != null)
                Destroy(sword);

        patternSwords.Clear();
    }

    IEnumerator MoveSequence()
    {
        if (!hasMovedDown)
        {
            Vector3 targetPos = transform.position + Vector3.down * 3f;
            yield return StartCoroutine(MoveTo(targetPos, moveSpeed));
            hasMovedDown = true;
        }

        while (true)
        {
            animator.Play("Boss_Idle");

            // BossSpecial or ESC 일시정지 대기
            while ((bossSpecialScript != null && bossSpecialScript.IsRunning) ||
                   (GamePauseUIManager.Instance != null && GamePauseUIManager.Instance.IsShown))
            {
                ClearPatternSwords();
                yield return null;
            }

            pattern1Executed = false;

            // 1️⃣ 패턴1
            if (!pattern1Executed)
            {
                while (!Mathf.Approximately(transform.position.x, 0f))
                    yield return StartCoroutine(MoveTo(new Vector3(0f, transform.position.y, 0f), moveSpeed));

                if (pattern1Script != null)
                    yield return StartCoroutine(pattern1Script.StartPattern());

                pattern1Executed = true;
                yield return new WaitForSeconds(1f);
            }

            // 2️⃣ 왼쪽 이동
            while ((bossSpecialScript != null && bossSpecialScript.IsRunning) ||
                   (GamePauseUIManager.Instance != null && GamePauseUIManager.Instance.IsShown))
                yield return null;

            if (flipController != null) flipController.FaceLeft();
            animator.Play("Boss_MoveRight");
            yield return StartCoroutine(MoveTo(new Vector3(-2.5f, transform.position.y, 0f), moveSpeed));
            yield return new WaitForSeconds(1f);

            // 3️⃣ 오른쪽 이동 + 잔상 + 패턴2
            while ((bossSpecialScript != null && bossSpecialScript.IsRunning) ||
                   (GamePauseUIManager.Instance != null && GamePauseUIManager.Instance.IsShown))
                yield return null;

            if (flipController != null) flipController.FaceRight();
            animator.Play("Boss_MoveRight");

            if (TimeStop.Instance != null && !TimeStop.Instance.IsTimeStopped)
                TimeStop.Instance.StartTimeStop();

            if (pattern2Script != null)
                pattern2Script.ResetPattern(transform.position);

            yield return StartCoroutine(MoveToWithPatternUpdate(new Vector3(2.5f, transform.position.y, 0f), moveSpeedWithGhost));

            if (TimeStop.Instance != null && TimeStop.Instance.IsTimeStopped)
            {
                TimeStop.Instance.EndTimeStop();
                if (pattern2Script != null)
                    pattern2Script.ShootAllSwords();
            }

            yield return new WaitForSeconds(1f);

            // 4️⃣ 가운데 복귀
            while ((bossSpecialScript != null && bossSpecialScript.IsRunning) ||
                   (GamePauseUIManager.Instance != null && GamePauseUIManager.Instance.IsShown))
                yield return null;

            if (flipController != null) flipController.FaceLeft();
            animator.Play("Boss_MoveRight");
            yield return StartCoroutine(MoveTo(new Vector3(0f, transform.position.y, 0f), moveSpeed));
            animator.Play("Boss_Idle");

            // 5️⃣ 패턴3 실행
            while ((bossSpecialScript != null && bossSpecialScript.IsRunning) ||
                   (GamePauseUIManager.Instance != null && GamePauseUIManager.Instance.IsShown))
                yield return null;

            if (pattern3Script != null && TimeStop.Instance != null)
            {
                if (!TimeStop.Instance.IsTimeStopped)
                    TimeStop.Instance.StartTimeStop();

                yield return StartCoroutine(pattern3Script.ExecutePattern());

                if (TimeStop.Instance.IsTimeStopped)
                    TimeStop.Instance.EndTimeStop();
            }

            pattern1Executed = false;
            yield return null;
        }
    }

    IEnumerator MoveToWithPatternUpdate(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            if ((bossSpecialScript != null && bossSpecialScript.IsRunning) ||
                (GamePauseUIManager.Instance != null && GamePauseUIManager.Instance.IsShown))
            {
                ClearPatternSwords();
                yield return null;
                continue;
            }

            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            ghostSpawnTimer += Time.deltaTime;
            if (ghostSpawnTimer >= ghostSpawnInterval)
            {
                SpawnGhost();
                ghostSpawnTimer = 0f;
            }

            if (pattern2Script != null)
                pattern2Script.UpdatePatternDuringMove();

            yield return null;
        }
        transform.position = target;
    }

    IEnumerator MoveTo(Vector3 target, float speed)
    {
        Vector3 start = transform.position;
        float distance = Vector3.Distance(start, target);
        float elapsed = 0f;
        float travelTime = distance / speed;

        while (elapsed < travelTime)
        {
            if ((bossSpecialScript != null && bossSpecialScript.IsRunning) ||
                (GamePauseUIManager.Instance != null && GamePauseUIManager.Instance.IsShown))
            {
                ClearPatternSwords();
                yield return null;
                continue;
            }

            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, elapsed / travelTime);
            yield return null;
        }

        transform.position = target;
    }

    void SpawnGhost()
    {
        if (ghostPrefab == null ||
            (bossSpecialScript != null && bossSpecialScript.IsRunning) ||
            (GamePauseUIManager.Instance != null && GamePauseUIManager.Instance.IsShown))
            return;

        GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
        SpriteRenderer ghostSR = ghost.GetComponent<SpriteRenderer>();
        SpriteRenderer bossSR = GetComponent<SpriteRenderer>();

        if (ghostSR != null && bossSR != null)
        {
            ghostSR.sprite = bossSR.sprite;
            ghostSR.flipX = bossSR.flipX;
            ghostSR.color = new Color(1f, 1f, 1f, 0.5f);
            ghostSR.sortingLayerID = bossSR.sortingLayerID;
            ghostSR.sortingOrder = bossSR.sortingOrder - 1;
        }
    }

    public void StartBossSpecial()
    {
        if (bossSpecialScript != null)
        {
            bossSpecialScript.TryStartSpecial(() =>
            {
                // 필요 시 종료 콜백
            });
        }
    }
}
