using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Custom sphere collider used for simple 3D collision detection.
/// Uses the GameObject's scale as base radius and allows manual extension.
/// </summary>
public class CustomSphereCollider : MonoBehaviour
{
    [Tooltip("Extra radius to add to the object's base scale-derived radius.")]
    public float extraRadius = 0f;

    [Tooltip("Color used to draw the collider gizmo in the editor.")]
    public Color gizmoColor = Color.green;

    /// <summary>
    /// Static list of all active CustomSphereColliders.
    /// </summary>
    public static readonly List<CustomSphereCollider> All = new();

    /// <summary>
    /// Total effective radius (based on scale + extra).
    /// </summary>
    public float Radius
    {
        get
        {
            float baseRadius = transform.localScale.magnitude / 2f;
            return baseRadius + extraRadius;
        }
    }

    /// <summary>
    /// Checks if this sphere is colliding with another CustomSphereCollider.
    /// </summary>
    public bool IsCollidingWith(CustomSphereCollider other)
    {
        float combinedRadius = this.Radius + other.Radius;
        float distanceSqr = (transform.position - other.transform.position).sqrMagnitude;
        return distanceSqr <= combinedRadius * combinedRadius;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }

    private void OnEnable()
    {
        if (!All.Contains(this))
            All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
    }
}
