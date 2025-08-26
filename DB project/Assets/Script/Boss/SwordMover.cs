using UnityEngine;

public class SwordMover : MonoBehaviour
{
    private Vector3 moveDirection;   // Į �̵� ����
    private float moveSpeed;         // Į �̵� �ӵ�

    private bool canMove = false;    // �̵� ���� ����

    // Į ����� �ӵ� ����
    public void SetDirection(Vector3 direction, float speed)
    {
        moveDirection = direction.normalized; // ���� ����ȭ
        moveSpeed = speed;                     // �ӵ� ����
    }

    // Į �̵� �Ͻ�����
    public void PauseMovement()
    {
        canMove = false;
    }

    // Į �̵� �簳
    public void ResumeMovement()
    {
        canMove = true;
    }

    void Update()
    {
        if (canMove) // �̵� ���� ��
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime; // �̵� ����
        }
    }
}
