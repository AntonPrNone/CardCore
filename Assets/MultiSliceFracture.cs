using UnityEngine;
using EzySlice;
using UnityEngine.InputSystem;  // Новый Input System
using System.Collections.Generic;

public class MultiSliceFracture : MonoBehaviour
{
    [Tooltip("Количество случайных плоскостей реза")]
    public int sliceCount = 3;

    [Tooltip("Сила взрыва, применяемая к кускам")]
    public float explosionForce = 300f;

    [Tooltip("Радиус действия взрыва")]
    public float explosionRadius = 5f;

    [Tooltip("Материал для срезанных поверхностей")]
    public Material crossSectionMaterial;

    private bool fractured = false;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Fracture();
        }
    }

    public void Fracture()
    {
        if (fractured) return;
        fractured = true;

        List<GameObject> currentPieces = new List<GameObject>() { gameObject };

        for (int i = 0; i < sliceCount; i++)
        {
            List<GameObject> newPieces = new List<GameObject>();

            foreach (GameObject piece in currentPieces)
            {
                UnityEngine.Plane unityPlane = GenerateRandomPlane(piece);
                EzySlice.Plane ezyPlane = ConvertToEzyPlane(unityPlane);

                SlicedHull hull = piece.Slice(ezyPlane, crossSectionMaterial);

                if (hull != null)
                {
                    GameObject upperHull = hull.CreateUpperHull(piece, crossSectionMaterial);
                    GameObject lowerHull = hull.CreateLowerHull(piece, crossSectionMaterial);

                    SetupFragment(upperHull);
                    SetupFragment(lowerHull);

                    Destroy(piece);

                    newPieces.Add(upperHull);
                    newPieces.Add(lowerHull);
                }
                else
                {
                    newPieces.Add(piece);
                }
            }

            currentPieces = newPieces;
        }

        foreach (GameObject piece in currentPieces)
        {
            Rigidbody rb = piece.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            Destroy(piece, 5f);
        }
    }

    private UnityEngine.Plane GenerateRandomPlane(GameObject obj)
    {
        Bounds bounds = obj.GetComponent<Renderer>().bounds;

        Vector3 point = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );

        Vector3 normal = Random.onUnitSphere;

        return new UnityEngine.Plane(normal, point);
    }

    private EzySlice.Plane ConvertToEzyPlane(UnityEngine.Plane unityPlane)
    {
        Vector3 normal = unityPlane.normal;
        Vector3 pointOnPlane = -normal * unityPlane.distance;
        return new EzySlice.Plane(normal, pointOnPlane);
    }

    private void SetupFragment(GameObject fragment)
    {
        fragment.layer = gameObject.layer;

        if (fragment.GetComponent<Collider>() == null)
        {
            MeshCollider collider = fragment.AddComponent<MeshCollider>();
            collider.convex = true;
        }

        if (fragment.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = fragment.AddComponent<Rigidbody>();
            rb.mass = 1f;
        }
    }
}
