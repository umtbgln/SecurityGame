using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damageAmount = 10;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"�arp��ma alg�land�: {collision.gameObject.name}");

        if (collision.gameObject.CompareTag("Player"))
        {
            HealthManager healthManager = collision.gameObject.GetComponent<HealthManager>();
            if (healthManager != null)
            {
                healthManager.TakeDamage(damageAmount);
                Debug.Log($"Hasar verildi: {damageAmount}");
            }
            else
            {
                Debug.Log("HealthManager bile�eni bulunamad�.");
            }
        }
    }
}
