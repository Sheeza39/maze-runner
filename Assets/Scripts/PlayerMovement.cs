using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement; 
using System.Collections; 
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public NavMeshAgent agent;
    public float speed = 6f;

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
    if (isPaused) return;

    bool moving = agent.velocity.magnitude > 0.1f;
    anim.SetBool("isMoving", moving);

    if (moving)
{
    idleTimer = 0f;
    anim.SetBool("isMoving", true);
}
else
{
    anim.SetBool("isMoving", false);
    
    // Only count up if we are NOT currently playing the animation
    // We check if the Animator is in the 'Stationary' state
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
    if (isPaused || Time.timeScale == 0f) 
    {
        if (agent != null && agent.hasPath) agent.ResetPath();
        return; 
    }

    float moveHorizontal = Input.GetAxis("Horizontal");
    float moveVertical = Input.GetAxis("Vertical");

    // Prevent Backward Movement
    if (moveVertical < 0) moveVertical = 0; 

    // Calculate direction relative to camera
    Vector3 forward = camTransform.forward;
    Vector3 right = camTransform.right;
    forward.y = 0f;
    right.y = 0f;
    forward.Normalize();
    right.Normalize();

    Vector3 movementDirection = (forward * moveVertical + right * moveHorizontal).normalized;

    if (movementDirection.magnitude >= 0.1f)
    {
        Vector3 potentialTarget = transform.position + movementDirection * 1.2f;
        NavMeshHit hit;

        // This checks the NavMesh for the nearest valid point (like a step)
        if (NavMesh.SamplePosition(potentialTarget, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
    else
    {
        // Stop movement if no keys are pressed
        if (agent != null && agent.isOnNavMesh) agent.ResetPath();
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
        if (other.gameObject.CompareTag("Enemy"))
        {
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