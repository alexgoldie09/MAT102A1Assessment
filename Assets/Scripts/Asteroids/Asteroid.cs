using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float speed = 2f;

    private Transform target;
    private CustomVector3 position;
    private CustomVector3 direction;

    private float lifetime;
    private float currentTime;

    private float initialScale = 1.2f;
    private float finalScale = 0.2f;

    private CustomQuaternion currentRotation;
    private CustomVector3 angularVelocity; // axis and speed
    private Vector3 originalLocalScale;

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        position = new CustomVector3(transform.position.x, transform.position.y, transform.position.z);

        if (target != null)
        {
            CustomVector3 targetPos = new CustomVector3(target.position.x, target.position.y, target.position.z);
            direction = (targetPos - position).Normalize();
        }

        lifetime = Random.Range(3f, 6f);
        currentTime = 0f;

        // Save original local scale
        originalLocalScale = transform.localScale;

        // Spin setup
        currentRotation = CustomQuaternion.Identity();
        angularVelocity = new CustomVector3(
            Random.Range(-60f, 60f),
            Random.Range(-60f, 60f),
            Random.Range(-60f, 60f)
        );
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        currentTime += deltaTime;

        // --- Move forward ---
        CustomVector3 deltaMove = direction * speed * deltaTime;
        position += deltaMove;

        // --- Shrink over time ---
        float t = Mathf.Clamp01(currentTime / lifetime);
        float scaleValue = Mathf.Lerp(initialScale, finalScale, t);

        // --- Spin ---
        CustomQuaternion deltaRotation = CustomQuaternion.FromEulerAngles(
            angularVelocity.x * deltaTime,
            angularVelocity.y * deltaTime,
            angularVelocity.z * deltaTime
        );
        currentRotation = (deltaRotation * currentRotation).Normalize();

        // --- Apply transformation ---
        ApplyMatrix(scaleValue);

        if (currentTime >= lifetime)
            Destroy(gameObject);
    }

    private void ApplyMatrix(float uniformScale)
    {
        // Only apply rotation and scale via matrix
        CustomMatrix4x4 scaleMatrix = CustomMatrix4x4.CreateScaling(uniformScale, uniformScale, uniformScale);
        CustomMatrix4x4 rotationMatrix = currentRotation.ToMatrix4x4();
        CustomMatrix4x4 transformMatrix = rotationMatrix * scaleMatrix;

        // Apply position directly
        transform.position = position.ToUnityVector3();

        // Apply rotation
        transform.rotation = currentRotation.ToUnityQuaternion();

        // Apply scale manually from original prefab
        CustomVector3 scaleVec = new CustomVector3(
            originalLocalScale.x * uniformScale,
            originalLocalScale.y * uniformScale,
            originalLocalScale.z * uniformScale
        );
        transform.localScale = scaleVec.ToUnityVector3();
    }
}
