using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public Key key;
    private Animator animator;
    private bool isOpen = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        if (key.isKeyActive)
        {
            if (!isOpen)
            {
                OpenDoor();
            }
        }
        else
        {
            Debug.Log("Kapý kilitli.");
        }
    }

    void OpenDoor()
    {
        Debug.Log("Kapý açýldý!");
        animator.SetTrigger("isOpen");
        isOpen = true;
    }
}
