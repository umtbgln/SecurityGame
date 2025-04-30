using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager instance;

    [Range(0, 24)] public float currentHour = 0f;
    public float dayDuration = 360f; // 1 gün 6 dakika sürecek (360 saniye)
    private bool isPaused = true;  // Zamanýn durup durmadýðýný kontrol etmek için bir flag
    
    
    private void Start()
    {
        // Zaman baþlatýlacak
        ResumeTime();
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        
        else Destroy(gameObject);
    }

    void Update()
    {
        if (isPaused) return;  // Eðer zaman durduysa, update fonksiyonu çalýþmasýn.

        float timeProgress = Time.deltaTime / dayDuration;
        currentHour += timeProgress * 24f;

        if (currentHour >= 24f)
        {
            PauseTime();
            currentHour = 0f;
        }
    }

    public int GetHour() => Mathf.FloorToInt(currentHour);
    public float GetTime() => currentHour;

    // Zamaný durdurma fonksiyonu
    public void PauseTime()
    {
        isPaused = true;
        Debug.Log("Zaman durdu!");
        NPCManager.instance.SendAllNPCsToExit(); // Call to send NPCs to exit
    }

    // Zamaný baþlatma fonksiyonu
    public void ResumeTime()
    {
        isPaused = false;
        Debug.Log("Zaman devam ediyor.");
    }
}