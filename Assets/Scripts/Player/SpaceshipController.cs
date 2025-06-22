using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


/// <summary>
/// Spaceship controller — Stable Rotation (Pro Flight Pattern).
/// </summary>
public class SpaceshipController : MonoBehaviour
{
    private CustomVector3 position = new CustomVector3(0, 0, 0);
    private CustomQuaternion rotation = CustomQuaternion.Identity();

    private float currentYaw = 0f;
    private float currentPitch = 0f;
    private float currentRoll = 0f;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float boostMultiplier = 2f;

    [Header("Rotation Settings")]
    public float rotateSpeed = 60f;
    public float mouseSensitivity = 0.5f;
    public float mouseDeadzone = 0.02f;
    public float pitchScale = 0.5f;
    public float rollScale = 0.5f;

    [Header("Clamping")]
    public float minPitch = -60f;
    public float maxPitch = 60f;
    public bool clampRoll = false;
    public float minRoll = -60f;
    public float maxRoll = 60f;

    [Header("World Bounds")]
    [SerializeField] private Transform boundsObject; // Cube object used as AABB boundary

    [Header("Post-Processing")]
    public VolumeProfile volumeProfile;
    private LensDistortion lensDistortion;


    private bool mouseHasMoved = false;

    void Start()
    {
        if (volumeProfile != null)
        {
            if (volumeProfile.TryGet(out LensDistortion ld))
            {
                lensDistortion = ld;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rotation = CustomQuaternion.Identity();
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        // --- Mouse Input ---
        float mouseX = -Input.GetAxis("Mouse X"); // Roll
        float mouseY = Input.GetAxis("Mouse Y");  // Pitch

        if (!mouseHasMoved && (Mathf.Abs(mouseX) > mouseDeadzone || Mathf.Abs(mouseY) > mouseDeadzone))
            mouseHasMoved = true;

        // --- Yaw (A/D) ---
        float yawInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(yawInput) > 0.001f)
            currentYaw += rotateSpeed * yawInput * deltaTime;

        // --- Pitch (Mouse Y) ---
        if (mouseHasMoved && Mathf.Abs(mouseY) > mouseDeadzone)
        {
            float pitch = rotateSpeed * mouseY * pitchScale * mouseSensitivity * deltaTime;
            currentPitch = Mathf.Clamp(currentPitch + pitch, minPitch, maxPitch);
        }

        // --- Roll (Mouse X) ---
        if (mouseHasMoved && Mathf.Abs(mouseX) > mouseDeadzone)
        {
            float roll = rotateSpeed * mouseX * rollScale * mouseSensitivity * deltaTime;
            currentRoll = clampRoll ? Mathf.Clamp(currentRoll + roll, minRoll, maxRoll) : currentRoll + roll;
        }

        // --- Apply Rotation using CreateRotationXYZ ---
        CustomMatrix4x4 rotationMatrix = CustomMatrix4x4.CreateRotationXYZ(currentPitch, currentYaw, currentRoll);
        rotation = CustomQuaternion.FromMatrix4x4(rotationMatrix).Normalize();

        // --- Movement ---
        float moveInput = Input.GetAxis("Vertical");
        float speed = Input.GetKey(KeyCode.Space) ? moveSpeed * boostMultiplier : moveSpeed;

        if (Mathf.Abs(moveInput) > 0.001f)
        {
            CustomVector3 forward = new CustomVector3(rotationMatrix.m[0, 2], rotationMatrix.m[1, 2], rotationMatrix.m[2, 2]).Normalize();
            position += forward * (speed * moveInput * deltaTime);
        }

        // --- Clamp to AABB Bounds ---
        if (boundsObject != null)
        {
            CustomVector3 center = new CustomVector3(
                boundsObject.position.x,
                boundsObject.position.y,
                boundsObject.position.z
            );

            CustomVector3 halfExtents = new CustomVector3(
                boundsObject.localScale.x / 2f,
                boundsObject.localScale.y / 2f,
                boundsObject.localScale.z / 2f
            );

            CustomVector3 relative = position - center;

            position = new CustomVector3(
                Mathf.Clamp(relative.x, -halfExtents.x, halfExtents.x),
                Mathf.Clamp(relative.y, -halfExtents.y, halfExtents.y),
                Mathf.Clamp(relative.z, -halfExtents.z, halfExtents.z)
            ) + center;
        }

        // --- Apply Transform ---
        transform.position = position.ToUnityVector3();
        transform.rotation = rotation.ToUnityQuaternion();

        // --- Apply Effects ---
        if (lensDistortion != null)
        {
            float targetIntensity = Input.GetKey(KeyCode.Space) ? -0.8f : 0f;
            lensDistortion.intensity.value = Mathf.Clamp(
                Mathf.Lerp(lensDistortion.intensity.value, targetIntensity, Time.deltaTime * 5f),
                -1f, 0f
            );

        }

        // --- Escape Unlock ---
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void OnDrawGizmos()
    {
        if (boundsObject == null)
            return;

        Gizmos.color = Color.green;

        // Draw the wire cube representing boundsObject
        Gizmos.DrawWireCube(boundsObject.position, boundsObject.localScale);
    }

}
