using UnityEngine;

/*
 * OrbitingAsteroid.cs
 * --------------------
 * This script animates an asteroid that orbits a central planet object in 3D space.
 * 
 * Tasks:
 *   - Orbit the asteroid around the Y-axis of the planet using circular motion in the XZ plane.
 *   - Apply continuous self-spin via cumulative rotation matrix.
 *   - Apply sinusoidal scale modulation over time to simulate dynamic breathing/growing asteroids.
 *   - Compose and apply a full transformation matrix per frame (Translate * Rotate * Scale).
 * 
 * Extras:
 *   - Uses polar coordinates to compute orbital position from angle and radius.
 *   - Uses matrix multiplication to compose transformations manually.
 *   - Extracts position, scale, and orientation from the final matrix to apply to the Unity Transform.
 */

public class OrbitingAsteroid : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform planet;                      // Central object the asteroid orbits
    public float orbitSpeed = 30f;                // Degrees per second around Y-axis
    public bool reverseOrbitDirection = false;    // Inverts orbital direction
    private float orbitRadius = 10f;              // Distance from planet (XZ plane)
    private float yOrbitOffset = 0f;              // Height offset above/below planet

    [Header("Spin Settings")]
    public float spinSpeed = 90f;                 // Self-spin speed (degrees/sec)

    [Header("Scale Settings")]
    public float initialScale = 1f;               // Starting scale size
    private float scaleAmplitude = 0.4f;          // Max variation from initial scale
    private float scaleFrequency = 1f;            // Frequency of scale pulsing

    private float orbitAngle = 0f;                // Accumulated orbit angle in degrees
    private CustomMatrix4x4 rotationMatrix;       // Spin rotation matrix (accumulated)
    private float spinTime = 0f;                  // Used for tracking spin progression

    /*
     * Start() initialises components and randomises orbiting and oscillation behaviour. 
    */
    void Start()
    {
        if (planet == null)
            planet = transform.parent;

        scaleFrequency = Random.Range(0.1f, 2f);
        scaleAmplitude = Random.Range(0.4f, 1f);
        orbitRadius = Random.Range(15f, 20f);
        yOrbitOffset = Random.Range(-18f, 18f);

        rotationMatrix = new CustomMatrix4x4(true);

    }

    /*
     * Update() called every frame. 
     * - Handles asteroid orbit trajectory, spin, and scale oscillation.
    */
    void Update()
    {
        float deltaTime = Time.deltaTime;
        // Determine orbit direction (clockwise or counterclockwise)
        float directionMultiplier = reverseOrbitDirection ? -1f : 1f;
        // Accumulate orbit angle (degrees) without clamping
        orbitAngle += orbitSpeed * deltaTime * directionMultiplier;
        spinTime += deltaTime;
        // Convert orbit angle to radians and calculate local position offset (polar - Cartesian)
        float rad = orbitAngle * Mathf.Deg2Rad;
        float x = Mathf.Cos(rad) * orbitRadius;
        float z = Mathf.Sin(rad) * orbitRadius;
        CustomVector3 localOffset = new CustomVector3(x, yOrbitOffset, z);
        // Calculate world position by adding local orbit offset to planet's position
        CustomVector3 planetPos = new CustomVector3(planet.position.x, planet.position.y, planet.position.z);
        CustomVector3 worldPos = planetPos + localOffset;
        // Accumulate spin using rotation matrix around Y-axis
        CustomMatrix4x4 deltaSpin = CustomMatrix4x4.CreateRotationY(spinSpeed * deltaTime);
        rotationMatrix = deltaSpin * rotationMatrix;
        // Calculate scale dynamically using sine wave for pulsing effect
        float scaleTime = Mathf.Sin(Time.time * scaleFrequency);
        float dynamicScale = initialScale + scaleAmplitude * scaleTime;
        // Build individual transform matrices
        CustomMatrix4x4 scaleMatrix = CustomMatrix4x4.CreateScaling(dynamicScale, dynamicScale, dynamicScale);
        CustomMatrix4x4 translationMatrix = CustomMatrix4x4.CreateTranslation(worldPos.x, worldPos.y, worldPos.z);
        // Final transformation matrix: Translate * Rotate * Scale
        CustomMatrix4x4 fullMatrix = translationMatrix * rotationMatrix * scaleMatrix;
        // Extract final position from transformation matrix (column 3)
        CustomVector3 finalPosition = new CustomVector3(
            fullMatrix.m[0, 3],
            fullMatrix.m[1, 3],
            fullMatrix.m[2, 3]
        );
        // Convert rotation matrix to quaternion for Unity's Transform system
        CustomQuaternion finalRotation = CustomQuaternion.FromMatrix4x4(rotationMatrix);
        // Extract each scale axis magnitude using column vector norms
        float scaleX = new CustomVector3(
            fullMatrix.m[0, 0],
            fullMatrix.m[1, 0],
            fullMatrix.m[2, 0]
        ).Magnitude();
        float scaleY = new CustomVector3(
            fullMatrix.m[0, 1],
            fullMatrix.m[1, 1],
            fullMatrix.m[2, 1]
        ).Magnitude();
        float scaleZ = new CustomVector3(
            fullMatrix.m[0, 2],
            fullMatrix.m[1, 2],
            fullMatrix.m[2, 2]
        ).Magnitude();
        CustomVector3 finalScale = new CustomVector3(scaleX, scaleY, scaleZ);
        // Apply to Unity transform
        transform.position = finalPosition.ToUnityVector3();
        transform.rotation = finalRotation.ToUnityQuaternion();
        transform.localScale = finalScale.ToUnityVector3();
    }
}
