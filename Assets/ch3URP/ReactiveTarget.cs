using UnityEngine;
using System.Collections;

public class ReactiveTarget : MonoBehaviour
{
    [Header("Death Effects")]
    public GameObject tombstonePrefab; // Assign in Inspector!

    private bool isDying = false;

    // rotation
    private bool rotating = false;
    private float rotatedSoFar = 0f;

    private const float totalDegrees = 90f;
    private const float durationSeconds = 1f;
    private float degreesPerSecond = totalDegrees / durationSeconds;

    private EnemySpawner spawner; // Cache reference

    void Start()
    {
        spawner = Object.FindFirstObjectByType<EnemySpawner>();
    }

    public void ReactToHit()
    {
        // Prevent double-trigger
        if (isDying) return;
        isDying = true;

        WanderingAI behavior = GetComponent<WanderingAI>();
        if (behavior != null)
        {
            behavior.SetAlive(false);
        }

        rotating = true;
        rotatedSoFar = 0f;

        StartCoroutine(Die());
    }

    void Update()
{
    if (!rotating) return;

    float step = degreesPerSecond * Time.deltaTime;
    if (rotatedSoFar + step >= totalDegrees)
        step = totalDegrees - rotatedSoFar;

    // Rotate (fall forward around local X axis)
    transform.Rotate(-step, 0f, 0f, Space.Self);

    // After rotating, drop it so its collider bottom sits on the ground (y=0)
    Collider col = GetComponent<Collider>();
    if (col != null)
    {
        float bottomY = col.bounds.min.y;   // current lowest point in world space
        float delta = 0f - bottomY;         // how far off the floor we are
        transform.position += new Vector3(0f, delta, 0f);
    }

    rotatedSoFar += step;
    if (rotatedSoFar >= totalDegrees)
        rotating = false;
}



    private IEnumerator Die()
    {
        yield return new WaitForSeconds(1.5f);

        // Spawn tombstone at enemy's position
        if (tombstonePrefab != null)
        {
            Vector3 tombstonePos = transform.position;
            tombstonePos.y = 0; // Place on ground
            Instantiate(tombstonePrefab, tombstonePos, Quaternion.identity);
        }

        Destroy(gameObject);

        if (spawner != null)
            spawner.StartCoroutine(SpawnNextFrame(spawner));
    }

    private IEnumerator SpawnNextFrame(EnemySpawner spawner)
    {
        yield return null;
        spawner.SpawnTwo();
    }
}