using UnityEngine;
using UnityEngine.EventSystems;

public class HoverListener : MonoBehaviour, IPointerEnterHandler
{
    public InventoryPanelController manager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        manager.SetHoveredButton(transform.parent);
    }
}
