using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class trimove : MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed = 2.0f;           // Initial speed
    public float counter = 0.0f;              // Counter for speed increase
    public float maxSpeed = 14.0f;            // Maximum speed
    public float score = 0;                    // Score tracking
    public Transform teleportTarget;           // Target for teleportation
    public GameObject specificObjectToQuit;   // Object that quits the game on collision
    public GameObject ui;                      // UI element to show on game quit
    public string playerTag = "Player";       // Tag for the player GameObject
    public float randomRange = 5.0f;           // Range for randomizing teleport position
    public float randomizationFactor = 3.0f;   // Factor for randomizing speed after teleport
    private Score scoreScript;
    public TextMeshProUGUI gameOver;
    public float liftAmount = 2.0f;
    public float upsideDownChance = 30.0f;        // Chance (in percentage) for the triangle to turn upside down
    public float slowModifier;             // Initial slow modifier
    public float maxSlowModifier = 1.0f;          // Maximum slow modifier
    public float slowModifierIncreaseDuration = 20.0f; // Duration to increase slow modifier
    private float slowTime = 0.0f;                // Timer for how long the triangle has been slowed
    private bool isSlowModified = false;          // Flag to check if the triangle is currently slowed
    private bool isTeleporting = false;           // Flag to check if currently teleporting

    // New variables for teleportation effect
    public float teleportDuration = 1.0f;       // Duration for the triangle to speed up after teleport
    public float initialReducedSpeed = 15.0f;   // Speed to start at after teleporting

    void Start()
    {
        scoreScript = FindObjectOfType<Score>();
        if (scoreScript == null)
        {
            Debug.LogError("No Score script found in the scene.");
        }
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Constantly move to the left if not teleporting
        if (!isTeleporting)
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
        }

        // Check if the triangle is currently slowed
        if (isSlowModified)
        {
            // Increase slow time
            slowTime += Time.deltaTime;

            // Calculate the current slow modifier based on time spent in the slow state
            slowModifier = Mathf.Lerp(0.8f, maxSlowModifier, slowTime / slowModifierIncreaseDuration);
            
            // If the slow time exceeds the duration, reset the flag
            if (slowTime >= slowModifierIncreaseDuration)
            {
                isSlowModified = false;
                slowTime = 0; // Reset the timer
            }
        }

        // Check for restart input
        if (Time.timeScale == 0) // Only check input when the game is paused
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("restarted");
                Scene currentScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(currentScene.name);
                Time.timeScale = 1;
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Application.Quit();

                // If you are running in the Unity Editor, stop playing the game
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif

                Debug.Log("Game Quit"); // This will only show in the editor
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Teleport logic when hitting the teleport trigger
        if (other.CompareTag("TeleportTrigger"))
        {
            // Increment speed up to maxSpeed
            if (counter < maxSpeed)
            {
                counter++;
                moveSpeed += counter;
            }

            // Generate a random float value between maxSpeed and maxSpeed + randomizationFactor
            float randomSpeed = Random.Range(maxSpeed + 0.1f, maxSpeed + randomizationFactor + 1.1f);
            moveSpeed = randomSpeed; // Set the random speed
            
            // Generate a random float value between -randomRange and +randomRange
            float randomOffsetX = Random.Range(-randomRange, randomRange);

            // Set the new random teleport location
            Vector2 newTeleportPosition = new Vector2(teleportTarget.position.x + randomOffsetX, teleportTarget.position.y);
            
            // Start the teleportation coroutine
            StartCoroutine(TeleportAndMove(newTeleportPosition, randomSpeed));

            // Use the upsideDownChance to decide whether to turn upside down and float
            if (Random.Range(0, 100) < upsideDownChance)
            {
                // Rotate the triangle upside down (180 degrees on the Z axis)
                transform.rotation = Quaternion.Euler(0, 0, 180);

                // Lift the triangle by a specified amount (positive Y-axis)
                transform.position = new Vector2(transform.position.x, transform.position.y + liftAmount);

                // Disable gravity to make the object float
                rb.gravityScale = 0;

                // Slow down the movement speed by the slowModifier
                moveSpeed *= slowModifier;

                // Set the flag to indicate that the triangle is slowed
                isSlowModified = true;
            }
            else
            {
                // Ensure the triangle is upright and gravity is enabled if the chance doesn't trigger
                transform.rotation = Quaternion.Euler(0, 0, 0);

                // Enable gravity (assuming normal gravity scale is 1)
                rb.gravityScale = 1;

                // Reset the movement speed back to normal if it was slowed down
                moveSpeed = Mathf.Min(moveSpeed / slowModifier, maxSpeed); // Ensure it doesn't exceed max speed

                // Reset the slow modifier if not slowed
                slowModifier = 0.8f; // Reset to initial value
            }
        }

        // Check if the collider that entered the trigger is the player
        if (other.gameObject == specificObjectToQuit && other.CompareTag(playerTag))
        {
            ui.SetActive(true);
            gameOver.text = "Game Over!\nScore - " + scoreScript.score + "\nPress 'R' to Restart.";
            Time.timeScale = 0; 
            Debug.Log("Game Quit"); // This will only show in the editor, but not in a built version
        }
    }

    private IEnumerator TeleportAndMove(Vector2 newTeleportPosition, float targetSpeed)
    {
        // Set the triangle to the teleport position
        transform.position = newTeleportPosition;

        isTeleporting = true;

        float elapsedTime = 0f;
        float startSpeed = initialReducedSpeed;  // Start at reduced speed

        // Gradually increase the speed back to the target speed over the duration
        while (elapsedTime < teleportDuration)
        {
            // Calculate the current speed based on the elapsed time
            moveSpeed = Mathf.Lerp(startSpeed, targetSpeed, elapsedTime / teleportDuration); // Start slow, then speed up

            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y); // Update velocity
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the speed is set to the target speed after the duration
        moveSpeed = targetSpeed;
        rb.velocity = new Vector2(-moveSpeed, rb.velocity.y); // Final velocity update

        isTeleporting = false;
    }
}
