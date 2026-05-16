using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameEndManager : MonoBehaviour
{
    [Header("UI Text Displays")]
    public TMP_Text totalScoreText;
    public TMP_Text totalCoinText;

    void Start()
    {
        // 1. Restore normal time flow just in case
        Time.timeScale = 1f;

        // 2. Read the saved stats out of your MacBook's memory system
        int finalScore = PlayerPrefs.GetInt("TotalScore", 0);
        int finalCoins = PlayerPrefs.GetInt("TotalCoins", 0);

        // 3. Print them onto the screen
        if (totalScoreText != null) totalScoreText.text = "Final Score: " + finalScore;
        if (totalCoinText != null) totalCoinText.text = "Total Coins: " + finalCoins;
    }

    // Call this function when the Main Menu button is clicked!
    public void GoToMainMenu()
    {
        // Clear out the saved memory data so the next playthrough starts fresh
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("MainMenu");
    }
}