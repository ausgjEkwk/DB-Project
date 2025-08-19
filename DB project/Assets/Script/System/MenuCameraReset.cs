using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MenuCameraReset : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();

        if (!cam.orthographic)
        {
            Debug.LogError("�� ��ũ��Ʈ�� Orthographic ī�޶󿡼��� �۵��մϴ�.");
            return;
        }

        // Menu �������� ȭ�� ���� ���
        cam.aspect = (float)Screen.width / Screen.height;
        cam.rect = new Rect(0f, 0f, 1f, 1f);
    }
}
