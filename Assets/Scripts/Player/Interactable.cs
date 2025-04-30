using UnityEngine;

public enum InteractionType
{
    Bed,
    NPCDialog // NPC ile etkileþim için yeni bir tür ekledik
}

public class Interactable : MonoBehaviour
{
    public InteractionType interactionType;
    public DialogueManager dialogueManager; // DialogueManager referansý
    public NPCData npcData; // NPC verisi

    public virtual void OnInteract()
    {
        if (interactionType == InteractionType.Bed)
        {
            GameTimeManager.instance.ResumeTime();
            Debug.Log("Yataða girildi, zaman baþladý.");
        }
        else if (interactionType == InteractionType.NPCDialog)
        {
            if (npcData != null && npcData.hasIllegalItem)
            {
                if (dialogueManager != null)
                {
                    var npcBehavior = GetComponent<NPCBehavior>();
                    if (npcBehavior != null)
                    {
                        dialogueManager.TryStartDialogue(npcData, npcBehavior);
                        Debug.Log("Illegal item nedeniyle NPC ile etkileþime girildi.");
                    }
                    else
                    {
                        Debug.LogWarning("NPCBehavior bileþeni eksik!");
                    }
                }
                else
                {
                    Debug.LogWarning("DialogueManager referansý eksik!");
                }
            }
            else
            {
                Debug.Log("Bu NPC'nin illegal bir eþyasý yok, etkileþim baþlatýlamaz.");
            }
        }
    }
}

