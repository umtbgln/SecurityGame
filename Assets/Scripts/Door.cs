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
            Debug.Log("Kap� kilitli.");
        }
    }

    void OpenDoor()
    {
        Debug.Log("Kap� a��ld�!");
        animator.SetTrigger("isOpen");
        isOpen = true;
    }
}
