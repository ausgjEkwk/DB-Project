using UnityEngine;

public class BossFacing : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 lastPosition;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 movement = transform.position - lastPosition;

        if (movement.x > 0.01f)
            spriteRenderer.flipX = false; // 오른쪽 이동
        else if (movement.x < -0.01f)
            spriteRenderer.flipX = true;  // 왼쪽 이동

        lastPosition = transform.position;
    }
}
