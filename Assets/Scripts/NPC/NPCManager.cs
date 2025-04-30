using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager instance;
    [Header("NPC Settings")]
    public float spawnInterval = 10f;  // Ka� saniyede bir NPC aktif olacak
    public int maxNPCCount = 10;        // Maksimum NPC say�s�

    private List<GameObject> npcList = new List<GameObject>();
    private GameTimeManager timeManager;
    private bool hasClosed = false;

    private void Start()
    {
        // T�m NPC'leri bul ve devre d��� b�rak
        InitializeNPCs();
        timeManager = GameTimeManager.instance;
        StartCoroutine(SpawnNPCs());
    }

    private void Awake()
    {
        // Singleton deseni ile instance ba�latma
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Ayn� sahnede birden fazla instance varsa di�erlerini sil
        }
    }

    private void InitializeNPCs()
    {
        GameObject[] allNPCs = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject npc in allNPCs)
        {
            npc.SetActive(false);
            npcList.Add(npc);
        }
    }

    private IEnumerator SpawnNPCs()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Yeni g�n ba�lad�ysa hasClosed flag'ini s�f�rla
            if (timeManager.GetHour() == 0 && hasClosed)
            {
                hasClosed = false;
                Debug.Log("Yeni g�n ba�lad�. NPC'ler yeniden spawnlanabilir.");
            }

            // NPC spawn zaman�n� kontrol et
            if (IsSpawnTime() && GetActiveNPCCount() < maxNPCCount)
            {
                ActivateRandomNPC();
            }
            else if (IsAfterClosingTime() && !hasClosed)
            {
                hasClosed = true; // Tek seferlik �a�r�l�r
                SendAllNPCsToExit();
            }
        }
    }

    private bool IsSpawnTime()
    {
        int currentHour = timeManager.GetHour();
        return currentHour > 0  &&  currentHour < 24;  // 24 saatlik d�ng�
    }

    private bool IsAfterClosingTime()
    {
        int currentHour = timeManager.GetHour();
        return currentHour == 0; // 0. saatte (gece) kapanma durumu
    }

    private void ActivateRandomNPC()
    {
        GameObject npc = GetRandomInactiveNPC();
        if (npc != null)
        {
            npc.SetActive(true);
            Debug.Log($"{npc.name} aktifle�tirildi.");
            StartCoroutine(DelayedStart(npc));
        }
    }

    private IEnumerator DelayedStart(GameObject npc)
    {
        yield return null; // Bir frame bekle
        NPCBehavior npcBehavior = npc.GetComponent<NPCBehavior>();
        if (npcBehavior != null)
        {
            npcBehavior.StartRoam();
        }
    }

    public void SendAllNPCsToExit()
    {
        foreach (GameObject npc in npcList)
        {
            if (npc.activeInHierarchy)
            {
                NPCBehavior npcBehavior = npc.GetComponent<NPCBehavior>();
                if (npcBehavior != null)
                {
                    npcBehavior.ForceExit();
                }
            }
        }
    }

    private GameObject GetRandomInactiveNPC()
    {
        // Aktif olmayan NPC' leri bul
        List<GameObject> inactiveNPCs = npcList.FindAll(npc =>
        {
            var behavior = npc.GetComponent<NPCBehavior>();
            return !npc.activeInHierarchy && behavior != null && !behavior.isDone;
        });

        if (inactiveNPCs.Count == 0) return null;

        int index = Random.Range(0, inactiveNPCs.Count);
        return inactiveNPCs[index];
    }

    // Aktif NPC say�s�n� d�nd�r�r
    private int GetActiveNPCCount()
    {
        int activeCount = 0;
        foreach (GameObject npc in npcList)
        {
            if (npc.activeInHierarchy)
            {
                activeCount++;
            }
        }
        return activeCount;
    }
}