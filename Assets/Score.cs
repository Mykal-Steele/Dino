using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import the TextMeshPro namespace

public class Score : MonoBehaviour
{
    public GameObject triangle;  // Reference to the triangle GameObject
    public Vector3 offset = new Vector3(0, 1.0f, 0);  // The offset to keep the score above the triangle
    public int score = 0;
    public GameObject trigger;  // The object that triggers score increment

    public TextMeshProUGUI scoreText; // Reference to the TextMeshProUGUI component for displaying score
    public Camera mainCamera;         // Reference to the main camera for changing background color

    // Define the colors you want to switch between (as hex codes converted to Color)
    private Color color1 = new Color32(0xB4, 0xD0, 0xFD, 0xFF);  // Hex #B4D0FD
    private Color color2 = new Color32(0x31, 0x41, 0x59, 0xFF);  // Hex #314159

    // Variable to store the target color for the transition
    private Color targetColor;
    // Transition speed
    public float transitionSpeed = 1.0f;

    void Start()
    {
        // Initialize the target color to the current background color
        targetColor = mainCamera.backgroundColor;
    }

    void Update()
    {
        // Always position the score object above the triangle
        if (triangle != null)
        {
            // Update the score object's position to be above the triangle
            transform.position = triangle.transform.position + offset;
        }

        // Gradually change the background color towards the target color
        mainCamera.backgroundColor = Color.Lerp(mainCamera.backgroundColor, targetColor, transitionSpeed * Time.deltaTime);

        // Update the score text
        UpdateScoreText(); // Call this function to update the score text
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Increment the score when the trigger is hit
        if (other.gameObject == trigger)
        {
            score++;
            UpdateScoreText(); // Update the text when score is incremented

            // Check if the score is a multiple of 10
            if (score % 10 == 0)
            {
                Debug.Log("Score reached: " + score); // Log the score
                ToggleTargetColor(); // Change the target color every 10 points
            }
        }
    }

    private void UpdateScoreText() // Function to update the score display
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score; // Update the text with the current score
        }
    }

    private void ToggleTargetColor() // Function to set the target color
    {
        // Set the target color to the opposite color
        if (IsColorEqual(targetColor, color1))
        {
            Debug.Log("Setting target color to color2"); // Log color change
            targetColor = color2; // Switch to color2 (Hex #314159)
        }
        else
        {
            Debug.Log("Setting target color to color1"); // Log color change
            targetColor = color1; // Switch back to color1 (Hex #B4D0FD)
        }
    }

    private bool IsColorEqual(Color colorA, Color colorB, float tolerance = 0.01f)
    {
        return Mathf.Abs(colorA.r - colorB.r) < tolerance &&
               Mathf.Abs(colorA.g - colorB.g) < tolerance &&
               Mathf.Abs(colorA.b - colorB.b) < tolerance;
    }
}
