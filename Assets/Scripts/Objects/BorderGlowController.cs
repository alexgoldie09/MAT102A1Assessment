using UnityEngine;

/// <summary>
/// Dynamically adjusts the alpha or emissive intensity of glow planes
/// based on how close the player is to the bounding box walls.
/// </summary>
public class BorderGlowController : MonoBehaviour
{
    public Transform player;
    public Transform boundsObject;

    public Renderer topRenderer;
    public Renderer bottomRenderer;
    public Renderer leftRenderer;
    public Renderer rightRenderer;
    public Renderer frontRenderer;
    public Renderer backRenderer;

    public float fadeDistance = 5f; // How close before fully glowing

    private Renderer[] renderers;

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

        // Set all materials to fully invisible and disable emission
        foreach (Renderer rend in renderers)
        {
            if (rend == null) continue;

            Color invisible = rend.material.color;
            invisible.a = 0f;
            rend.material.color = invisible;

            if (rend.material.HasProperty("_EmissionColor"))
            {
                rend.material.SetColor("_EmissionColor", Color.black);
            }
        }
    }

    void Update()
    {
        if (player == null || boundsObject == null)
            return;

        CustomVector3 center = new CustomVector3(boundsObject.position.x, boundsObject.position.y, boundsObject.position.z);
        CustomVector3 scale = new CustomVector3(boundsObject.localScale.x, boundsObject.localScale.y, boundsObject.localScale.z);
        CustomVector3 half = scale * 0.5f;
        CustomVector3 playerPos = new CustomVector3(player.position.x, player.position.y, player.position.z);

        // Distance from each face
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

    void SetGlowAlpha(Renderer rend, float dist)
    {
        if (rend == null) return;

        float alpha = Mathf.Clamp01(1f - (dist / fadeDistance));

        Color baseColor = rend.material.color;
        baseColor.a = alpha;
        rend.material.color = baseColor;

        // Optional: boost emission based on alpha
        if (rend.material.HasProperty("_EmissionColor"))
        {
            Color emission = Color.red * alpha * 2f; // red glow
            rend.material.SetColor("_EmissionColor", emission);
        }
    }
}
