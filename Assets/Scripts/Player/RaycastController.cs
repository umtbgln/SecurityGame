using UnityEngine;

public class RaycastController : MonoBehaviour
{
    public float rayDistance = 1f;               // Raycast uzakl���
    public LayerMask interactableLayer;           // Etkile�im yap�lacak objelerin katman�
    public LayerMask npcLayer;                    // NPC'lerin bulundu�u katman maskesi

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact"))  // T�klama tu�una bas�ld���nda etkile�im ba�las�n
        {
            PerformRaycast();
        }
    }

    void PerformRaycast()
    {
        RaycastHit hit;

        // interactableLayer ve npcLayer'� bitwise OR i�lemiyle birle�tiriyoruz
        LayerMask combinedLayer = interactableLayer | npcLayer;

        // Raycast i�lemi, iki katman maskesini birle�tirerek yap�yoruz
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, rayDistance, combinedLayer))
        {
            Interactable currentInteractable = hit.collider.GetComponent<Interactable>();

            if (currentInteractable != null)
            {
                currentInteractable.OnInteract();  // NPC ile etkile�im ba�lat
            }
        }
    }
}
