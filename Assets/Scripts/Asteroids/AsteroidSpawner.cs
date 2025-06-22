using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject[] asteroidPrefabs;
    public float spawnRate = 2f;
    public Transform boundsObject;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            SpawnAsteroid();
            timer = 0f;
        }
    }

    void SpawnAsteroid()
    {
        if (boundsObject == null || asteroidPrefabs.Length == 0)
            return;

        // Randomly pick one of the 6 faces of the bounds
        int face = Random.Range(0, 6); // 0=+X, 1=-X, 2=+Y, 3=-Y, 4=+Z, 5=-Z

        Vector3 center = boundsObject.position;
        Vector3 half = boundsObject.localScale / 2f;

        Vector3 offset = Vector3.zero;
        Vector3 axisOffset = Vector3.zero;

        switch (face)
        {
            case 0: axisOffset = Vector3.right * (half.x + 10f); break;     // +X
            case 1: axisOffset = Vector3.left * (half.x + 10f); break;     // -X
            case 2: axisOffset = Vector3.up * (half.y + 10f); break;     // +Y
            case 3: axisOffset = Vector3.down * (half.y + 10f); break;     // -Y
            case 4: axisOffset = Vector3.forward * (half.z + 10f); break;   // +Z
            case 5: axisOffset = Vector3.back * (half.z + 10f); break;   // -Z
        }

        // Random offset along the two other axes (tangential to face)
        if (face < 2) offset = new Vector3(0, Random.Range(-half.y, half.y), Random.Range(-half.z, half.z));     // X faces
        else if (face < 4) offset = new Vector3(Random.Range(-half.x, half.x), 0, Random.Range(-half.z, half.z)); // Y faces
        else offset = new Vector3(Random.Range(-half.x, half.x), Random.Range(-half.y, half.y), 0);              // Z faces

        Vector3 spawnPos = center + axisOffset + offset;

        int prefabIndex = Random.Range(0, asteroidPrefabs.Length);
        Instantiate(asteroidPrefabs[prefabIndex], spawnPos, Quaternion.identity);
    }
}
