using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;

    public void SetMaxHealth(int maxHealth, int? currentHealth = null)
    {
        slider.maxValue = maxHealth;
        if (currentHealth != null)
        {
            slider.value = (float) currentHealth;
        }
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
