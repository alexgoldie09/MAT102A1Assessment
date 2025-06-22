using UnityEngine;

public class OrbitingAsteroid : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform planet;             // Planet to orbit around
    public float orbitSpeed = 30f;       // Degrees per second
    public bool reverseOrbitDirection = false; // Toggle for clockwise vs counter-clockwise
    private float orbitRadius = 10f;      // Distance from the planet
    private float yOrbitOffset = 0f;      // Custom Y offset from planet center

    [Header("Spin Settings")]
    public float spinSpeed = 90f;        // Degrees per second

    [Header("Scale Settings")]
    public float initialScale = 1f;
    private float scaleAmplitude = 0.4f;  // Amount to grow/shrink
    private float scaleFrequency = 1f;    // Oscillations per second

    private float orbitAngle = 0f;       // Orbit angle (degrees)
    private CustomMatrix4x4 rotationMatrix; // Cached spin matrix
    private float spinTime = 0f;         // Used for accumulating spin


    private void Start()
    {
        if (planet == null)
            planet = transform.parent;

        // Randomize scale frequency, scale amp, radius and height offset
        scaleFrequency = Random.Range(0.1f, 2f);
        scaleAmplitude = Random.Range(0.4f, 1f);
        orbitRadius = Random.Range(15f, 20f);
        yOrbitOffset = Random.Range(-18f, 18f);

        // Initialize to identity matrix for spin
        rotationMatrix = new CustomMatrix4x4(true);

    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        // Orbit direction multiplier
        float directionMultiplier = reverseOrbitDirection ? -1f : 1f;

        // Update orbit angle without clamping (continuous spinning)
        orbitAngle += orbitSpeed * deltaTime * directionMultiplier;
        spinTime += deltaTime;

        // Orbit offset calculation in XZ plane
        float rad = orbitAngle * Mathf.Deg2Rad;
        float x = Mathf.Cos(rad) * orbitRadius;
        float z = Mathf.Sin(rad) * orbitRadius;
        CustomVector3 localOffset = new CustomVector3(x, yOrbitOffset, z);

        // Get planet position
        CustomVector3 planetPos = new CustomVector3(
            planet.position.x,
            planet.position.y,
            planet.position.z
        );

        // Final world position
        CustomVector3 worldPos = planetPos + localOffset;

        // Spin accumulation via matrix rotation
        CustomMatrix4x4 deltaSpin = CustomMatrix4x4.CreateRotationY(spinSpeed * deltaTime);
        rotationMatrix = deltaSpin * rotationMatrix;

        // Pulsing scale using sine wave
        float scaleTime = Mathf.Sin(Time.time * scaleFrequency);
        float dynamicScale = initialScale + scaleAmplitude * scaleTime;

        // Dynamic scale (pulsating)
        CustomMatrix4x4 scaleMatrix = CustomMatrix4x4.CreateScaling(dynamicScale, dynamicScale, dynamicScale);

        // Translation to orbit position
        CustomMatrix4x4 translationMatrix = CustomMatrix4x4.CreateTranslation(worldPos.x, worldPos.y, worldPos.z);

        // Compose: Translate * Rotate * Scale
        CustomMatrix4x4 fullMatrix = translationMatrix * rotationMatrix * scaleMatrix;

        // Extract position from matrix
        CustomVector3 finalPosition = new CustomVector3(
            fullMatrix.m[0, 3],
            fullMatrix.m[1, 3],
            fullMatrix.m[2, 3]
        );

        // Extract rotation from matrix
        CustomQuaternion finalRotation = CustomQuaternion.FromMatrix4x4(rotationMatrix);

        // Extract scale from matrix basis vectors
        float scaleX = new CustomVector3(fullMatrix.m[0, 0], fullMatrix.m[1, 0], fullMatrix.m[2, 0]).Magnitude();
        float scaleY = new CustomVector3(fullMatrix.m[0, 1], fullMatrix.m[1, 1], fullMatrix.m[2, 1]).Magnitude();
        float scaleZ = new CustomVector3(fullMatrix.m[0, 2], fullMatrix.m[1, 2], fullMatrix.m[2, 2]).Magnitude();

        // Apply to Unity transform
        transform.position = finalPosition.ToUnityVector3();
        transform.rotation = finalRotation.ToUnityQuaternion();
        transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }
}
