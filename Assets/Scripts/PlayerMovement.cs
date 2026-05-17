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

    void Start()
{
    if (Camera.main != null) 
    {
        camTransform = Camera.main.transform;
    }
    else
    {
        Debug.LogError("Ghost cannot find the Main Camera! Make sure your camera is tagged as 'MainCamera'.");
    }

    if (agent == null) agent = GetComponent<NavMeshAgent>();
    
    // RESET THESE TO DEFAULTS
    agent.updatePosition = true; 
    agent.updateRotation = true;
    agent.acceleration = 12f;
    agent.speed = speed;
}

void Update()
{
    if (isPaused || Time.timeScale == 0f) return;

    float moveHorizontal = Input.GetAxis("Horizontal");
    float moveVertical = Input.GetAxis("Vertical");

    if (moveVertical < 0) moveVertical = 0; // Keep your backward restriction

    Vector3 forward = camTransform.forward;
    Vector3 right = camTransform.right;
    forward.y = 0f;
    right.y = 0f;
    forward.Normalize();
    right.Normalize();

    Vector3 movementDirection = (forward * moveVertical + right * moveHorizontal).normalized;

    if (movementDirection.magnitude >= 0.1f)
    {
        // Use a fixed distance for the destination to keep it stable
        Vector3 targetPos = transform.position + movementDirection * 2f;
        agent.SetDestination(targetPos);
    }
    else
    {
        // If no input, tell the agent to stop immediately
        agent.ResetPath();
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