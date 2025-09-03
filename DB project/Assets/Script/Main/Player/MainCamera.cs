using UnityEngine;

[RequireComponent(typeof(Camera))] // �ݵ�� Camera ������Ʈ�� �پ�� ��
public class CameraReset : MonoBehaviour
{
    private Camera cam; // ī�޶� ������Ʈ ����

    private void Start()
    {
        cam = GetComponent<Camera>(); // ���� ������Ʈ�� Camera ������Ʈ ��������

        if (!cam.orthographic) // ī�޶� Orthographic�� �ƴϸ�
        {
            Debug.LogError("�� ��ũ��Ʈ�� Orthographic ī�޶󿡼��� �۵��մϴ�."); // ��� ���
            return; // ���� �ߴ�
        }

        // ī�޶� ������ Viewport Rect�� �⺻������ ����
        cam.aspect = (float)Screen.width / Screen.height; // ȭ�� ������ ���� Aspect ����
        cam.rect = new Rect(0f, 0f, 1f, 1f);            // ȭ�� ��ü ��� (x, y, width, height)
    }
}
