using UnityEngine;

public class SwordMover : MonoBehaviour
{
    private Vector3 moveDirection;
    private float moveSpeed;

    private bool canMove = false;

    public void SetDirection(Vector3 direction, float speed)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
    }

    public void PauseMovement()
    {
        canMove = false;
    }

    public void ResumeMovement()
    {
        canMove = true;
    }

    void Update()
    {
        if (canMove)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }
}
