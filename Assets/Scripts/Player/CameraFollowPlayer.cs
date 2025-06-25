using UnityEngine;

/*
 * CameraFollowPlayer.cs
 * ----------------------
 * This script provides a dynamic third-person camera system that follows a player.
 *
 * Tasks:
 *  - Calculates the camera's ideal position behind and slightly above the player using vector math.
 *  - Smoothly interpolates the camera's position using `CustomVector3.Lerp`.
 *  - Orients the camera to always look in the direction the player is facing.
 *
 * Extras:
 *  - Uses vector normalization to extract forward and up directions of the player.
 *  - Applies linear interpolation to smooth the transition between current and desired camera positions.
 *  - Combines multiple vectors to compute the offset location from the player.
 *  - Executes in `LateUpdate()` to ensure it follows movement done in `Update()`.
 */

public class CameraFollowPlayer : MonoBehaviour
{
    [Header("References")]
    public Transform player;          // The player's transform (spaceship)

    [Header("Offset Settings")]
    public float distance = 5f;       // Distance behind the player along the forward vector
    public float height = 3f;         // Height above the player along the up vector

    [Header("Smoothing Settings")]
    public float smoothSpeed = 5f;    // Speed for smoothing movement using interpolation

    /*
     * Start() initialises components. 
    */
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    /*
     * LateUpdate() called at the end of every frame. 
     * - Handles camera position, orientation, and movement.
    */
    void LateUpdate()
    {
        if (player == null) return;
        // Extract directional vectors from the player object
        CustomVector3 playerPos = new CustomVector3(player.position.x, player.position.y, player.position.z);
        CustomVector3 forward = new CustomVector3(player.forward.x, player.forward.y, player.forward.z).Normalize();
        CustomVector3 up = new CustomVector3(player.up.x, player.up.y, player.up.z).Normalize();
        // Compute ideal camera position using vector math
        CustomVector3 desiredPos = playerPos - forward * distance + up * height;
        // Smooth move toward desired position using CustomVector3.Lerp
        CustomVector3 currentPos = new CustomVector3(transform.position.x, transform.position.y, transform.position.z);
        CustomVector3 smoothedPos = CustomVector3.Lerp(currentPos, desiredPos, smoothSpeed * Time.deltaTime);
        // Apply the final position
        transform.position = smoothedPos.ToUnityVector3();
        // Orient the camera to look slightly ahead of the player
        transform.LookAt(player.position + player.forward);
    }
}
