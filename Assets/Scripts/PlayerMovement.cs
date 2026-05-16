using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement; 
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public NavMeshAgent agent;
    public float speed = 6f;

    [Header("UI Text Fields")]
    public TMP_Text scoreText; 
    public TMP_Text coinText;  

    [Header("Mobile Pause Menu Elements")]
    public GameObject pausePanel; 

    [Header("Scoring Tracker System")]
    private int score = 0;
    private int coinsCollected = 0;
    private const int totalCoinsNeeded = 40;

    void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (agent != null)
        {
            agent.speed = speed;
        }

        Time.timeScale = 1f;
        UpdateGameUI();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(moveHorizontal, 0f, moveVertical).normalized;

        if (movementDirection.magnitude >= 0.1f)
        {
            Vector3 targetPosition = transform.position + movementDirection;
            
            if (agent != null)
            {
                agent.SetDestination(targetPosition);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
    }

    void UpdateGameUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
        if (coinText != null) coinText.text = "Coins: " + coinsCollected + " / " + totalCoinsNeeded;
    }

    public void PauseGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true); 
            Time.timeScale = 0f;        
        }
    }

    public void ResumeGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false); 
            Time.timeScale = 1f;         
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu"); 
    }

    void TriggerVictory()
    {
        Debug.Log("🏆 VICTORY! All 40 coins collected! 🏆");
    }
}