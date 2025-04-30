using UnityEngine;

public class RaycastController : MonoBehaviour
{
    public float rayDistance = 1f;               // Raycast uzaklýðý
    public LayerMask interactableLayer;           // Etkileþim yapýlacak objelerin katmaný
    public LayerMask npcLayer;                    // NPC'lerin bulunduðu katman maskesi

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact"))  // Týklama tuþuna basýldýðýnda etkileþim baþlasýn
        {
            PerformRaycast();
        }
    }

    void PerformRaycast()
    {
        RaycastHit hit;

        // interactableLayer ve npcLayer'ý bitwise OR iþlemiyle birleþtiriyoruz
        LayerMask combinedLayer = interactableLayer | npcLayer;

        // Raycast iþlemi, iki katman maskesini birleþtirerek yapýyoruz
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, rayDistance, combinedLayer))
        {
            Interactable currentInteractable = hit.collider.GetComponent<Interactable>();

            if (currentInteractable != null)
            {
                currentInteractable.OnInteract();  // NPC ile etkileþim baþlat
            }
        }
    }
}
