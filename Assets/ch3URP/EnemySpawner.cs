using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("What to spawn")]
    public GameObject enemyPrefab;

    [Header("Where to spawn")]
    public Transform[] spawnPoints;
    public float randomRadius = 12f;

    [Header("Spawn height tweak")]
    public float yOffset = 1f; 

    public void SpawnTwo()
    {
        SpawnOne();
        SpawnOne();
    }

    private void SpawnOne()
    {
        Vector3 pos;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            pos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            pos.y += yOffset;
        }
        else
        {
            Vector2 r = Random.insideUnitCircle * randomRadius;
            pos = new Vector3(transform.position.x + r.x, yOffset, transform.position.z + r.y);
        }

        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }
}
