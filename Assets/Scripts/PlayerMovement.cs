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

    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Level 1")
        {
            PlayerPrefs.DeleteAll(); 
            score = 0;
            coinsCollected = 0;
            Debug.Log("🧹 Fresh game started in Level 1! Scores reset to 0.");
        }
        else if (currentScene == "Level 2")
        {
            score = PlayerPrefs.GetInt("TotalScore", 0);
            coinsCollected = 0; 
            Debug.Log("📦 Level 2 Loaded! Carrying over score: " + score);
        }

        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.speed = speed;

        Time.timeScale = 1f; 
        isPaused = false;
        
        if (pausePanel != null) pausePanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false); 
        
        UpdateGameUI();
    }

    void Update()
    {
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
            
            if (agent != null && agent.isOnNavMesh && agent.isActiveAndEnabled)
            {
                agent.SetDestination(targetPosition);
            }
        }
    }

    // This public function is what the CoinCollector scripts will trigger!
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
        // Handle Enemy crash loops
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