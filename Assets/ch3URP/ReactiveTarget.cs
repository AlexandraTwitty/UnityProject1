using UnityEngine;
using System.Collections;

public class ReactiveTarget : MonoBehaviour
{
    [Header("Death Effects")]
    public GameObject tombstonePrefab;

    private bool isDying = false;
    private bool rotating = false;
    private float rotatedSoFar = 0f;

    private const float totalDegrees = 90f;
    private const float durationSeconds = 1f;
    private float degreesPerSecond = totalDegrees / durationSeconds;

    private EnemySpawner spawner;
    public GameObject deathParticlesPrefab;
    private Animator _animator; 

    void Start()
    {
        spawner = Object.FindFirstObjectByType<EnemySpawner>();
        _animator = GetComponent<Animator>(); 
    }

    public void ReactToHit()
    {
        if (isDying) return;
        isDying = true;

        // Spawn explosion on hit
        if (deathParticlesPrefab != null)
        {
            Vector3 explosionPos = transform.position;
            
            GameObject particles = Instantiate(deathParticlesPrefab, explosionPos, Quaternion.identity);
            Debug.Log($"Explosion spawned at {explosionPos}");
        }
        else
        {
            Debug.LogWarning("deathParticlesPrefab is not assigned!");
        }

        WanderingAI behavior = GetComponent<WanderingAI>();
        if (behavior != null)
        {
            behavior.SetAlive(false);
        }

        // Trigger death animation
        if (_animator != null) {
            _animator.SetBool("isDead", true);
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

        transform.Rotate(-step, 0f, 0f, Space.Self);

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            float bottomY = col.bounds.min.y;
            float delta = 0f - bottomY;
            transform.position += new Vector3(0f, delta, 0f);
        }

        rotatedSoFar += step;
        if (rotatedSoFar >= totalDegrees)
            rotating = false;
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(1.5f);

        //Spawn tombstone
        if (tombstonePrefab != null)
        {
            Vector3 tombstonePos = transform.position;
            tombstonePos.y = 0;
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