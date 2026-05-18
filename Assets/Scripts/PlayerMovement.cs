using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using Terresquall;
public class PlayerMovement : MonoBehaviour
{
    // Drag the Joystick from your Hierarchy into this slot in the Inspector
    // Change this line in your variables
    public VirtualJoystick variableJoystick;
    [Header("Movement Settings")]
    public NavMeshAgent agent;
    public float speed = 4f;

    [Header("Main Gameplay UI")]
    public TMP_Text scoreText;
    public TMP_Text coinText;

    [Header("Mobile Pause Menu Elements")]
    public GameObject pausePanel;
    public TMP_Text pauseMenuTitleText;

    [Header("Victory Screen Elements")]
    public GameObject victoryPanel;
    public TMP_Text finalScoreText;
    public TMP_Text finalCoinText;

    [Header("Scoring Tracker System")]
    private int score = 0;
    private int coinsCollected = 0;
    public int totalCoinsNeeded = 3;

    private bool isPaused = false;
    private Transform camTransform; // Added to store camera reference

    private Animator anim;
    private float idleTimer = 0f;
    private float idleInterval = 3f;

    void Start()
    {
        anim = GetComponent<Animator>();
        // 1. Setup Camera Reference
        if (Camera.main != null) camTransform = Camera.main.transform;

        // 2. Handle Level Scoring Logic
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Level 1")
        {
            PlayerPrefs.DeleteAll();
            score = 0;
            coinsCollected = 0;
        }
        else if (currentScene == "Level 2")
        {
            score = PlayerPrefs.GetInt("TotalScore", 0);
            coinsCollected = 0;
        }

        // 3. Reset Agent to standard (Stair-friendly) mode
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = speed;
            agent.updatePosition = true; // MUST be true for stairs!
            agent.updateRotation = true;
            agent.acceleration = 12f; // Back to a stable value
        }

        Time.timeScale = 1f;
        isPaused = false;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        UpdateGameUI();
    }

    void Update()
    {
        // 1. Pause Check
        if (isPaused || Time.timeScale == 0f)
        {
            if (agent != null && agent.isOnNavMesh) agent.ResetPath();
            return;
        }

        // 2. Animation Logic (Movement Detection)
        // We use agent.velocity to see if the ghost is actually sliding
        bool moving = agent.velocity.magnitude > 0.1f;
        anim.SetBool("isMoving", moving);

        if (moving)
        {
            idleTimer = 0f;
        }
        else
        {
            // Only count up for the idle breath if we are standing in the 'Stationary' state
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Stationary"))
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= 3.0f)
                {
                    anim.SetTrigger("PlayIdle");
                    idleTimer = 0f;
                }
            }
        }

        // Inside your Update function where you get joystick input:
float moveHorizontal = VirtualJoystick.GetAxis("Horizontal") * 0.7f; // Reduces sensitivity by 30%
float moveVertical = VirtualJoystick.GetAxis("Vertical") * 0.7f;

if (Mathf.Abs(moveHorizontal) > 0.1f || Mathf.Abs(moveVertical) > 0.1f)
    {
        anim.SetBool("isRunning", true);
    }
    else
    {
        anim.SetBool("isRunning", false);
    }
    
        // Prevent Backward Movement (as requested)
        if (moveVertical < 0) moveVertical = 0;

        // 4. Calculate Direction relative to Camera
        Vector3 forward = camTransform.forward;
        Vector3 right = camTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 movementDirection = (forward * moveVertical + right * moveHorizontal).normalized;

        // 5. Execution (The Smooth Glide Fix)
        // 5. Execution (The Smooth Glide Fix)
        if (movementDirection.magnitude >= 0.1f)
        {
            Vector3 desiredMove = movementDirection * speed * Time.deltaTime;
            agent.Move(desiredMove);

            // --- SMOOTH ROTATION FIX ---
            // 10f is the rotation speed. 
            // Lower this number (e.g., 5f) to make turns even slower/wider.
            // Higher this number (e.g., 20f) to make turns sharper.
            float rotationSpeed = 3f;

            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // If no keys are pressed, clear the velocity to stop sliding instantly
            if (agent.isOnNavMesh)
            {
                agent.velocity = Vector3.zero;
                agent.ResetPath();
            }
        }
    }

    public void OnCoinCollected()
    {
        score += 10;
        coinsCollected++;
        UpdateGameUI();

        if (coinsCollected >= totalCoinsNeeded)
        {
            TriggerVictory();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the thing we hit is tagged 'Enemy'
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("💀 GHOST WAS HIT! 💀");
            TriggerGameOver();
        }
    }

    // Add this as a backup in case the physics setup 
    // prefers a standard collision over a trigger
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("💀 GHOST COLLIDED! 💀");
            TriggerGameOver();
        }
    }

    void UpdateGameUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
        if (coinText != null) coinText.text = "Coins: " + coinsCollected + " / " + totalCoinsNeeded;
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseMenuTitleText != null) pauseMenuTitleText.text = "PAUSED";
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    void TriggerGameOver()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseMenuTitleText != null) pauseMenuTitleText.text = "GAME OVER";
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            Transform resumeBtn = pausePanel.transform.Find("ResumeButton");
            if (resumeBtn != null) resumeBtn.gameObject.SetActive(false);
        }
    }

    void TriggerVictory()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (finalScoreText != null) finalScoreText.text = "Final Score: " + score;
        if (finalCoinText != null) finalCoinText.text = "Coins Collected: " + coinsCollected + " / " + totalCoinsNeeded;

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Debug.Log("🎯 VICTORY PANEL ACTIVATED SUCCESSFULLY!");
        }

        StartCoroutine(WaitAndLoadNextLevel());
    }

    IEnumerator WaitAndLoadNextLevel()
    {
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;

        if (SceneManager.GetActiveScene().name == "Level 2")
        {
            PlayerPrefs.SetInt("TotalScore", score);
            PlayerPrefs.SetInt("TotalCoins", coinsCollected);
            PlayerPrefs.Save();
            SceneManager.LoadScene("GameEnd");
        }
        else
        {
            PlayerPrefs.SetInt("TotalScore", score);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Level 2");
        }
    }
}