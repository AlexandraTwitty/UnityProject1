using UnityEngine;
using System.Collections;

public class ReactiveTarget : MonoBehaviour
{
    [Header("Death Effects")]
    public GameObject tombstonePrefab;
    public GameObject deathParticlesPrefab;

    [Header("Audio")]
    public AudioClip hitSound;          
    private AudioSource audioSource;    

    private bool isDying = false;
    private bool rotating = false;
    private float rotatedSoFar = 0f;

    private const float totalDegrees = 90f;
    private const float durationSeconds = 1f;
    private float degreesPerSecond = totalDegrees / durationSeconds;

    private EnemySpawner spawner;
    private Animator _animator;

    void Start()
    {
        spawner = Object.FindFirstObjectByType<EnemySpawner>();
        _animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void ReactToHit()
    {
        if (isDying) return;
        isDying = true;

        // Play hit sound immediately
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position, 1f);
        }


        // Spawn explosion on hit
        if (deathParticlesPrefab != null)
        {
            Vector3 explosionPos = transform.position;
            Instantiate(deathParticlesPrefab, explosionPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("deathParticlesPrefab is not assigned!");
        }

        // Stop enemy movement
        WanderingAI behavior = GetComponent<WanderingAI>();
        if (behavior != null)
        {
            behavior.SetAlive(false);
        }

        // Trigger death animation
        if (_animator != null)
        {
            _animator.SetBool("isDead", true);
        }

        // Start rotation
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

        // Rotate (fall)
        transform.Rotate(-step, 0f, 0f, Space.Self);

        // Snap to ground so it doesn't "float" while tipping
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            float bottomY = col.bounds.min.y;
            float delta = 0f - bottomY; // ground is y=0
            transform.position += new Vector3(0f, delta, 0f);
        }

        rotatedSoFar += step;
        if (rotatedSoFar >= totalDegrees)
            rotating = false;
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(1.5f);

        // Spawn tombstone
        if (tombstonePrefab != null)
        {
            Vector3 tombstonePos = transform.position;
            tombstonePos.y = 0f;
            Instantiate(tombstonePrefab, tombstonePos, Quaternion.identity);
        }

        // Destroy enemy
        Destroy(gameObject);

        // Spawn next enemies (next frame)
        if (spawner != null)
            spawner.StartCoroutine(SpawnNextFrame(spawner));
    }

    private IEnumerator SpawnNextFrame(EnemySpawner spawner)
    {
        yield return null;
        spawner.SpawnTwo();
    }
}
