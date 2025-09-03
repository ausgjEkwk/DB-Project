using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 input;
    private Animator animator;

    [Header("서포트 옵션")]
    public GameObject supportPrefab;
    public Transform supportPosLeft;
    public Transform supportPosRight;
    private List<GameObject> activeSupports = new List<GameObject>();
    private List<Transform> supportPositions = new List<Transform>();
    private int itemCount = 0;

    [Header("이동 제한")]
    public Transform areaLimit; // BoxCollider2D 포함
    private Vector2 minBounds;
    private Vector2 maxBounds;
    private bool areaLimitDisabled = false; // PlayArea 제한 해제 여부

    private Health health; // Health 컴포넌트 참조

    [Header("아이템 흡수")]
    public float itemAttractRadius = 3f;  // 끌어당기기 시작 거리
    public float itemAbsorbSpeed = 5f;    // 이동 속도

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();

        // 서포트 위치 초기화
        if (supportPosLeft != null) supportPositions.Add(supportPosLeft);
        if (supportPosRight != null) supportPositions.Add(supportPosRight);

        // 이동 제한 영역 계산
        if (areaLimit != null)
        {
            BoxCollider2D box = areaLimit.GetComponent<BoxCollider2D>();
            if (box != null)
            {
                Bounds bounds = box.bounds;
                minBounds = bounds.min;
                maxBounds = bounds.max;
            }
            else Debug.LogWarning("Area 오브젝트에 BoxCollider2D가 없습니다.");
        }

        // 체력 UI 초기화
        if (health != null)
        {
            HealthUIManager.Instance?.InitializeHearts(health.maxHealth);
        }
    }

    private void Update()
    {
        // 이동 입력
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        animator.SetBool("isMoving", input.magnitude > 0.01f);

        // 주변 아이템 흡수
        AttractNearbyItems();
    }

    private void FixedUpdate()
    {
        Vector2 targetVelocity = input.normalized * moveSpeed;
        Vector2 newPosition = rb.position + targetVelocity * Time.fixedDeltaTime;

        if (!areaLimitDisabled)
        {
            // 이동 제한 영역 적용
            float clampedX = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
            float clampedY = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);
            rb.MovePosition(new Vector2(clampedX, clampedY));
        }
        else
        {
            rb.MovePosition(newPosition); // 제한 해제 시 자유 이동
        }
    }

    // 아이템 획득 처리
    public void AddItem()
    {
        FindObjectOfType<ScoreManager>()?.AddScore(100);
        itemCount++;

        // 아이템 획득에 따라 서포트 소환
        if (itemCount == 3 && activeSupports.Count == 0) SpawnSupport();
        else if (itemCount == 6 && activeSupports.Count == 1) SpawnSupport();
    }

    private void SpawnSupport()
    {
        if (activeSupports.Count >= supportPositions.Count) return;

        int index = activeSupports.Count;
        Vector3 spawnPos = supportPositions[index].position;

        GameObject support = Instantiate(supportPrefab, spawnPos, Quaternion.identity);
        support.transform.SetParent(transform);

        SupportShooter shooter = support.GetComponent<SupportShooter>();
        if (shooter != null)
        {
            shooter.player = this.transform;
            shooter.followOffset = Vector3.zero;
        }

        activeSupports.Add(support);
    }

    // 주변 아이템 흡수 기능
    private void AttractNearbyItems()
    {
        Collider2D[] items = Physics2D.OverlapCircleAll(transform.position, itemAttractRadius);

        foreach (Collider2D itemCol in items)
        {
            if (itemCol.CompareTag("Item"))
            {
                Transform itemTransform = itemCol.transform;
                itemTransform.position = Vector2.MoveTowards(
                    itemTransform.position,
                    transform.position,
                    itemAbsorbSpeed * Time.deltaTime
                );

                if (Vector2.Distance(transform.position, itemTransform.position) < 0.2f)
                {
                    AddItem();
                    Destroy(itemCol.gameObject);
                }
            }
        }
    }

    // 이동 제한 해제
    public void DisableAreaLimit(bool disable)
    {
        areaLimitDisabled = disable;
        if (!disable && areaLimit != null)
        {
            BoxCollider2D box = areaLimit.GetComponent<BoxCollider2D>();
            if (box != null)
            {
                Bounds bounds = box.bounds;
                minBounds = bounds.min;
                maxBounds = bounds.max;
            }
        }
    }

    // 아이템 흡수 범위 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, itemAttractRadius);
    }
}
