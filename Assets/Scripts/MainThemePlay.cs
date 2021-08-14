using UnityEngine;

public class MainThemePlay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var audioManager = FindObjectOfType<AudioManager>();
        audioManager.Play("MainTheme");
    }
}
