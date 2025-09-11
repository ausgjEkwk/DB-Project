using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossMovement : MonoBehaviour
{
    [Header("Speed Settings")]
    public float moveSpeed = 3f;            // 일반 이동 속도
    public float moveSpeedWithGhost = 1.5f; // 잔상 이동 시 속도

    private Animator animator;              // 보스 Animator

    [Header("Ghost/Flip Settings")]
    public BossFlipController flipController; // 좌우 방향 전환용 컨트롤러
    public GameObject ghostPrefab;            // 잔상 프리팹
    public float ghostSpawnInterval = 0.05f;  // 잔상 생성 주기

    [Header("Boss Patterns")]
    public BossPattern1 pattern1Script;
    public BossPattern2 pattern2Script;
    public BossPattern3 pattern3Script;

    public BossSpecial bossSpecialScript; // BossSpecial 스크립트 연결 필수

    private bool hasMovedDown = false;     // 최초 하강 여부
    private float ghostSpawnTimer = 0f;    // 잔상 생성 타이머
    private bool pattern1Executed = false; // BossSpecial 종료 후 패턴1 실행 여부

    private List<GameObject> patternSwords = new List<GameObject>(); // 현재 패턴에서 소환된 칼 리스트

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator 컴포넌트가 없습니다!");
        if (flipController == null) Debug.LogError("BossFlipController가 할당되지 않았습니다!");
        if (bossSpecialScript == null) Debug.LogError("BossSpecial 스크립트가 연결되지 않았습니다!");
    }

    // 보스 이동 패턴 시작
    public void StartMovePattern()
    {
        StartCoroutine(StartMovePatternRoutine());
    }

    // TimeStop 인스턴스가 존재할 때까지 대기 후 이동 시퀀스 실행
    private IEnumerator StartMovePatternRoutine()
    {
        while (TimeStop.Instance == null || !TimeStop.Instance.gameObject.activeInHierarchy)
            yield return null;

        yield return null;

        yield return StartCoroutine(MoveSequence());
    }

    // 패턴 중 소환되는 칼 등록
    public void RegisterPatternSword(GameObject sword)
    {
        BossHealth bossHealth = GetComponent<BossHealth>();
        if (bossHealth != null && bossHealth.HealthPercent <= 0f)
        {
            Destroy(sword); // 보스 사망 시 칼 제거
            return;
        }

        // BossSpecial 진행 중이거나 일시정지 중이면 칼 제거
        if ((bossSpecialScript != null && bossSpecialScript.IsRunning) ||
            (GamePauseUIManager.Instance != null && GamePauseUIManager.Instance.IsShown))
        {
            Destroy(sword);
            return;
        }

        if (!patternSwords.Contains(sword))
            patternSwords.Add(sword);
    }

    // 현재 등록된 칼 모두 제거
    public void ClearPatternSwords()
    {
        foreach (var sword in patternSwords)
            if (sword != null)
                Destroy(sword);

        patternSwords.Clear();
    }

    // 보스 이동 및 패턴 실행 시퀀스
    IEnumerator MoveSequence()
    {
        // 1. 최초 하강
        if (!hasMovedDown)
        {
            Vector3 targetPos = transform.position + Vector3.down * 3f;
            yield return StartCoroutine(MoveTo(targetPos, moveSpeed));
            hasMovedDown = true;
        }

        while (true)
        {
            animator.Play("Boss_Idle");

            // BossSpecial 또는 ESC 일시정지 상태일 때 대기
            while ((bossSpecialScript != null && bossSpecialScript.IsRunning) ||
                   (GamePauseUIManager.Instance != null && GamePauseUIManager.Instance.IsShown))
            {
                ClearPatternSwords();
                yield return null;
            }

            pattern1Executed = false;

            // 1️ 패턴1 실행
            if (!pattern1Executed)
            {
                // 중앙으로 이동
                while (!Mathf.Approximately(transform.position.x, 0f))
                    yield return StartCoroutine(MoveTo(new Vector3(0f, transform.position.y, 0f), moveSpeed));

                if (pattern1Script != null)
                    yield return StartCoroutine(pattern1Script.StartPattern());

                pattern1Executed = true;
                yield return new WaitForSeconds(1f);
            }

            // 2️ 왼쪽 이동
            while ((bossSpecialScript != null && bossSpecialScript.IsRunning) ||
                   (GamePauseUIManager.Instance != null && GamePauseUIManager.Instance.IsShown))
                yield return null;

            if (flipController != null) flipController.FaceLeft();
            animator.Play("Boss_MoveRight");
            yield return StartCoroutine(MoveTo(new Vector3(-2.5f, transform.position.y, 0f), moveSpeed));
            yield return new WaitForSeconds(1f);

            // 3️ 오른쪽 이동 + 잔상 + 패턴2
            while ((bossSpecialScript != null && bossSpecialScript.IsRunning) ||
                   (GamePauseUIManager.Instance != null && GamePauseUIManager.Instance.IsShown))
                yield return null;

            if (flipController != null) flipController.FaceRight();
            animator.Play("Boss_MoveRight");

            // 시간 정지 시작
            if (TimeStop.Instance != null && !TimeStop.Instance.IsTimeStopped)
                TimeStop.Instance.StartTimeStop();

            if (pattern2Script != null)
                pattern2Script.ResetPattern(transform.position);

            yield return StartCoroutine(MoveToWithPatternUpdate(new Vector3(2.5f, transform.position.y, 0f), moveSpeedWithGhost));

            // 시간 정지 종료 후 칼 발사
            if (TimeStop.Instance != null && TimeStop.Instance.IsTimeStopped)
            {
                TimeStop.Instance.EndTimeStop();
                if (pattern2Script != null)
                    pattern2Script.ShootAllSwords();
            }

            yield return new WaitForSeconds(1f);

            // 4️ 가운데 복귀
            while ((bossSpecialScript != null && bossSpecialScript.IsRunning) ||
                   (GamePauseUIManager.Instance != null && GamePauseUIManager.Instance.IsShown))
                yield return null;

            if (flipController != null) flipController.FaceLeft();
            animator.Play("Boss_MoveRight");
            yield return StartCoroutine(MoveTo(new Vector3(0f, transform.position.y, 0f), moveSpeed));
            animator.Play("Boss_Idle");

            // 5️⃣ 패턴3 실행 (시간 정지 포함)
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

    // 이동 + 패턴 업데이트 코루틴 (잔상 생성 포함)
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

            // 잔상 생성
            ghostSpawnTimer += Time.deltaTime;
            if (ghostSpawnTimer >= ghostSpawnInterval)
            {
                SpawnGhost();
                ghostSpawnTimer = 0f;
            }

            // 패턴2 이동 중 업데이트
            if (pattern2Script != null)
                pattern2Script.UpdatePatternDuringMove();

            yield return null;
        }
        transform.position = target;
    }

    // 단순 이동 코루틴
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

    // 보스 잔상 생성
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

    // BossSpecial 시작
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
