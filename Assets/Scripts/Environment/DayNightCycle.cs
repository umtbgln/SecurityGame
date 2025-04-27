using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light sun;                   // Directional Light referansý
    public float dayDuration = 20f;    // 1 gün kaç saniye sürecek (örnek: 2 dakika)

    private float time;

    void Update()
    {
        time += Time.deltaTime;
        float rotation = (time / dayDuration) * 360f;

        // Güneþi döndür (X ekseninde)
        sun.transform.rotation = Quaternion.Euler(rotation - 90, 170, 0);

        // Iþýk yoðunluðu ayarý (daha geliþmiþ için interpolate kullanýlabilir)
        sun.intensity = Mathf.Clamp01(Vector3.Dot(sun.transform.forward, Vector3.down));
    }
}
