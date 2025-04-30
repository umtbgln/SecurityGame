using UnityEngine;

public enum InteractionType
{
    Bed,
    NPCDialog // NPC ile etkile�im i�in yeni bir t�r ekledik
}

public class Interactable : MonoBehaviour
{
    public InteractionType interactionType;
    public DialogueManager dialogueManager; // DialogueManager referans�
    public NPCData npcData; // NPC verisi

    public virtual void OnInteract()
    {
        if (interactionType == InteractionType.Bed)
        {
            GameTimeManager.instance.ResumeTime();
            Debug.Log("Yata�a girildi, zaman ba�lad�.");
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
                        Debug.Log("Illegal item nedeniyle NPC ile etkile�ime girildi.");
                    }
                    else
                    {
                        Debug.LogWarning("NPCBehavior bile�eni eksik!");
                    }
                }
                else
                {
                    Debug.LogWarning("DialogueManager referans� eksik!");
                }
            }
            else
            {
                Debug.Log("Bu NPC'nin illegal bir e�yas� yok, etkile�im ba�lat�lamaz.");
            }
        }
    }
}

