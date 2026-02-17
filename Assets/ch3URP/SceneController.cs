using UnityEngine;

public class SceneController : MonoBehaviour {
    [SerializeField] private GameObject enemyPrefab;
    private bool hasSpawnedInitialEnemy = false;
    
    void Start() {
        if (!hasSpawnedInitialEnemy) {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.transform.position = new Vector3(0, 1, 0);
            float angle = Random.Range(0, 360);
            enemy.transform.Rotate(0, angle, 0);
            hasSpawnedInitialEnemy = true;
        }
    }
}