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
    private bool isInBagOptions = false; // �anta se�eneklerinde olup olmad���n� kontrol etmek i�in

    private NPCBehavior currentNPC;
    private NPCData npcData;

    // Dinamik se�enekler listesi
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

        this.npcData = npcData;  // NPC verisini sakl�yoruz
        currentNPC = npcBehavior; // Aktif NPC'yi sakl�yoruz
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

        // Ba�lang��ta g�sterilecek se�enekleri ekliyoruz
        currentOptions.Clear();
        currentOptions.Add(new DialogueOption("�antan� a�ar m�s�n?", () =>
        {
            ShowMessage("Oyuncu: �antan� a�ar m�s�n?");
            StartCoroutine(WaitAndShowNpcResponse("A�may� kabul ediyor musun?", npcData.confessChance, "bag"));
        }));

        currentOptions.Add(new DialogueOption("Bir �ey s�yleyebilir misin?", () =>
        {
            ShowMessage("Oyuncu: Bir �ey s�yleyebilir misin?");
            StartCoroutine(WaitAndShowNpcResponse("Sadece bir �eyler s�yledim.", npcData.confessChance, "talk"));
        }));

        currentOptions.Add(new DialogueOption("Bo� �uanl�k de�i�ecek", () =>
        {
            ShowMessage("Oyuncu: Bo� �uanl�k de�i�ecek");
            StartCoroutine(WaitAndShowNpcResponse("Sadece bir �eyler s�yledim.", npcData.confessChance, "talk"));
        }));

        currentOptions.Add(new DialogueOption("Diyalo�u bitir.", () =>
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
            yield return new WaitForSeconds(0.01f); // Harfler aras� gecikme (0.02 saniye)
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
            // �anta se�enekleri aktifse
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
                // Normal se�enekler aktifse
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

        if (Random.value <= confessChance) // NPC'nin itiraf etme �ans�na g�re cevap veriyoruz
        {
            ShowMessage("NPC: Tamam, i�te �antam� g�steriyorum.");
            npcData.hasIllegalItem = true; // �antada illegal item var
            if (context == "bag")
            {
                ShowBagOptions(); // �anta a��ld���nda yeni se�enekler
                isInBagOptions = true; // �anta se�eneklerine ge�ti�imizi belirtiyoruz
            }
            else if (context == "talk")
            {
                ShowMessage("NPC: Hay�r, sadece konu�tum."); // Burada de�i�ecek
            }
        }
        else
        {
            ShowMessage("NPC: Hay�r, g�steremem.");
            currentNPC.ForceExit(); // NPC ��k�yor

            Invoke(nameof(EndDialogue), 2f); // 2 saniye sonra EndDialogue �al���r
        }

        yield return new WaitForSeconds(1.5f);
        isPlayerTurn = true;
    }

    void ShowBagOptions()
    {
        currentOptions.Clear();
        currentOptions.Add(new DialogueOption("E�yay� al", () =>
        {
            ShowMessage("Oyuncu: E�yay� al�yorum.");
            npcData.hasIllegalItem = false; // E�yay� al�yoruz
            if (currentNPC != null)
            {
                currentNPC.StartRoam(); // NPC'yi dola�maya ba�lat�yoruz
                Debug.Log("NPC dola�maya ba�lad�.");
                EndDialogue();
            }
        }));

        currentOptions.Add(new DialogueOption("��k��a g�nder", () =>
        {
            ShowMessage("Oyuncu: NPC'yi ��k��a y�nlendiriyorum.");
            if (currentNPC != null)
            {
                currentNPC.ForceExit(); // NPC ��k�yor
                EndDialogue();
            }
        }));

        ShowOptions();
    }

    void OnBagOptionSelected(int optionNumber)
    {
        currentOptions[optionNumber - 1].onSelectAction.Invoke();
        isInBagOptions = false; // �anta se�enekleri bitti, normal se�eneklere geri d�n�yoruz
        isPlayerTurn = false;
        optionsText.text = "";
    }

    void EndDialogue()
    {
        ShowMessage("Diyalog kapat�ld�.");
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