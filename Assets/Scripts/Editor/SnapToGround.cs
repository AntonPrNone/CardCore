using UnityEngine;
using UnityEditor;

public static class SnapToGround
{
    public static void Snap(GameObject obj, bool useCollider)
    {
        Bounds bounds;

        if (useCollider)
        {
            Collider collider = obj.GetComponentInChildren<Collider>();
            if (collider == null)
            {
                Debug.LogWarning($"Объект {obj.name} не имеет Collider.");
                return;
            }
            bounds = collider.bounds;
        }
        else
        {
            Renderer renderer = obj.GetComponentInChildren<Renderer>();
            if (renderer == null)
            {
                Debug.LogWarning($"Объект {obj.name} не имеет Renderer.");
                return;
            }
            bounds = renderer.bounds;
        }

        Vector3 center = bounds.center;
        float objectBottomY = bounds.min.y;

        Ray ray = new Ray(center, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            float deltaY = objectBottomY - hit.point.y;
            obj.transform.position -= new Vector3(0f, deltaY, 0f);
            Debug.Log($"Объект '{obj.name}' привязан к полу на высоте {hit.point.y}");
        }
        else
        {
            Debug.LogWarning($"Под объектом {obj.name} ничего не найдено.");
        }
    }

    // Оставим для меню Tools
    [MenuItem("Tools/Привязать к полу (Snap to Ground) %#d")]
    static void SnapSelectedToGround()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Snap(obj, useCollider: false); // По умолчанию Renderer
        }
    }
}
