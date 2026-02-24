using UnityEngine;
using System.Collections;

public class WanderingAI : MonoBehaviour {
    public float speed = 3.0f;
    public float obstacleRange = 5.0f;
    
    [SerializeField] private GameObject fireballPrefab;
    private GameObject _fireball;
    
    private bool _alive;
    private Animator _animator; 
    
    void Start() {
        _alive = true;
        _animator = GetComponent<Animator>(); 
    }
    
    void Update() {
        if (_alive) {
            transform.Translate(0, 0, speed * Time.deltaTime);
            
            // Set isMoving parameter to true when alive and moving
            if (_animator != null) {
                _animator.SetBool("isMoving", true);
            }
            
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.SphereCast(ray, 0.75f, out hit)) {
                GameObject hitObject = hit.transform.gameObject;
                if (hitObject.GetComponent<PlayerCharacter>()) {
                    if (_fireball == null) {
                        // Trigger attack animation
                        if (_animator != null) {
                            _animator.SetTrigger("attack");
                        }
                        
                        _fireball = Instantiate(fireballPrefab) as GameObject;
                        _fireball.transform.position = transform.TransformPoint(Vector3.forward * 1.5f);
                        _fireball.transform.rotation = transform.rotation;
                    }
                }
                else if (hit.distance < obstacleRange) {
                    float angle = Random.Range(-110, 110);
                    transform.Rotate(0, angle, 0);
                }
            }
        }
        else {
            // Stop moving animation when dead
            if (_animator != null) {
                _animator.SetBool("isMoving", false);
            }
        }
    }

    public void SetAlive(bool alive) {
        _alive = alive;
    }
}