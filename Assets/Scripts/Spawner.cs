using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawnA;
    public GameObject objectToSpawnB;
    public float spawnInterval = 2f;
    public float moveRange = 5f;
    public float initialSpeed = 2f;

    private float speed;
    private float direction = 1f;
    private float timer;
    private Vector3 startPos;
    private int spawnType;

    void Start()
    {
        speed = initialSpeed;
        timer = spawnInterval;
        startPos = transform.position;
    }

    void Update()
    {
        // Move left and right
        transform.Translate(Vector3.right * speed * direction * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startPos.x) >= moveRange)
        {
            direction *= -1;
        }

        // Countdown to spawn
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // Randomize spawn type (0 or 1)
            spawnType = Random.Range(0, 2);
            SpawnObject(spawnType);
            timer = spawnInterval;
        }
    }

    void SpawnObject(int type)
    {
        if (type == 0)
        {
            Instantiate(objectToSpawnA, transform.position, Quaternion.identity);
        }
        else if (type == 1)
        {
            Instantiate(objectToSpawnB, transform.position, Quaternion.identity);
        }

        speed += 1f;
    }
}
