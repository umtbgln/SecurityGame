using System.Collections;
using UnityEngine;

public class NPCData : MonoBehaviour
{
    public bool hasIllegalItem;
    public float confessChance; // �tiraf etme ihtimali (0.0 - 1.0 aras� de�er)

    private void Start()
    {
        ResetData();
    }

    public void ResetData()
    {
        // 20% �ansla illegal item var
        hasIllegalItem = Random.value < 0.20f;

        // �tiraf etme ihtimali: 30% ile 80% aras�nda rastgele ayarlan�yor
        confessChance = Random.Range(0.3f, 0.8f);
    }
}
