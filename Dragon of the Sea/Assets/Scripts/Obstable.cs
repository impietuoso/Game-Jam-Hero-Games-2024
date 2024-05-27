using UnityEngine;
using UnityEngine.UI;

public class Obstable : MonoBehaviour {
    public int maxHealth;
    private int currentHealth;
    public Slider healthSlider;

    private void Awake() {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void TakeDamage(int damage) {
        if (currentHealth - damage <= 0) {
            currentHealth = 0;
            healthSlider.value = currentHealth;
            Destroy();
        } else currentHealth -= damage;
        healthSlider.value = currentHealth;
    }

    public void Destroy() {

    }
}