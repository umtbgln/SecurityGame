using UnityEngine;
using UnityEngine.UI; // UI bile�enlerini kullanmak i�in

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 100; // Maksimum can
    private int currentHealth;    // Mevcut can
    public Slider healthSlider;   // Can �ubu�u referans�

    private void Start()
    {
        currentHealth = maxHealth; // Ba�lang��ta mevcut can maksimum can de�erine e�it
        UpdateHealthSlider(); // Slider'� ba�lang��ta g�ncelle
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Hasar al
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Can� s�f�r�n alt�na d���rme

        Debug.Log($"Current Health: {currentHealth}");
        UpdateHealthSlider(); // Slider'� g�ncelle

        if (currentHealth <= 0)
        {
            Die(); // �l�m durumunu kontrol et
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount; // �yile�
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Can� maksimum de�ere s�n�rla

        Debug.Log($"Current Health: {currentHealth}");
        UpdateHealthSlider(); // Slider'� g�ncelle
    }

    private void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth; // Slider de�erini g�ncell
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} �ld�!");
        // �l�m i�lemleri burada yap�labilir (�rne�in, yeniden do�ma, oyun sonu ekran� vb.)
        gameObject.SetActive(false); // Karakteri devre d��� b�rak
    }
}