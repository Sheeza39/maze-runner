using UnityEngine;
using UnityEngine.SceneManagement; // Crucial for switching scenes

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        // This instantly triggers the transition to your level map.
        // Ensure your gameplay scene file is named EXACTLY "Level 1" in your folders!
        SceneManager.LoadScene("Level 1"); 
    }
}