using UnityEngine;

public class SwordMover : MonoBehaviour
{
    private Vector3 moveDirection;   // 칼 이동 방향
    private float moveSpeed;         // 칼 이동 속도

    private bool canMove = false;    // 이동 가능 여부

    // 칼 방향과 속도 설정
    public void SetDirection(Vector3 direction, float speed)
    {
        moveDirection = direction.normalized; // 방향 정규화
        moveSpeed = speed;                     // 속도 저장
    }

    // 칼 이동 일시정지
    public void PauseMovement()
    {
        canMove = false;
    }

    // 칼 이동 재개
    public void ResumeMovement()
    {
        canMove = true;
    }

    void Update()
    {
        if (canMove) // 이동 가능 시
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime; // 이동 적용
        }
    }
}
