using UnityEngine;

public class XRayScanner : MonoBehaviour
{
    public Transform scannerPoint;
    public float scanRadius = 5f; // Yar��ap� b�y�tt�k
    public LayerMask npcLayer;
    public Light scannerLight; // I��k objesi
    public Color cleanColor = Color.green;
    public Color illegalColor = Color.red;

    void Update()
    {
        bool foundIllegal = false;
        Collider[] npcs = Physics.OverlapSphere(scannerPoint.position, scanRadius, npcLayer);

        if (npcs.Length == 0)
        {
            scannerLight.color = cleanColor;
            return;
        }

        foreach (Collider npc in npcs)
        {
            if (npc == null) continue;

            NPCData data = npc.GetComponent<NPCData>();
            if (data != null)
            {
                Debug.Log($"{npc.name} - Illegal: {data.hasIllegalItem}", npc.gameObject); // Objeyi Console'da t�klanabilir yapar
                if (data.hasIllegalItem)
                {
                    foundIllegal = true;
                    break;
                }
            }
            else
            {
                Debug.LogError($"{npc.name} �zerinde NPCData yok!", npc.gameObject);
            }
        }

        scannerLight.color = foundIllegal ? illegalColor : cleanColor;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(scannerPoint.position, scanRadius);
    }
}