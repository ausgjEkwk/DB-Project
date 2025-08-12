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
    public BossPattern3 pattern3Script;  // 패턴3 스크립트 추가

    private bool hasRunPattern1 = false;
    private bool hasRunPattern3 = false;  // 패턴3 실행 여부 체크
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

    IEnumerator MoveSequence()
    {
        animator.Play("Boss_Idle");

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.down * 3f;

        if (!hasMovedDown)
        {
            yield return StartCoroutine(MoveTo(targetPos, moveSpeed));
            hasMovedDown = true;
        }
        else
        {
            // 이미 내려왔으면 targetPos는 현재 위치로 (이동 없음)
            targetPos = transform.position;
        }

        // 1. 패턴1 (처음만 실행)
        if (!hasRunPattern1 && pattern1Script != null)
        {
            yield return StartCoroutine(pattern1Script.StartPattern());
            hasRunPattern1 = true;
        }

        yield return new WaitForSeconds(1f);

        // 2. 왼쪽으로 이동
        if (flipController != null) flipController.FaceLeft();
        animator.Play("Boss_MoveRight");
        yield return StartCoroutine(MoveTo(new Vector3(-2.5f, targetPos.y, 0f), moveSpeed));

        yield return new WaitForSeconds(1f);

        // 3. 오른쪽 이동 + 잔상 + 시간 정지 + 패턴2 업데이트
        if (flipController != null) flipController.FaceRight();
        animator.Play("Boss_MoveRight");

        if (TimeStop.Instance != null)
            TimeStop.Instance.StartTimeStop();

        if (pattern2Script != null)
            pattern2Script.ResetPattern(transform.position);

        yield return StartCoroutine(MoveToWithPatternUpdate(new Vector3(2.5f, targetPos.y, 0f), moveSpeedWithGhost));

        if (TimeStop.Instance != null)
        {
            TimeStop.Instance.EndTimeStop();

            if (pattern2Script != null)
                pattern2Script.ShootAllSwords();
        }

        yield return new WaitForSeconds(1f);

        // 4. 가운데 복귀
        animator.Play("Boss_MoveRight");
        if (flipController != null) flipController.FaceLeft();
        yield return StartCoroutine(MoveTo(new Vector3(0f, targetPos.y, 0f), moveSpeed));

        animator.Play("Boss_Idle");

        // 5. 패턴3 실행
        if (!hasRunPattern3 && pattern3Script != null && TimeStop.Instance != null)
        {
            TimeStop.Instance.StartTimeStop();

            yield return StartCoroutine(pattern3Script.ExecutePattern());

            TimeStop.Instance.EndTimeStop();

            hasRunPattern3 = true;
        }

        // 6. 2초 대기
        yield return new WaitForSeconds(2f);

        // 7. 상태 초기화 및 반복 (내려오지 않음)
        hasRunPattern1 = false;
        hasRunPattern3 = false;

        StartCoroutine(MoveSequence());
    }

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