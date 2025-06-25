using UnityEngine;

/*
 * BorderGlowController.cs
 * -----------------------
 * This script controls the emissive glow and transparency of border wall planes in the scene.
 *
 * Tasks:
 *  - Identify the player's distance from each of the six AABB bounding box planes (top, bottom, left, right, front, back).
 *  - Interpolate alpha and emission values using inverse linear distance (clamped by fadeDistance).
 *  - Apply the results to the wall renderers for a glow feedback effect.
 *
 * Extras:
 *  - The emissive intensity scales with proximity to simulate a glow effect.
 *  - All walls fade in only when the player approaches within a defined threshold.
 *  - Uses absolute distance comparisons between player position and wall plane positions.
 *  - Linearly maps distance to alpha using: alpha = 1 - (distance / fadeDistance)
 *  - This mapping is clamped to [0, 1] using Mathf.Clamp01 to prevent visual artifacts.
 */

public class BorderGlowController : MonoBehaviour
{
    [Header("Transform References")]
    public Transform player;              // Reference to the player's transform
    public Transform boundsObject;        // Bounding box object defining the play area's extents

    [Header("Renderer References")]
    public Renderer topRenderer;          // Glow renderer for the top face
    public Renderer bottomRenderer;       // Glow renderer for the bottom face
    public Renderer leftRenderer;         // Glow renderer for the left face
    public Renderer rightRenderer;        // Glow renderer for the right face
    public Renderer frontRenderer;        // Glow renderer for the front face
    public Renderer backRenderer;         // Glow renderer for the back face

    [Header("Fade Settings")]
    public float fadeDistance = 5f;       // Maximum distance before glow fully fades out

    private Renderer[] renderers;         // Stored array of all renderers

    /*
     * Start() initialises components and initialises all renderers to be invisible. 
    */
    void Start()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if(boundsObject == null)
        {
            boundsObject = transform;
        }

        renderers = new Renderer[] {
            topRenderer, bottomRenderer,
            leftRenderer, rightRenderer,
            frontRenderer, backRenderer
        };

        // Loop through all renderers
        foreach (Renderer rend in renderers)
        {
            if (rend == null) continue;
            // Set alpha to 0
            Color invisible = rend.material.color;
            invisible.a = 0f;
            rend.material.color = invisible;
            // Disable emission if available
            if (rend.material.HasProperty("_EmissionColor"))
            {
                rend.material.SetColor("_EmissionColor", Color.black);
            }
        }
    }

    /*
     * Update() called every frame. 
     * - Handles distance calculations and glow based on proximity.
    */
    void Update()
    {
        if (player == null || boundsObject == null)
            return;
        // Get center and half-extents of the bounding box
        CustomVector3 center = new CustomVector3(boundsObject.position.x, boundsObject.position.y, boundsObject.position.z);
        CustomVector3 scale = new CustomVector3(boundsObject.localScale.x, boundsObject.localScale.y, boundsObject.localScale.z);
        CustomVector3 half = scale * 0.5f;
        // Get player position
        CustomVector3 playerPos = new CustomVector3(player.position.x, player.position.y, player.position.z);
        // Calculate distance to each bounding plane
        float topDist = Mathf.Abs((center.y + half.y) - playerPos.y);
        float bottomDist = Mathf.Abs((center.y - half.y) - playerPos.y);
        float leftDist = Mathf.Abs((center.x - half.x) - playerPos.x);
        float rightDist = Mathf.Abs((center.x + half.x) - playerPos.x);
        float frontDist = Mathf.Abs((center.z + half.z) - playerPos.z);
        float backDist = Mathf.Abs((center.z - half.z) - playerPos.z);
        // Glow based on proximity
        SetGlowAlpha(topRenderer, topDist);
        SetGlowAlpha(bottomRenderer, bottomDist);
        SetGlowAlpha(leftRenderer, leftDist);
        SetGlowAlpha(rightRenderer, rightDist);
        SetGlowAlpha(frontRenderer, frontDist);
        SetGlowAlpha(backRenderer, backDist);
    }

    /*
     * SetGlowAlpha() adjusts the alpha and emissive intensity of the given 
     * renderer based on the player's proximity to the bounding face.
    */
    private void SetGlowAlpha(Renderer rend, float dist)
    {
        if (rend == null) return;
        // Inverse linear interpolation: alpha increases as distance decreases
        float alpha = Mathf.Clamp01(1f - (dist / fadeDistance));
        // Apply alpha to base color
        Color baseColor = rend.material.color;
        baseColor.a = alpha;
        rend.material.color = baseColor;
        // Optional: boost emission based on alpha
        if (rend.material.HasProperty("_EmissionColor"))
        {
            // Stronger red glow closer to player
            Color emission = Color.red * alpha * 2f;
            rend.material.SetColor("_EmissionColor", emission);
        }
    }
}
