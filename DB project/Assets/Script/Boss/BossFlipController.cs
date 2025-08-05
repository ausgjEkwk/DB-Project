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
            Debug.LogError("SpriteRenderer가 없습니다.");
        if (animator == null)
            Debug.LogError("Animator가 없습니다.");
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
