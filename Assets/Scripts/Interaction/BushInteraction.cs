using UnityEngine;

public class BushInteraction : MonoBehaviour, IInteractable
{
    private ResourceNode resource;

    private void Awake()
    {
        resource = GetComponent<ResourceNode>();
    }

    public void Interact()
    {
        if (resource != null)
        {
            resource.Hit(999);
        }
    }
}