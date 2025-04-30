using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light sun;                   // Directional Light referans�
    public float dayDuration = 20f;    // 1 g�n ka� saniye s�recek (�rnek: 2 dakika)

    private float time;

    void Update()
    {
        time += Time.deltaTime;
        float rotation = (time / dayDuration) * 360f;

        // G�ne�i d�nd�r (X ekseninde)
        sun.transform.rotation = Quaternion.Euler(rotation - 90, 170, 0);

        // I��k yo�unlu�u ayar� (daha geli�mi� i�in interpolate kullan�labilir)
        sun.intensity = Mathf.Clamp01(Vector3.Dot(sun.transform.forward, Vector3.down));
    }
}
