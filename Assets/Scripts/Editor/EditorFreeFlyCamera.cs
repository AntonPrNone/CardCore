#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class EditorCameraFollower : MonoBehaviour
{
    void Update()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;

        if (sceneView != null)
        {
            Camera sceneCamera = sceneView.camera;

            if (sceneCamera != null)
            {
                transform.position = sceneCamera.transform.position;
                transform.rotation = sceneCamera.transform.rotation;
            }
        }
    }
}
#endif
