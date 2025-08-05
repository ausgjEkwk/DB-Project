using System.Collections;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public float moveSpeed = 3f;               // 전체 이동 속도
    public float moveSpeedWithGhost = 1.5f;   // 잔상 효과 낼 때만 적용할 속도

    private float spawnX;
    private float spawnY;

    private Animator animator;

    public BossFlipController flipController;

    public GameObject ghostPrefab;             // Inspector에서 연결할 잔상 프리팹
    public float ghostSpawnInterval = 0.05f;  // 잔상 생성 간격

    public BossPattern1 pattern1Script;       // 공격 패턴 스크립트 참조

    private bool isMovingRight = false;       // 오른쪽 이동 상태 체크

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("Animator 컴포넌트가 없습니다!");
        if (flipController == null)
            Debug.LogError("BossFlipController가 할당되지 않았습니다!");
        if (pattern1Script == null)
            Debug.LogError("BossPattern1 스크립트가 연결되지 않았습니다!");
        if (ghostPrefab == null)
            Debug.LogWarning("ghostPrefab이 할당되지 않았습니다!");
    }

    public void SetSpawnPosition(Vector3 spawnPos)
    {
        spawnX = spawnPos.x;
        spawnY = spawnPos.y;
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

        // 아래로 내려오기 (기본 속도)
        yield return StartCoroutine(MoveTo(targetPos, moveSpeed));

        // 내려온 뒤 공격 패턴 실행
        if (pattern1Script != null)
        {
            yield return StartCoroutine(pattern1Script.StartPattern());
        }

        yield return new WaitForSeconds(1f);

        // 왼쪽 이동 (기본 속도)
        if (flipController != null) flipController.FaceLeft();
        animator.Play("Boss_MoveRight");
        isMovingRight = false; // 왼쪽 이동 중이므로 false
        yield return StartCoroutine(MoveTo(new Vector3(-2.5f, targetPos.y, 0f), moveSpeed));

        yield return new WaitForSeconds(1f);

        // 오른쪽 이동 + 시간 정지 + 잔상 효과 (잔상 속도)
        if (flipController != null) flipController.FaceRight();
        animator.Play("Boss_MoveRight");
        isMovingRight = true;  // 오른쪽 이동 시작

        // 시간 정지 시작
        if (TimeStop.Instance != null)
            TimeStop.Instance.StartTimeStop();

        // 잔상 생성 코루틴 시작 (0.5초 동안)
        Coroutine ghostCoroutine = StartCoroutine(CreateGhostTrail(0.5f));

        // 이동 시작 (잔상 효과 속도)
        yield return StartCoroutine(MoveTo(new Vector3(2.5f, targetPos.y, 0f), moveSpeedWithGhost));

        // 잔상 생성 중지 (만약 아직 끝나지 않았다면)
        if (ghostCoroutine != null)
            StopCoroutine(ghostCoroutine);

        // 시간 정지 해제
        if (TimeStop.Instance != null)
            TimeStop.Instance.EndTimeStop();

        isMovingRight = false;  // 이동 완료 후 false

        yield return new WaitForSeconds(1f);  // 잠시 대기

        // 가운데로 돌아오기 (기본 속도)
        animator.Play("Boss_MoveRight");  // 돌아올 때도 이동 애니메이션 재생
        if (flipController != null) flipController.FaceLeft();  // 돌아올 땐 왼쪽 방향
        yield return StartCoroutine(MoveTo(new Vector3(0f, targetPos.y, 0f), moveSpeed));

        animator.Play("Boss_Idle");  // 이동 끝나면 Idle 상태로 전환
    }

    // MoveTo 함수가 속도 인자를 받도록 변경
    IEnumerator MoveTo(Vector3 target, float speed)
    {
        Vector3 start = transform.position;
        float distance = Vector3.Distance(start, target);
        float elapsed = 0f;
        float travelTime = distance / speed;

        while (elapsed < travelTime)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / travelTime);

            // 오른쪽 이동 중일 때만 잔상 생성
            if (isMovingRight)
            {
                SpawnGhost();
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }

    IEnumerator CreateGhostTrail(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            SpawnGhost();
            yield return new WaitForSeconds(ghostSpawnInterval);
            elapsed += ghostSpawnInterval;
        }
    }

    void SpawnGhost()
    {
        if (ghostPrefab == null)
            return;

        GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
        SpriteRenderer ghostSR = ghost.GetComponent<SpriteRenderer>();
        SpriteRenderer bossSR = GetComponent<SpriteRenderer>();

        if (ghostSR != null && bossSR != null)
        {
            ghostSR.sprite = bossSR.sprite;
            ghostSR.flipX = bossSR.flipX;
            ghostSR.color = new Color(1f, 1f, 1f, 0.5f); // 투명도 조절
            ghostSR.sortingLayerID = bossSR.sortingLayerID;
            ghostSR.sortingOrder = bossSR.sortingOrder - 1;
        }
    }
}
