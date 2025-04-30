using UnityEngine;

public class ExitZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // E�er �arp��an nesne bir NPC ise ve "NPCBehavior" bile�eni varsa
        NPCBehavior npc = other.GetComponent<NPCBehavior>();
        if (npc != null)
        {
            npc.DeactivateViaTrigger(); // NPC kendi kendini sistem d��� b�rak�r
        }
    }
}
