using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public Text dialogueText;
    public Text optionsText;
    public GameObject dialogueBox;

    private bool isDialogueActive = false;
    private bool isPlayerTurn = true;
    private bool isInBagOptions = false; // Çanta seçeneklerinde olup olmadýðýný kontrol etmek için

    private NPCBehavior currentNPC;
    private NPCData npcData;

    // Dinamik seçenekler listesi
    private List<DialogueOption> currentOptions = new List<DialogueOption>();

    Coroutine messageCoroutine;
    Coroutine optionsCoroutine;

    void Start()
    {
        dialogueBox.SetActive(false);
    }

    public void TryStartDialogue(NPCData npcData, NPCBehavior npcBehavior)
    {
        if (npcData == null)
        {
            Debug.LogWarning("NPCData null!");
            return;
        }

        this.npcData = npcData;  // NPC verisini saklýyoruz
        currentNPC = npcBehavior; // Aktif NPC'yi saklýyoruz
        StartDialogue();
    }

    public void StartDialogue()
    {
        if (isDialogueActive)
            return;

        dialogueBox.SetActive(true);
        ShowMessage("Oyuncu: Merhaba!");
        isDialogueActive = true;
        isPlayerTurn = true;

        // Baþlangýçta gösterilecek seçenekleri ekliyoruz
        currentOptions.Clear();
        currentOptions.Add(new DialogueOption("Çantaný açar mýsýn?", () =>
        {
            ShowMessage("Oyuncu: Çantaný açar mýsýn?");
            StartCoroutine(WaitAndShowNpcResponse("Açmayý kabul ediyor musun?", npcData.confessChance, "bag"));
        }));

        currentOptions.Add(new DialogueOption("Bir þey söyleyebilir misin?", () =>
        {
            ShowMessage("Oyuncu: Bir þey söyleyebilir misin?");
            StartCoroutine(WaitAndShowNpcResponse("Sadece bir þeyler söyledim.", npcData.confessChance, "talk"));
        }));

        currentOptions.Add(new DialogueOption("Boþ þuanlýk deðiþecek", () =>
        {
            ShowMessage("Oyuncu: Boþ þuanlýk deðiþecek");
            StartCoroutine(WaitAndShowNpcResponse("Sadece bir þeyler söyledim.", npcData.confessChance, "talk"));
        }));

        currentOptions.Add(new DialogueOption("Diyaloðu bitir.", () =>
        {
            EndDialogue();
        }));

        ShowOptions();
    }

    void ShowMessage(string message)
    {
        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        messageCoroutine = StartCoroutine(TypeText(message));
    }

    IEnumerator TypeText(string message)
    {
        dialogueText.text = "";
        foreach (char letter in message.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.01f); // Harfler arasý gecikme (0.02 saniye)
        }
    }

    IEnumerator TypeOptions()
    {
        optionsText.text = "";
        string fullText = "";
        for (int i = 0; i < currentOptions.Count; i++)
        {
            fullText += (i + 1) + ": " + currentOptions[i].optionText + "\n";
        }

        foreach (char letter in fullText.ToCharArray())
        {
            optionsText.text += letter;
            yield return new WaitForSeconds(0.01f);
        }
    }

    void ShowOptions()
    {
        if (optionsCoroutine != null)
            StopCoroutine(optionsCoroutine);

        optionsCoroutine = StartCoroutine(TypeOptions());
    }

    void Update()
    {
        if (isDialogueActive && isPlayerTurn)
        {
            // Çanta seçenekleri aktifse
            if (isInBagOptions)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    OnBagOptionSelected(1);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    OnBagOptionSelected(2);
                }
            }
            else
            {
                // Normal seçenekler aktifse
                for (int i = 0; i < currentOptions.Count; i++)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    {
                        OnOptionSelected(i + 1);
                        break;
                    }
                }
            }
        }
    }

    void OnOptionSelected(int optionNumber)
    {
        currentOptions[optionNumber - 1].onSelectAction.Invoke();
        isPlayerTurn = false;
        optionsText.text = "";
    }

    IEnumerator WaitAndShowNpcResponse(string response, float confessChance, string context)
    {
        yield return new WaitForSeconds(1f);

        if (Random.value <= confessChance) // NPC'nin itiraf etme þansýna göre cevap veriyoruz
        {
            ShowMessage("NPC: Tamam, iþte çantamý gösteriyorum.");
            npcData.hasIllegalItem = true; // Çantada illegal item var
            if (context == "bag")
            {
                ShowBagOptions(); // Çanta açýldýðýnda yeni seçenekler
                isInBagOptions = true; // Çanta seçeneklerine geçtiðimizi belirtiyoruz
            }
            else if (context == "talk")
            {
                ShowMessage("NPC: Hayýr, sadece konuþtum."); // Burada deðiþecek
            }
        }
        else
        {
            ShowMessage("NPC: Hayýr, gösteremem.");
            currentNPC.ForceExit(); // NPC çýkýyor

            Invoke(nameof(EndDialogue), 2f); // 2 saniye sonra EndDialogue çalýþýr
        }

        yield return new WaitForSeconds(1.5f);
        isPlayerTurn = true;
    }

    void ShowBagOptions()
    {
        currentOptions.Clear();
        currentOptions.Add(new DialogueOption("Eþyayý al", () =>
        {
            ShowMessage("Oyuncu: Eþyayý alýyorum.");
            npcData.hasIllegalItem = false; // Eþyayý alýyoruz
            if (currentNPC != null)
            {
                currentNPC.StartRoam(); // NPC'yi dolaþmaya baþlatýyoruz
                Debug.Log("NPC dolaþmaya baþladý.");
                EndDialogue();
            }
        }));

        currentOptions.Add(new DialogueOption("Çýkýþa gönder", () =>
        {
            ShowMessage("Oyuncu: NPC'yi çýkýþa yönlendiriyorum.");
            if (currentNPC != null)
            {
                currentNPC.ForceExit(); // NPC çýkýyor
                EndDialogue();
            }
        }));

        ShowOptions();
    }

    void OnBagOptionSelected(int optionNumber)
    {
        currentOptions[optionNumber - 1].onSelectAction.Invoke();
        isInBagOptions = false; // Çanta seçenekleri bitti, normal seçeneklere geri dönüyoruz
        isPlayerTurn = false;
        optionsText.text = "";
    }

    void EndDialogue()
    {
        ShowMessage("Diyalog kapatýldý.");
        dialogueBox.SetActive(false);
        isDialogueActive = false;
        isPlayerTurn = false;
        optionsText.text = "";
    }
}

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public System.Action onSelectAction;

    public DialogueOption(string text, System.Action action)
    {
        optionText = text;
        onSelectAction = action;
    }
}