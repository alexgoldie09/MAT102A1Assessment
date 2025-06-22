using UnityEngine;

/// <summary>
/// Automatically positions and orients 6 glow strips (planes) to align with the faces of a bounds box using the custom math stack.
/// Designed specifically for Unity Plane primitives (10x10 flat on XZ).
/// </summary>
public class GlowStripSetup : MonoBehaviour
{
    public Transform boundsObject;
    public Transform glowTop, glowBottom, glowLeft, glowRight, glowFront, glowBack;
    public float stripThickness = 1f; // scale in the axis perpendicular to the face

    void Start()
    {
        if (boundsObject == null)
        {
            Debug.LogError("GlowStripSetup: boundsObject is not assigned.");
            return;
        }

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
