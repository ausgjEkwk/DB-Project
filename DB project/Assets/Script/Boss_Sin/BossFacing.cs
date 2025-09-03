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
        // Rigidbody2D�� ������ �ӵ��� ���� ����
        if (rb != null)
        {
            if (rb.velocity.x > 0.01f)
                spriteRenderer.flipX = false;  // ������
            else if (rb.velocity.x < -0.01f)
                spriteRenderer.flipX = true;   // ����
        }
        else
        {
            // ���� �̵���Ű�� ����� Transform �������� ����
            if (Input.GetKey(KeyCode.RightArrow)) // ��: �ӽ� �̵�
                spriteRenderer.flipX = true;
            else if (Input.GetKey(KeyCode.LeftArrow))
                spriteRenderer.flipX = false;
        }
    }
}
