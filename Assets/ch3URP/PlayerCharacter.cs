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

    private bool isGameOver = false;

    void Start() 
    {
        _health = 2;
        UpdateHealthDisplay();

        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
        }

        if (blackBackground != null)
        {
            blackBackground.SetActive(false);
        }
    }

    public void Hurt(int damage) 
    {
        if (isGameOver) return;

        _health -= damage;
        Debug.Log("Health: " + _health);
        
        UpdateHealthDisplay();

        if (_health <= 0)
        {
            Die();
        }
    }

    void UpdateHealthDisplay()
    {
        if (healthText == null) return;

        string healthString = "Health: ";
        
        for (int i = 0; i < _health; i++)
        {
            healthString += "* ";
        }

        healthText.text = healthString;
    }

    void Die()
    {
        Debug.Log("Game Over!");
        isGameOver = true;

        if (blackBackground != null)
        {
            blackBackground.SetActive(true);
        }

        if (gameOverText != null)
        {
            gameOverText.SetActive(true);
        }

        FPSInput fpsInput = GetComponent<FPSInput>();
        if (fpsInput != null)
        {
            fpsInput.enabled = false;
        }

        MouseLook mouseLook = GetComponent<MouseLook>();
        if (mouseLook != null)
        {
            mouseLook.enabled = false;
        }
    }

    void Update()
    {
        // Check if keyboard exists and H key was pressed
        if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
        {
            Hurt(1);
        }
    }
}