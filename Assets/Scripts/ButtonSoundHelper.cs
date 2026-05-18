using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundHelper : MonoBehaviour
{
    public void PlayClick()
    {
        // Search the whole game for the traveler object
        GameObject musicObj = GameObject.Find("BackgroundMusic");

        if (musicObj != null)
        {
            // Get the second AudioSource (the one for clicks)
            AudioSource[] sources = musicObj.GetComponents<AudioSource>();
            if (sources.Length > 1)
            {
                sources[1].Play(); // Plays the click sound
            }
        }
    }
}