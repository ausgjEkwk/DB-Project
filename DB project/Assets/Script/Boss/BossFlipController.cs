using UnityEngine;

public class BossFlipController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            Debug.LogError("SpriteRenderer�� �����ϴ�.");
        if (animator == null)
            Debug.LogError("Animator�� �����ϴ�.");
    }

    public void FaceRight()
    {
        spriteRenderer.flipX = false;
    }

    public void FaceLeft()
    {
        spriteRenderer.flipX = true;
    }
}
