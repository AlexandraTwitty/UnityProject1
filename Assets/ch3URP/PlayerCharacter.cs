using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerCharacter : MonoBehaviour
{
    private int _health;

    [Header("UI References")]
    public TextMeshProUGUI healthText;
    public GameObject gameOverText;
    public GameObject blackBackground;

    [Header("Audio")]
    public AudioClip hurtSound;      
    public AudioClip gameOverSound;  
    private AudioSource audioSource; 

    private bool isGameOver = false;

    void Start()
    {
        _health = 2;
        UpdateHealthDisplay();

        audioSource = GetComponent<AudioSource>();

        if (gameOverText != null)
            gameOverText.SetActive(false);

        if (blackBackground != null)
            blackBackground.SetActive(false);
    }

    public void Hurt(int damage)
    {
        if (isGameOver) return;

        // Play hurt sound
        if (hurtSound != null)
        {
            if (audioSource != null)
                audioSource.PlayOneShot(hurtSound);
            else
                AudioSource.PlayClipAtPoint(hurtSound, transform.position);
        }

        _health -= damage;
        Debug.Log("Health: " + _health);

        UpdateHealthDisplay();

        if (_health <= 0)
            Die();
    }

    void UpdateHealthDisplay()
    {
        if (healthText == null) return;

        string healthString = "Health: ";
        for (int i = 0; i < _health; i++)
            healthString += "* ";

        healthText.text = healthString;
    }

    void Die()
    {
        Debug.Log("Game Over!");
        isGameOver = true;

        // Optional game over sound
        if (gameOverSound != null)
        {
            if (audioSource != null)
                audioSource.PlayOneShot(gameOverSound);
            else
                AudioSource.PlayClipAtPoint(gameOverSound, transform.position);
        }

        if (blackBackground != null)
            blackBackground.SetActive(true);

        if (gameOverText != null)
            gameOverText.SetActive(true);

        FPSInput fpsInput = GetComponent<FPSInput>();
        if (fpsInput != null)
            fpsInput.enabled = false;

        MouseLook mouseLook = GetComponent<MouseLook>();
        if (mouseLook != null)
            mouseLook.enabled = false;
    }

    void Update()
    {
        // Test key
        if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
            Hurt(1);
    }
}
