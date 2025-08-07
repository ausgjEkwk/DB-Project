using System.Collections;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public float moveSpeed = 3f;               // 전체 이동 속도
    public float moveSpeedWithGhost = 1.5f;   // 잔상 효과 낼 때만 적용할 속도

    private Animator animator;

    public BossFlipController flipController;

    public GameObject ghostPrefab;             // Inspector에서 연결할 잔상 프리팹
    public float ghostSpawnInterval = 0.05f;  // 잔상 생성 간격

    public BossPattern1 pattern1Script;       // 패턴1 스크립트 참조
    public BossPattern2 pattern2Script;       // 패턴2 스크립트 참조

    private bool isMovingRight = false;       // 오른쪽 이동 상태 체크
    private bool hasRunPattern1 = false;      // 패턴1 실행 여부 체크

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

        // 1. 아래로 내려오기 (기본 속도)
        yield return StartCoroutine(MoveTo(targetPos, moveSpeed));

        // 2. 패턴1은 처음 한번만 실행
        if (!hasRunPattern1 && pattern1Script != null)
        {
            yield return StartCoroutine(pattern1Script.StartPattern());
            hasRunPattern1 = true;
        }

        yield return new WaitForSeconds(1f);

        // 3. 왼쪽으로 이동 (기본 속도)
        if (flipController != null) flipController.FaceLeft();
        animator.Play("Boss_MoveRight");
        isMovingRight = false; // 왼쪽 이동 중이므로 false
        yield return StartCoroutine(MoveTo(new Vector3(-2.5f, targetPos.y, 0f), moveSpeed));

        yield return new WaitForSeconds(1f);

        // 4. 오른쪽 이동 + 시간 정지 + 잔상 효과 (잔상 속도)
        if (flipController != null) flipController.FaceRight();
        animator.Play("Boss_MoveRight");
        isMovingRight = true;  // 오른쪽 이동 시작

        // 시간 정지 시작
        if (TimeStop.Instance != null)
            TimeStop.Instance.StartTimeStop();

        // 패턴2 초기화
        if (pattern2Script != null)
            pattern2Script.ResetPattern(transform.position);

        // 이동 시작 (잔상 효과 속도)
        yield return StartCoroutine(MoveToWithPatternUpdate(new Vector3(2.5f, targetPos.y, 0f), moveSpeedWithGhost));

        // 시간 정지 해제
        if (TimeStop.Instance != null)
        {
            TimeStop.Instance.EndTimeStop();

            // 시간 정지 해제 후 패턴2 칼 일제 발사
            if (pattern2Script != null)
                pattern2Script.ShootAllSwords();
        }

        isMovingRight = false;  // 이동 완료 후 false

        yield return new WaitForSeconds(1f);  // 잠시 대기

        // 5. 가운데로 돌아오기 (기본 속도)
        animator.Play("Boss_MoveRight");  // 돌아올 때도 이동 애니메이션 재생
        if (flipController != null) flipController.FaceLeft();  // 돌아올 땐 왼쪽 방향
        yield return StartCoroutine(MoveTo(new Vector3(0f, targetPos.y, 0f), moveSpeed));

        animator.Play("Boss_Idle");  // 이동 끝나면 Idle 상태로 전환
    }

    // 기존 MoveTo 함수는 그대로 두고, 오른쪽 이동 중 패턴2 업데이트와 잔상 생성을 함께하는 새로운 MoveTo 함수 추가
    IEnumerator MoveToWithPatternUpdate(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            // 오른쪽 이동 중 잔상 생성 타이머
            ghostSpawnTimer += Time.deltaTime;
            if (ghostSpawnTimer >= ghostSpawnInterval)
            {
                SpawnGhost();
                ghostSpawnTimer = 0f;
            }

            // 패턴2 업데이트 (칼 소환)
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
