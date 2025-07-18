using UnityEngine;

/// <summary>
/// Вспомогательные методы для расчёта дистанций и радиусов в боевой системе.
/// </summary>
public static class CombatUtils
{
    /// <summary>
    /// Получить примерный радиус коллайдера.
    /// </summary>
    public static float GetColliderApproxRadius(Collider col)
    {
        if (col is CapsuleCollider capsule) return capsule.radius;
        if (col is SphereCollider sphere) return sphere.radius;
        if (col is BoxCollider box)
        {
            Vector3 extents = box.bounds.extents;
            return Mathf.Max(extents.x, extents.z);
        }
        Vector3 extentsFallback = col.bounds.extents;
        return Mathf.Max(extentsFallback.x, extentsFallback.z);
    }

    /// <summary>
    /// Получить расстояние между ближайшими точками двух коллайдеров (по поверхности).
    /// </summary>
    public static float GetSurfaceDistance(Transform a, Transform b)
    {
        if (a == null || b == null) return Mathf.Infinity;
        Collider colA = a.GetComponent<Collider>();
        Collider colB = b.GetComponent<Collider>();
        if (colA == null || colB == null)
            return Vector3.Distance(a.position, b.position); // fallback
        Vector3 closestA = colA.ClosestPoint(colB.bounds.center);
        Vector3 closestB = colB.ClosestPoint(closestA);
        return Vector3.Distance(closestA, closestB);
    }
}