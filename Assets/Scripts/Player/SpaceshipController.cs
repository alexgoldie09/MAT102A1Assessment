using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/*
 * SpaceshipController.cs
 * -----------------------
 * This script controls the movement, rotation, and collision logic of the 
 * player using a custom math stack.
 * 
 * Tasks:
 *  - Handles movement via forward thrust and boost.
 *  - Handles rotation through pitch (mouse Y), yaw (A/D), and roll (mouse X).
 *  - Applies transformations using a combination of custom translation, rotation, and quaternion matrices.
 *  - Clamps position to a world bounding box (AABB).
 *  - Detects sphere-based collisions using CustomSphereCollider.
 *  - Triggers VFX and game over logic on collision.
 *  - Includes post-processing effects (e.g. lens distortion when boosting).
 * 
 * Extras:
 *  - Rotation is handled using Euler angles and converted to a custom rotation matrix (CreateRotationXYZ).
 *  - Movement applies forward vector extracted from the rotation matrix (3rd column).
 *  - Final transform is applied via a translation * rotation matrix multiplication.
 *  - World boundaries are clamped using AABB logic.
 */

public class SpaceshipController : MonoBehaviour
{
    // Current spaceship position (custom vector math stack)
    private CustomVector3 position = new CustomVector3(0, 0, 0);
    // Current orientation in space, stored as quaternion for stable 3D rotation
    private CustomQuaternion rotation = CustomQuaternion.Identity();

    private float currentYaw = 0f;            // Rotation around Y-axis
    private float currentPitch = 0f;          // Rotation around X-axis
    private float currentRoll = 0f;           // Rotation around Z-axis

    [Header("Movement Settings")]
    public float moveSpeed = 5f;              // Base speed
    public float boostMultiplier = 2f;        // Boost speed multiplier

    [Header("Rotation Settings")]
    public float rotateSpeed = 60f;           // Speed for rotating (deg/sec)
    public float mouseSensitivity = 0.5f;     // Mouse input sensitivity
    public float mouseDeadzone = 0.02f;       // Ignores unwanted mouse drift
    public float pitchScale = 0.5f;           // Pitch multiplier
    public float rollScale = 0.5f;            // Roll multiplier

    [Header("Clamping")]
    public float minPitch = -60f;             // Minimum value to clamp pitch
    public float maxPitch = 60f;              // Maximum value to clamp pitch
    public bool clampRoll = false;            // Optional: enable clamp of roll
    public float minRoll = -60f;              // Minimum value to clamp roll
    public float maxRoll = 60f;               // Maximum value to clamp roll

    // Reference cube defining axis-aligned bounding box (AABB) for movement
    [Header("World Bounds")]
    [SerializeField] private Transform boundsObject;

    [Header("Post-Processing")]
    public VolumeProfile volumeProfile;       // Reference to global post-processing
    private LensDistortion lensDistortion;    // Warp effect on boost

    [Header("Game VFX")]
    public GameObject destroyVFX;             // Destroy particle effect

    private bool mouseHasMoved = false;       // Check if mouse input has been made
    private CustomSphereCollider myCollider;  // Reference to manual sphere collider

    /*
     * Start() initialises components and sets up cursor and post-processing. 
    */
    void Start()
    {
        if (volumeProfile != null && volumeProfile.TryGet(out LensDistortion ld))
            lensDistortion = ld;

        myCollider = GetComponent<CustomSphereCollider>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rotation = CustomQuaternion.Identity();
        position = new CustomVector3(transform.position.x, transform.position.y, transform.position.z);
    }

    /*
     * Update() called every frame. 
     * - Handles input, movement, VFX, and collision detection.
    */
    void Update()
    {
        float deltaTime = Time.deltaTime;
        HandleMovement(deltaTime);
        HandleEffects(deltaTime);
        HandleEscapeUnlock();
        CheckCollisions();
    }

