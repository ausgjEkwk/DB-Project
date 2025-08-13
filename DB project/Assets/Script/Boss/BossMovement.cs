using System.Collections;
using UnityEngine;

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

    private bool hasMovedDown = false;    // 최초 1회만 내려왔는지 체크
    private float ghostSpawnTimer = 0f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("Animator 컴포넌트가 없습니다!");
        if (flipController == null)
            Debug.LogError("BossFlipController가 할당되지 않았습니다!");
        if (pattern1Script == null)
            Debug.LogError("BossPattern1 스크립트가 연결되지 않았습니다!");
        if (pattern2Script == null)
            Debug.LogError("BossPattern2 스크립트가 연결되지 않았습니다!");
        if (pattern3Script == null)
            Debug.LogError("BossPattern3 스크립트가 연결되지 않았습니다!");
        if (ghostPrefab == null)
            Debug.LogWarning("ghostPrefab이 할당되지 않았습니다!");
    }

    public void StartMovePattern()
    {
        StartCoroutine(MoveSequence());
    }

    // 🔹 패턴1 → 왼쪽 이동 → 패턴2 → 가운데 복귀 → 패턴3 통합
    IEnumerator MoveSequence()
    {
        // 등장 시 최초 1회만 아래로 이동
        if (!hasMovedDown)
        {
            Vector3 targetPos = transform.position + Vector3.down * 3f;
            yield return StartCoroutine(MoveTo(targetPos, moveSpeed));
            hasMovedDown = true;
        }

        while (true)
        {
            animator.Play("Boss_Idle");

            // ───────────────────────────────
            // 🔹 통합 패턴 시작
            // ───────────────────────────────

            // 1️⃣ 패턴1 실행
            if (pattern1Script != null)
                yield return StartCoroutine(pattern1Script.StartPattern());

            yield return new WaitForSeconds(1f);

            // 2️⃣ 왼쪽 이동
            if (flipController != null) flipController.FaceLeft();
            animator.Play("Boss_MoveRight");
            yield return StartCoroutine(MoveTo(new Vector3(-2.5f, transform.position.y, 0f), moveSpeed));
            yield return new WaitForSeconds(1f);

            // 3️⃣ 오른쪽 이동 + 잔상 + 패턴2 업데이트
            if (flipController != null) flipController.FaceRight();
            animator.Play("Boss_MoveRight");

            if (TimeStop.Instance != null)
                TimeStop.Instance.StartTimeStop();

            if (pattern2Script != null)
                pattern2Script.ResetPattern(transform.position);

            yield return StartCoroutine(MoveToWithPatternUpdate(new Vector3(2.5f, transform.position.y, 0f), moveSpeedWithGhost));

            if (TimeStop.Instance != null)
            {
                TimeStop.Instance.EndTimeStop();

                if (pattern2Script != null)
                    pattern2Script.ShootAllSwords();
            }

            yield return new WaitForSeconds(1f);

            // 4️⃣ 가운데 복귀
            if (flipController != null) flipController.FaceLeft();
            animator.Play("Boss_MoveRight");
            yield return StartCoroutine(MoveTo(new Vector3(0f, transform.position.y, 0f), moveSpeed));
            animator.Play("Boss_Idle");

            // 5️⃣ 패턴3 실행
            if (pattern3Script != null && TimeStop.Instance != null)
            {
                TimeStop.Instance.StartTimeStop();
                yield return StartCoroutine(pattern3Script.ExecutePattern());
                TimeStop.Instance.EndTimeStop();
            }
        }
    }

    // 잔상 생성 + 패턴2 업데이트 이동
    IEnumerator MoveToWithPatternUpdate(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
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

    // 단순 이동
    IEnumerator MoveTo(Vector3 target, float speed)
    {
        Vector3 start = transform.position;
        float distance = Vector3.Distance(start, target);
        float elapsed = 0f;
        float travelTime = distance / speed;

        while (elapsed < travelTime)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / travelTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }

    // 잔상 생성
    void SpawnGhost()
    {
        if (ghostPrefab == null) return;

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
}
