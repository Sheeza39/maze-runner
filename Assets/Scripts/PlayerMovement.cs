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
    public int totalCoinsNeeded = 3; // Changeable in the inspector layout

    private bool isPaused = false;

    void Start()
    {
        // 1. Check current scene name to handle score memory states correctly
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Level 1")
        {
            // Reset cache fully whenever starting a fresh playthrough
            PlayerPrefs.DeleteAll(); 
            score = 0;
            coinsCollected = 0;
            Debug.Log("🧹 Fresh game started in Level 1! Scores reset to 0.");
        }
        else if (currentScene == "Level 2")
        {
            // Load cumulative score carried over from the end of Level 1
            score = PlayerPrefs.GetInt("TotalScore", 0);
            coinsCollected = 0; // Reset specific coin count for this level map layout
            Debug.Log("📦 Level 2 Loaded! Carrying over score: " + score);
        }

        // 2. Set up NavMeshAgent linkages safely
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.speed = speed;

        // 3. Keep the layout system clean and ensure time flows normally
        Time.timeScale = 1f; 
        isPaused = false;
        
        if (pausePanel != null) pausePanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false); 
        
        UpdateGameUI();
    }

    void Update()
    {
        // Safety guard: Don't calculate paths if paused or if time is frozen
        if (isPaused || Time.timeScale == 0f) 
        {
            if (agent != null && agent.hasPath) agent.ResetPath(); 
            return; 
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(moveHorizontal, 0f, moveVertical).normalized;

        if (movementDirection.magnitude >= 0.1f)
        {
            Vector3 targetPosition = transform.position + movementDirection;
            
            // Safety guard fix for the NavMesh crash error:
            if (agent != null && agent.isOnNavMesh && agent.isActiveAndEnabled)
            {
                agent.SetDestination(targetPosition);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Handle Coin picking up triggers
        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            score += 10;
            coinsCollected++;
            UpdateGameUI();

            if (coinsCollected >= totalCoinsNeeded)
            {
                TriggerVictory();
            }
        }

        // Handle Enemy crash game over loops
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
        Time.timeScale = 0f; // Freeze game actions instantly!

        // 1. Push current calculated runtime stats to victory text components
        if (finalScoreText != null) finalScoreText.text = "Final Score: " + score;
        if (finalCoinText != null) finalCoinText.text = "Coins Collected: " + coinsCollected + " / " + totalCoinsNeeded;

        // 2. Turn the Victory Panel layout fully ON
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Debug.Log("🎯 VICTORY PANEL ACTIVATED SUCCESSFULLY!");
        }

        // 3. Fire real-time countdown timer to bridge scene swapping
        StartCoroutine(WaitAndLoadNextLevel());
    }

    IEnumerator WaitAndLoadNextLevel()
    {
        // Wait 3 seconds using actual real-time wristwatch values
        yield return new WaitForSecondsRealtime(3f); 
        
        Time.timeScale = 1f; // Re-align core time speed flags

        // Check if the current level finishing up is Level 2
        if (SceneManager.GetActiveScene().name == "Level 2")
        {
            // Save final values to cache memory references
            PlayerPrefs.SetInt("TotalScore", score);
            PlayerPrefs.SetInt("TotalCoins", coinsCollected);
            PlayerPrefs.Save(); 

            Debug.Log("🏁 Game Complete! Loading final GameEnd scene.");
            SceneManager.LoadScene("GameEnd"); 
        }
        else
        {
            // Otherwise save standard step score markers and launch Level 2
            PlayerPrefs.SetInt("TotalScore", score);
            PlayerPrefs.Save();

            Debug.Log("🚀 Level 1 Complete! Launching Level 2.");
            SceneManager.LoadScene("Level 2"); 
        }
    }
}