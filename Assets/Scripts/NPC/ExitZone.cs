using UnityEngine;

public class ExitZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Eðer çarpýþan nesne bir NPC ise ve "NPCBehavior" bileþeni varsa
        NPCBehavior npc = other.GetComponent<NPCBehavior>();
        if (npc != null)
        {
            npc.DeactivateViaTrigger(); // NPC kendi kendini sistem dýþý býrakýr
        }
    }
}
