using UnityEngine;

/*
 * GlowStripSetup.cs
 * ------------------
 * This script automatically positions and orients six glow strip planes (typically Unity primitives)
 * around the six faces of a 3D bounds box using a custom vector and quaternion math stack.
 *
 * Tasks:
 *   - Align planes to the faces of a provided bounding box object.
 *   - Use custom math types to determine position and orientation.
 *   - Apply accurate transforms that reflect Unity's scale system (Unity Plane = 10x10 flat on XZ).
 *
 * Extras:
 *   - The setup ensures all glow strips visually wrap the boundsObject properly regardless of its size.
 *   - The script assumes planes are created in Unity’s default orientation, i.e., flat on XZ.
 */

public class GlowStripSetup : MonoBehaviour
{
    [Header("Target Bounds and Glow Planes")]
    public Transform boundsObject;               // The cube or box that defines the bounds to wrap glow strips around

    public Transform glowTop;                    // Plane aligned to the top (+Y)
    public Transform glowBottom;                 // Plane aligned to the bottom (-Y)
    public Transform glowLeft;                   // Plane aligned to the left (-X)
    public Transform glowRight;                  // Plane aligned to the right (+X)
    public Transform glowFront;                  // Plane aligned to the front (+Z)
    public Transform glowBack;                   // Plane aligned to the back (-Z)

    /*
     * Start() calculates the bounds and sets up each face in the appropriate position and size.
     * - Each glow face transform is given:
     *   - Position based on center of bounds plus extents of the appropriate side.
     *   - Rotation based on a given Euler angle.
     *   - Scale based on the bounds scale (for planes divide 10 on the XZ).
    */
    void Start()
    {
        if (boundsObject == null)
            return;
        // Calculate bounds center, scale, and extents
        CustomVector3 center = new CustomVector3(boundsObject.position.x, boundsObject.position.y, boundsObject.position.z);
        CustomVector3 scale = new CustomVector3(boundsObject.localScale.x, boundsObject.localScale.y, boundsObject.localScale.z);
        CustomVector3 half = scale * 0.5f;
        // Top (Y+)
        if (glowTop)
        {
            glowTop.position = (center + new CustomVector3(0, half.y, 0)).ToUnityVector3();
            glowTop.rotation = CustomQuaternion.FromEulerAngles(0, 0, 180).ToUnityQuaternion();
            glowTop.localScale = new CustomVector3(scale.x / 10f, 1f, scale.z / 10f).ToUnityVector3();
        }
        // Bottom (Y-)
        if (glowBottom)
        {
            glowBottom.position = (center + new CustomVector3(0, -half.y, 0)).ToUnityVector3();
            glowBottom.rotation = CustomQuaternion.FromEulerAngles(0, 0, 0).ToUnityQuaternion();
            glowBottom.localScale = new CustomVector3(scale.x / 10f, 1f, scale.z / 10f).ToUnityVector3();
        }
        // Left (X-)
        if (glowLeft)
        {
            glowLeft.position = (center + new CustomVector3(-half.x, 0, 0)).ToUnityVector3();
            glowLeft.rotation = CustomQuaternion.FromEulerAngles(90, 180, 90).ToUnityQuaternion();
            glowLeft.localScale = new CustomVector3(scale.z / 10f, 1f, scale.y / 10f).ToUnityVector3();
        }
        // Right (X+)
        if (glowRight)
        {
            glowRight.position = (center + new CustomVector3(half.x, 0, 0)).ToUnityVector3();
            glowRight.rotation = CustomQuaternion.FromEulerAngles(-90, 180, -90).ToUnityQuaternion();
            glowRight.localScale = new CustomVector3(scale.z / 10f, 1f, scale.y / 10f).ToUnityVector3();
        }
        // Front (Z+)
        if (glowFront)
        {
            glowFront.position = (center + new CustomVector3(0, 0, half.z)).ToUnityVector3();
            glowFront.rotation = CustomQuaternion.FromEulerAngles(90, 0, 180).ToUnityQuaternion();
            glowFront.localScale = new CustomVector3(scale.x / 10f, 1f, scale.y / 10f).ToUnityVector3();
        }
        // Back (Z-)
        if (glowBack)
        {
            glowBack.position = (center + new CustomVector3(0, 0, -half.z)).ToUnityVector3();
            glowBack.rotation = CustomQuaternion.FromEulerAngles(-90, 0, 180).ToUnityQuaternion();
            glowBack.localScale = new CustomVector3(scale.x / 10f, 1f, scale.y / 10f).ToUnityVector3();
        }
    }
}
