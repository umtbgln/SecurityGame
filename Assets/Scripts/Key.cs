using UnityEngine;

public class Key : MonoBehaviour
{
    public GameObject key;
    public bool isKeyActive;

    void Update()
    {
        isKeyActive = key.activeSelf;
    }
}
