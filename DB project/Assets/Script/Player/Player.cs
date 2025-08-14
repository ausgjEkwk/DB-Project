using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 input;
    private Animator animator;

    public GameObject supportPrefab;
    public Transform supportPosLeft;
    public Transform supportPosRight;

    private List<GameObject> activeSupports = new List<GameObject>();
    private List<Transform> supportPositions = new List<Transform>();

    private int itemCount = 0;

    // 이동 제한용
    public Transform areaLimit; // Area 오브젝트 (BoxCollider2D 포함)
    private Vector2 minBounds;
    private Vector2 maxBounds;

    private Health health; // Health 컴포넌트 참조

    // ▼ 아이템 흡수 기능 관련 변수
    public float itemAttractRadius = 3f;     // 끌어당기기 시작할 거리
    public float itemAbsorbSpeed = 5f;       // 끌어당기는 속도

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();

        if (supportPosLeft != null) supportPositions.Add(supportPosLeft);
        if (supportPosRight != null) supportPositions.Add(supportPosRight);

        if (areaLimit != null)
        {
            BoxCollider2D box = areaLimit.GetComponent<BoxCollider2D>();
            if (box != null)
            {
                Bounds bounds = box.bounds;
                minBounds = bounds.min;
                maxBounds = bounds.max;
            }
            else
            {
                Debug.LogWarning("Area 오브젝트에 BoxCollider2D가 없습니다.");
            }
        }

        if (health != null)
        {
            HealthUIManager.Instance?.InitializeHearts(health.maxHealth);
        }
    }

    private void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        animator.SetBool("isMoving", input.magnitude > 0.01f);

        AttractNearbyItems(); // 아이템 흡수 실행
    }

    private void FixedUpdate()
    {
        Vector2 targetVelocity = input.normalized * moveSpeed;
        Vector2 newPosition = rb.position + targetVelocity * Time.fixedDeltaTime;

        float clampedX = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

        rb.MovePosition(new Vector2(clampedX, clampedY));
    }

    public void AddItem()
    {
        FindObjectOfType<ScoreManager>()?.AddScore(100);

        itemCount++;

        if (itemCount == 3 && activeSupports.Count == 0)
        {
            SpawnSupport();
        }
        else if (itemCount == 6 && activeSupports.Count == 1)
        {
            SpawnSupport();
        }
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

    
    // ▼ 아이템 흡수 기능
    private void AttractNearbyItems()
    {
        Collider2D[] items = Physics2D.OverlapCircleAll(transform.position, itemAttractRadius);

        foreach (Collider2D itemCol in items)
        {
            if (itemCol.CompareTag("Item")) // Item 프리팹의 태그가 "Item"이어야 함
            {
                Transform itemTransform = itemCol.transform;
                itemTransform.position = Vector2.MoveTowards(
                    itemTransform.position,
                    transform.position,
                    itemAbsorbSpeed * Time.deltaTime
                );

                float distance = Vector2.Distance(transform.position, itemTransform.position);
                if (distance < 0.2f)
                {
                    AddItem();
                    Destroy(itemCol.gameObject);
                }
            }
        }
    }

    // (선택) 아이템 흡수 반경 시각화 Gizmo
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, itemAttractRadius);
    }
}
