using UnityEngine;
using UnityEngine.UI; // UI bileþenlerini kullanmak için

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 100; // Maksimum can
    private int currentHealth;    // Mevcut can
    public Slider healthSlider;   // Can çubuðu referansý

    private void Start()
    {
        currentHealth = maxHealth; // Baþlangýçta mevcut can maksimum can deðerine eþit
        UpdateHealthSlider(); // Slider'ý baþlangýçta güncelle
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Hasar al
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Caný sýfýrýn altýna düþürme

        Debug.Log($"Current Health: {currentHealth}");
        UpdateHealthSlider(); // Slider'ý güncelle

        if (currentHealth <= 0)
        {
            Die(); // Ölüm durumunu kontrol et
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount; // Ýyileþ
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Caný maksimum deðere sýnýrla

        Debug.Log($"Current Health: {currentHealth}");
        UpdateHealthSlider(); // Slider'ý güncelle
    }

    private void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth; // Slider deðerini güncell
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} öldü!");
        // Ölüm iþlemleri burada yapýlabilir (örneðin, yeniden doðma, oyun sonu ekraný vb.)
        gameObject.SetActive(false); // Karakteri devre dýþý býrak
    }
}