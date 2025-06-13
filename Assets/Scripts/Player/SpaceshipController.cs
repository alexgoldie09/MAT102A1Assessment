using UnityEngine;

/// <summary>
/// Spaceship controller — Stable Rotation (Pro Flight Pattern).
/// - Mouse Y → Pitch (clamped)
/// - Mouse X → Roll (clamped optional or full)
/// - A/D → Yaw (world space yaw, stable)
/// - W/S → Move forward/back
/// - Space → Boost
/// - Cursor locked.
/// </summary>
public class SpaceshipController : MonoBehaviour
{
    // Spaceship state
    private CustomVector3 position = new CustomVector3(0, 0, 0);
    private CustomQuaternion rotation = CustomQuaternion.Identity();

    // Explicit rotation state for stability
    private float currentYaw = 0f;
    private float currentPitch = 0f;
    private float currentRoll = 0f;

    // Settings
    public float moveSpeed = 5f;
    public float boostMultiplier = 2f;
    public float rotateSpeed = 60f;
    public float mouseSensitivity = 0.5f;
    public float mouseDeadzone = 0.02f;

    public float pitchScale = 0.5f;
    public float rollScale = 0.5f;

    public float minPitch = -60f;
    public float maxPitch = 60f;

    public bool clampRoll = false;
    public float minRoll = -60f;
    public float maxRoll = 60f;

    private bool mouseHasMoved = false;

    void Start()
    {
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initial rotation = identity
        rotation = CustomQuaternion.Identity();
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        // --- Mouse Input ---
        float mouseX = -Input.GetAxis("Mouse X"); // Roll
        float mouseY = Input.GetAxis("Mouse Y"); // Pitch (invert)

        // Detect if mouse moved
        if (!mouseHasMoved && (Mathf.Abs(mouseX) > mouseDeadzone || Mathf.Abs(mouseY) > mouseDeadzone))
        {
            mouseHasMoved = true;
        }

        // --- Apply Yaw (world yaw axis) ---
        float yawInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(yawInput) > 0.001f)
        {
            float yawAngle = rotateSpeed * yawInput * deltaTime;
            currentYaw += yawAngle;
        }

        // --- Apply Pitch (clamped) ---
        if (mouseHasMoved && Mathf.Abs(mouseY) > mouseDeadzone)
        {
            float pitchInput = mouseY * pitchScale;
            float pitchAngle = rotateSpeed * pitchInput * mouseSensitivity * deltaTime;

            currentPitch = Mathf.Clamp(currentPitch + pitchAngle, minPitch, maxPitch);
        }

        // --- Apply Roll (clamped optional) ---
        if (mouseHasMoved && Mathf.Abs(mouseX) > mouseDeadzone)
        {
            float rollInput = mouseX * rollScale;
            float rollAngle = rotateSpeed * rollInput * mouseSensitivity * deltaTime;

            if (clampRoll)
            {
                currentRoll = Mathf.Clamp(currentRoll + rollAngle, minRoll, maxRoll);
            }
            else
            {
                currentRoll += rollAngle;
            }
        }

        // --- Rebuild full rotation from scratch ---
        CustomQuaternion yawQ = CustomQuaternion.FromAxisAngle(new CustomVector3(0, 1, 0), currentYaw);
        CustomQuaternion pitchQ = CustomQuaternion.FromAxisAngle(new CustomVector3(1, 0, 0), currentPitch);
        CustomQuaternion rollQ = CustomQuaternion.FromAxisAngle(new CustomVector3(0, 0, 1), currentRoll);

        rotation = (yawQ * pitchQ * rollQ).Normalize();

        // --- Movement Input ---
        float moveInput = Input.GetAxis("Vertical");

        float currentMoveSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.Space))
        {
            currentMoveSpeed *= boostMultiplier;
        }

        if (Mathf.Abs(moveInput) > 0.001f)
        {
            // Get forward vector from rotation
            CustomMatrix4x4 rotMatrix = rotation.ToMatrix4x4();
            CustomVector3 forward = new CustomVector3(rotMatrix.m[0, 2], rotMatrix.m[1, 2], rotMatrix.m[2, 2]).Normalize();

            // Move along forward
            position += forward * (currentMoveSpeed * moveInput * deltaTime);
        }

        // --- Apply final transform ---
        transform.position = position.ToUnityVector3();
        transform.rotation = rotation.ToUnityQuaternion();

        // --- Optional: Unlock mouse on Escape ---
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
