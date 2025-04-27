using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCBehavior : MonoBehaviour
{
    public enum NPCState { Idle, Roaming, Exiting }

    [Header("Waiting Area Settings")]
    public List<Transform> waitingAreas = new List<Transform>(); // Birden fazla bekleme noktası
    private static HashSet<Transform> occupiedWaitingAreas = new HashSet<Transform>(); // Dolu alanları takip etmek için

    [Header("NPC Settings")]
    public Transform exitPoint;           // Çıkış noktası
    public Transform scanner;             // XRay cihazının hedef noktası
    public float roamRadius = 25f;        // Dolaşma yarıçapı
    public int roamLimit = 5;             // Kaç kez dolaşacak
    public bool isDone = false;           // Görevini bitirdi mi?

    [Header("Avoidance Settings")]
    public float minDistanceFromSpawn = 10f; // Spawn noktasından en az bu kadar uzakta dolaşsın
    private Vector3 spawnPosition; // NPC'nin ilk aktif olduğu pozisyon

    private DialogueManager dialogueManager;
    private NPCData npcData;
    private NavMeshAgent agent;
    private Coroutine activeRoutine;
    private NPCState currentState = NPCState.Idle;
    Animator npcAnimator;

    private void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();  // DialogueManager referansını al
        agent = GetComponent<NavMeshAgent>();
        npcData = GetComponent<NPCData>();
        spawnPosition = transform.position; // Başlangıç pozisyonunu kaydet
        npcAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        // NPC'nin hızına bağlı olarak koşma animasyonunu kontrol et
        if (agent.velocity.magnitude > 0.1f) // Eğer hız 0'dan büyükse (koşuyorsa)
        {
            npcAnimator.SetBool("Run", true);
        }
        else // Eğer hız 0'a yakınsa (idle durumda)
        {
            npcAnimator.SetBool("Run", false);
        }
    }

    public void InteractWithNPC()
    {
        if (npcData != null)
        {
            var npcBehavior = GetComponent<NPCBehavior>();
            if (npcBehavior != null && dialogueManager != null)
            {
                dialogueManager.TryStartDialogue(npcData, npcBehavior); // İki parametre gönder
            }
            else
            {
                Debug.LogWarning("NPCBehavior veya DialogueManager eksik!");
            }
        }
    }



    public void StartRoam()
    {
        if (currentState != NPCState.Idle || isDone) return;
        currentState = NPCState.Roaming;
        activeRoutine = StartCoroutine(RoamAndExit());
    }


    public void ForceExit()
    {
        if (isDone) return;
        Debug.Log($"{gameObject.name} çıkışa gidiyor.");
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        currentState = NPCState.Exiting;
        activeRoutine = StartCoroutine(GoToExit());
    }

    private IEnumerator RoamAndExit()
    {
        // Tarayıcıya git
        if (scanner != null)
        {
            yield return MoveToDestination(scanner.position);
            yield return new WaitForSeconds(0.5f); // Tarayıcıda bekleme süresi
        }

        if (npcData != null && npcData.hasIllegalItem && waitingAreas.Count > 0)
        {
            Transform assignedWaitingArea = GetAvailableWaitingArea();
            if (assignedWaitingArea != null)
            {
                occupiedWaitingAreas.Add(assignedWaitingArea);
                yield return MoveToDestination(assignedWaitingArea.position);
                Debug.Log($"{gameObject.name} illegal item nedeniyle bekleme alanına yönlendirildi: {assignedWaitingArea.name}");

                // Burada BEKLEMEYE geç
                currentState = NPCState.Idle;  // Buraya önemli: Beklemedeyken Idle moda geçiriyoruz
                yield break; // Burada RoamAndExit coroutine'i durdur
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: Boş bekleme alanı bulunamadı, çıkışa yönlendiriliyor.");
                yield return MoveToDestination(GetExitPointWithOffset());
                yield break;
            }
        }



        // Rastgele dolaşma işlemi
        for (int i = 0; i < roamLimit; i++)
        {
            Vector3 randomDestination = GetRandomNavMeshPosition(transform.position, roamRadius);
            yield return MoveToDestination(randomDestination);
            yield return new WaitForSeconds(Random.Range(4f, 10f));
        }



        // Çıkış noktasına git
        yield return MoveToDestination(GetExitPointWithOffset());
        DeactivateNPC();
    }

    private IEnumerator GoToExit()
    {
        yield return MoveToDestination(GetExitPointWithOffset());
        DeactivateNPC();
    }

    private IEnumerator MoveToDestination(Vector3 destination)
    {
        if (agent == null || !agent.isOnNavMesh)
        {
            Debug.LogWarning($"{gameObject.name} NavMesh'te değil, işlem iptal.");
            yield break;
        }

        agent.SetDestination(destination);

        float distance = Vector3.Distance(transform.position, destination);
        float timeout = Mathf.Clamp(distance / agent.speed, 10f, 30f); // Mesafeye göre zaman aşımı belirle
        float timer = 0f;

        while (true)
        {
            if (agent == null || !agent.isOnNavMesh)
            {
                Debug.LogWarning($"{gameObject.name} NavMesh kaybı.");
                yield break;
            }

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                yield break; // Hedefe ulaştı
            }

            timer += Time.deltaTime;

            if (timer >= timeout)
            {
                Debug.LogWarning($"{gameObject.name} hedefe ulaşamadı. Alternatif rota aranıyor...");
                agent.SetDestination(GetExitPointWithOffset()); // Alternatif bir yol belirle
                timer = 0f; // Tekrar denemek için süreyi sıfırla
            }

            yield return null;
        }
    }



    private Vector3 GetExitPointWithOffset()
    {
        Vector3 offset = Random.insideUnitSphere * 3f; // 0.5 birimlik rastgele sapma
        offset.y = 0; // Yükseklik değişmesin
        return exitPoint.position + offset;
    }

    private Vector3 GetRandomNavMeshPosition(Vector3 origin, float radius)
    {
        Vector3 randomPosition = Vector3.zero;
        int maxAttempts = 30; // 30 denemeden sonra vazgeç

        for (int i = 0; i < maxAttempts; i++)
        {
            // 1. Rastgele bir yön seç (origin etrafında)
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += origin;
            randomDirection.y = origin.y; // Yükseklik sabit kalsın

            // 2. NavMesh'te örnekleme yap
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
                // 3. Spawn noktasından minimum uzaklık kontrolü
                if (Vector3.Distance(hit.position, spawnPosition) >= minDistanceFromSpawn)
                {
                    // 4. Hedefin önünde engel var mı? (Opsiyonel)
                    if (!Physics.Raycast(origin, hit.position - origin, Vector3.Distance(origin, hit.position)))
                    {
                        return hit.position; // Uygun pozisyon bulundu!
                    }
                }
            }
        }

        // Uygun pozisyon bulunamazsa FALLBACK: Çıkış noktasına git
        Debug.LogWarning($"{name}: Rastgele pozisyon bulunamadı! Çıkışa yönlendiriliyor.");
        return GetExitPointWithOffset();
    }

    private void DeactivateNPC()
    {
        isDone = false;  // Görev sıfırlanıyor
                         
        // Bekleme alanı boşaltma
        if (waitingAreas.Count > 0)
        {
            occupiedWaitingAreas.RemoveWhere(area => Vector3.Distance(transform.position, area.position) < 1.5f);
        }


        currentState = NPCState.Idle;
        if (npcData != null)
        {
            npcData.hasIllegalItem = false;  // Illegal item sıfırlama
            npcData.ResetData();
            Debug.Log($"{gameObject.name} illegal item sıfırlandı.");
        }
        Debug.Log($"{gameObject.name} görevi sıfırlandı ve devre dışı bırakıldı.");
        gameObject.SetActive(false); // NPC'yi görünmez yap
    }

    // Bu fonksiyon NPC'nin kendi kendini devre dışı bırakmasını sağlar
    public void DeactivateViaTrigger()
    {
        if (!isDone)
        {
            Debug.Log($"{gameObject.name} Trigger alanına girdi ve devre dışı bırakıldı.");
            DeactivateNPC();
        }
    }
    private Transform GetAvailableWaitingArea()
    {
        foreach (Transform area in waitingAreas)
        {
            if (!occupiedWaitingAreas.Contains(area))
            {
                return area;
            }
        }
        return null; // Hiç boş yer kalmadıysa
    }

}