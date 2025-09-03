using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFacing : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Rigidbody2D가 있으면 속도로 방향 판정
        if (rb != null)
        {
            if (rb.velocity.x > 0.01f)
                spriteRenderer.flipX = false;  // 오른쪽
            else if (rb.velocity.x < -0.01f)
                spriteRenderer.flipX = true;   // 왼쪽
        }
        else
        {
            // 직접 이동시키는 경우라면 Transform 기준으로 판정
            if (Input.GetKey(KeyCode.RightArrow)) // 예: 임시 이동
                spriteRenderer.flipX = true;
            else if (Input.GetKey(KeyCode.LeftArrow))
                spriteRenderer.flipX = false;
        }
    }
}
