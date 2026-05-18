using UnityEngine;

public class PersistentMusic : MonoBehaviour
{
    private static PersistentMusic instance;

    void Awake()
    {
        // If an instance already exists, destroy this one so we don't have 2 songs playing
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            // Tell Unity: "Don't kill this object when the scene changes"
            DontDestroyOnLoad(gameObject);
        }
    }
}