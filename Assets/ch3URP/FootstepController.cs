using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class FootstepController : MonoBehaviour
{
    [Header("Footstep Settings")]
    [SerializeField] private AudioClip[] footstepSounds; // Array of footstep sounds
    [SerializeField] private float footstepInterval = 0.5f; // Time between steps
    [SerializeField] private float volumeMin = 0.8f;
    [SerializeField] private float volumeMax = 1.0f;
    [SerializeField] private float pitchMin = 0.9f;
    [SerializeField] private float pitchMax = 1.1f;
    
    private AudioSource audioSource;
    private CharacterController characterController;
    private float footstepTimer = 0f;
    
    void Start()
    {
        // Get or create AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound (first-person)
        
        // Get CharacterController
        characterController = GetComponent<CharacterController>();
    }
    
    void Update()
    {
        // Check if player is moving
        bool isMoving = IsPlayerMoving();
        
        if (isMoving && characterController.isGrounded)
        {
            footstepTimer -= Time.deltaTime;
            
            if (footstepTimer <= 0f)
            {
                PlayFootstep();
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            // Reset timer when not moving
            footstepTimer = 0f;
        }
    }
    
    bool IsPlayerMoving()
    {
#if ENABLE_INPUT_SYSTEM
        // New Input System
        Vector2 movement = Vector2.zero;
        
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) movement.y += 1;
            if (Keyboard.current.sKey.isPressed) movement.y -= 1;
            if (Keyboard.current.aKey.isPressed) movement.x -= 1;
            if (Keyboard.current.dKey.isPressed) movement.x += 1;
        }
        
        return movement.magnitude > 0.1f;
#else
        // Old Input System
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        return Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;
#endif
    }
    
    void PlayFootstep()
    {
        if (footstepSounds.Length == 0) return;
        
        // Pick random footstep sound
        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        
        // Randomize volume and pitch for variety
        audioSource.volume = Random.Range(volumeMin, volumeMax);
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        
        // Play sound
        audioSource.PlayOneShot(clip);
    }
}