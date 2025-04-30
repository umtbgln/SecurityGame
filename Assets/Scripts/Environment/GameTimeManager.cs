using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager instance;

    [Range(0, 24)] public float currentHour = 0f;
    public float dayDuration = 360f; // 1 g�n 6 dakika s�recek (360 saniye)
    private bool isPaused = true;  // Zaman�n durup durmad���n� kontrol etmek i�in bir flag
    
    
    private void Start()
    {
        // Zaman ba�lat�lacak
        ResumeTime();
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        
        else Destroy(gameObject);
    }

    void Update()
    {
        if (isPaused) return;  // E�er zaman durduysa, update fonksiyonu �al��mas�n.

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

    // Zaman� durdurma fonksiyonu
    public void PauseTime()
    {
        isPaused = true;
        Debug.Log("Zaman durdu!");
        NPCManager.instance.SendAllNPCsToExit(); // Call to send NPCs to exit
    }

    // Zaman� ba�latma fonksiyonu
    public void ResumeTime()
    {
        isPaused = false;
        Debug.Log("Zaman devam ediyor.");
    }
}