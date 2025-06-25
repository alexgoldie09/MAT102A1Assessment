using UnityEngine;
using System.Collections.Generic;

/*
 * CustomSphereCollider.cs
 * ------------------------
 * This script defines a lightweight, custom sphere-based collision detection system for 3D objects.
 *
 * Tasks:
 *  - Automatically registers and unregisters itself in a static list (`All`) for efficient global collision checks.
 *  - Calculates an effective collision radius by averaging the object’s scale and allowing optional radius extension.
 *  - Performs basic sphere-sphere intersection detection using distance comparison.
 *  - Visualizes the collider in the Unity editor using OnDrawGizmos for debug and design-time feedback.
 *
 * Extras:
 *  - Uses the Euclidean distance formula to detect intersections.
 *  - Squared distance is used for optimization (avoids unnecessary sqrt computation).
 *  - Used for planet, asteroid field and spaceship physics response.
 */

public class CustomSphereCollider : MonoBehaviour
{
    public float extraRadius = 0f;            // Add to object's base scale

    public Color gizmoColor = Color.green;    // Color for the collider gizmo

    // Static list of all CustomSphereCollider instances for global lookup
    public static readonly List<CustomSphereCollider> All = new();

    /*
     * Getter method that calculates the effective collision radius of this object.
     * - Based on the magnitude of local scale, divided by 2 (approximate bounding sphere),
     * plus optional extra.
     * - Extra scale was added for objects like the asteroids which are children objects.
    */
    public float Radius
    {
        get
        {
            float baseRadius = transform.localScale.magnitude / 2f;
            return baseRadius + extraRadius;
        }
    }

    /*
     * IsCollidingWith() checks if this sphere is colliding with another CustomSphereCollider.
     * - Collision occurs if distance is less than or equal to combined radii
    */
    public bool IsCollidingWith(CustomSphereCollider other)
    {
        // Sum of both radii
        float combinedRadius = this.Radius + other.Radius;
        // Squared Euclidean distance between centers
        float distanceSqr = (transform.position - other.transform.position).sqrMagnitude;
        return distanceSqr <= combinedRadius * combinedRadius;
    }

    /*
     * OnDrawGizmos() draws the collider as a wireframe sphere in the scene view.
     * - Useful for debugging.
    */
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }

    /*
     * OnEnable() automatically adds this collider to the static list when enabled.
    */
    private void OnEnable()
    {
        if (!All.Contains(this))
            All.Add(this);
    }

    /*
     * OnDisable() automatically removes this collider from the static list when disabled.
    */
    private void OnDisable()
    {
        All.Remove(this);
    }
}
