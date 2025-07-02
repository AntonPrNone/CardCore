#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteAlways]
public class EditorCameraFollower : MonoBehaviour
{
#if UNITY_EDITOR
    void Update()
    {
        // �������� ��������� �������� ���� Scene View
        SceneView sceneView = SceneView.lastActiveSceneView;

        if (sceneView != null)
        {
            Camera sceneCamera = sceneView.camera;

            if (sceneCamera != null)
            {
                // ��������� ������� � ������� ������������ ������ � ����� �������
                transform.position = sceneCamera.transform.position;
                transform.rotation = sceneCamera.transform.rotation;
            }
        }
    }
#endif
}
