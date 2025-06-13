using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform player;
    public float distance = 5f;
    public float height = 3f;
    public float smoothSpeed = 5f;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        // Get ship's orientation vectors
        CustomVector3 playerPos = new CustomVector3(player.position.x, player.position.y, player.position.z);
        CustomVector3 forward = new CustomVector3(player.forward.x, player.forward.y, player.forward.z).Normalize();
        CustomVector3 up = new CustomVector3(player.up.x, player.up.y, player.up.z).Normalize();

        // Desired camera position: behind the ship and above it
        CustomVector3 desiredPos = playerPos - forward * distance + up * height;

        // Smooth move toward desired position using CustomVector3.Lerp
        CustomVector3 currentPos = new CustomVector3(transform.position.x, transform.position.y, transform.position.z);
        CustomVector3 smoothedPos = CustomVector3.Lerp(currentPos, desiredPos, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPos.ToUnityVector3();

        // Look in same direction as ship is facing → look slightly ahead of ship
        transform.LookAt(player.position + player.forward);
    }
}
