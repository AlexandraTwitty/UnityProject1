using UnityEngine;

public class GameOverBounce : MonoBehaviour
{
    [Header("Movement Settings")]
    public float horizontalSpeed = 200f;
    public float verticalSpeed = 150f;

    private RectTransform rectTransform;
    private RectTransform canvasRect;
    private Vector2 velocity;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Canvas canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        
        // Set initial velocity
        velocity = new Vector2(horizontalSpeed, verticalSpeed);
    }

    void Update()
    {
        // Move the text
        rectTransform.anchoredPosition += velocity * Time.deltaTime;

        // Get canvas dimensions
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        // Get text dimensions (accounting for scale)
        float textWidth = rectTransform.rect.width * rectTransform.localScale.x;
        float textHeight = rectTransform.rect.height * rectTransform.localScale.y;

        // Calculate boundaries
        float halfWidth = textWidth / 2f;
        float halfHeight = textHeight / 2f;

        float leftBound = -canvasWidth / 2f + halfWidth;
        float rightBound = canvasWidth / 2f - halfWidth;
        float bottomBound = -canvasHeight / 2f + halfHeight;
        float topBound = canvasHeight / 2f - halfHeight;

        // Get current position
        Vector2 pos = rectTransform.anchoredPosition;

        // Check horizontal bounds and bounce
        if (pos.x <= leftBound)
        {
            velocity.x = Mathf.Abs(velocity.x); // Force positive (move right)
            pos.x = leftBound;
        }
        else if (pos.x >= rightBound)
        {
            velocity.x = -Mathf.Abs(velocity.x); // Force negative (move left)
            pos.x = rightBound;
        }

        // Check vertical bounds and bounce
        if (pos.y <= bottomBound)
        {
            velocity.y = Mathf.Abs(velocity.y); // Force positive (move up)
            pos.y = bottomBound;
        }
        else if (pos.y >= topBound)
        {
            velocity.y = -Mathf.Abs(velocity.y); // Force negative (move down)
            pos.y = topBound;
        }

        // Apply corrected position
        rectTransform.anchoredPosition = pos;
    }
}