    /*
     * HandleMovement() applies mouse and keyboard input to control the spaceship's 
     * position and rotation.
     * - World bounds is added to prevent the player from going out too far.
    */
    private void HandleMovement(float deltaTime)
    {
        // Stores mouse input
        float mouseX = -Input.GetAxis("Mouse X"); // Roll
        float mouseY = Input.GetAxis("Mouse Y");  // Pitch
        // Set movement flag once user moves beyond deadzone
        if (!mouseHasMoved && (Mathf.Abs(mouseX) > mouseDeadzone || Mathf.Abs(mouseY) > mouseDeadzone))
            mouseHasMoved = true;
        // Apply yaw
        float yawInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(yawInput) > 0.001f)
            currentYaw += rotateSpeed * yawInput * deltaTime;
        // Only apply pitch/roll if mouse movement has started
        if (mouseHasMoved)
        {
            float pitch = rotateSpeed * mouseY * pitchScale * mouseSensitivity * deltaTime;
            currentPitch = Mathf.Clamp(currentPitch + pitch, minPitch, maxPitch);

            float roll = rotateSpeed * mouseX * rollScale * mouseSensitivity * deltaTime;
            currentRoll = clampRoll ? Mathf.Clamp(currentRoll + roll, minRoll, maxRoll) : currentRoll + roll;
        }
        // Build rotation matrix from pitch, yaw, roll (YXZ)
        CustomMatrix4x4 rotationMatrix = CustomMatrix4x4.CreateRotationXYZ(currentPitch, currentYaw, currentRoll);
        rotation = CustomQuaternion.FromMatrix4x4(rotationMatrix).Normalize();
        // Forward thrust movement
        float moveInput = Input.GetAxis("Vertical");
        float speed = Input.GetKey(KeyCode.Space) ? moveSpeed * boostMultiplier : moveSpeed;

        if (Mathf.Abs(moveInput) > 0.001f)
        {
            // Extract forward vector from rotation matrix (Z-axis)
            CustomVector3 forward = new CustomVector3(rotationMatrix.m[0, 2], rotationMatrix.m[1, 2], rotationMatrix.m[2, 2]).Normalize();
            position += forward * (speed * moveInput * deltaTime);
        }
        // Clamp position to world bounds (AABB)
        if (boundsObject != null)
        {
            CustomVector3 center = new CustomVector3(boundsObject.position.x, boundsObject.position.y, boundsObject.position.z);
            CustomVector3 halfExtents = new CustomVector3(boundsObject.localScale.x / 2f, boundsObject.localScale.y / 2f, boundsObject.localScale.z / 2f);
            CustomVector3 relative = position - center;

            position = new CustomVector3(
                Mathf.Clamp(relative.x, -halfExtents.x, halfExtents.x),
                Mathf.Clamp(relative.y, -halfExtents.y, halfExtents.y),
                Mathf.Clamp(relative.z, -halfExtents.z, halfExtents.z)
            ) + center;
        }
        // Combine translation and rotation matrices to build transform matrix
        CustomMatrix4x4 translationMatrix = CustomMatrix4x4.CreateTranslation(position.x, position.y, position.z);
        CustomMatrix4x4 fullMatrix = translationMatrix * rotationMatrix;
        // Apply to Unity transform
        transform.position = new CustomVector3(fullMatrix.m[0, 3], fullMatrix.m[1, 3], fullMatrix.m[2, 3]).ToUnityVector3();
        transform.rotation = rotation.ToUnityQuaternion();
    }

    /*
     * HandleEffects() applies boost-based visual distortion (lens effect) for user feedback.
    */
    private void HandleEffects(float deltaTime)
    {
        if (lensDistortion != null)
        {
            float targetIntensity = Input.GetKey(KeyCode.Space) ? -0.8f : 0f;
            lensDistortion.intensity.value = Mathf.Clamp(
                Mathf.Lerp(lensDistortion.intensity.value, targetIntensity, deltaTime * 5f),
                -1f, 0f
            );
        }
    }

    /*
     * HandleEscapeUnlock() unlocks mouse cursor when Escape key is pressed.
    */
    private void HandleEscapeUnlock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /*
     * CheckCollisions() checks for overlap with other CustomSphereColliders and triggers game over.
    */
    private void CheckCollisions()
    {
        // Returns if collider script is not on player
        if (myCollider == null)
            return;
        // Store detected collision target
        CustomSphereCollider hit = null;
        // Loops through all objects with a custom collider and checks if a collision occurs
        foreach (var other in CustomSphereCollider.All)
        {
            // If a collision has occurred, set hit to the other objects collider script
            if (other != myCollider && myCollider.IsCollidingWith(other))
            {
                hit = other;
                break; // Stop checking after the first collision
            }
        }
        // If there has been a collision
        if (hit != null)
        {
            Debug.Log($"Collided with {hit.name}");
            // Spawn VFX if assigned
            if (destroyVFX != null)
                Instantiate(destroyVFX, transform.position, Quaternion.identity);
            // Trigger game over
            GameManager.Instance.TriggerGameOver(5f);
            // Disable ship visuals + logic
            gameObject.SetActive(false);
        }
    }

    /*
     * OnDrawGizmos() draws the bounding box limits for the scene view.
     * - Useful for debugging.
    */
    void OnDrawGizmos()
    {
        if (boundsObject == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boundsObject.position, boundsObject.localScale);
    }
}
