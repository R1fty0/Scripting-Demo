using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
public class TriggerArea : MonoBehaviour
{
    // To-Do: Make this trigger area work with all colliders 


    [Header("Debug")]
    [Tooltip("Show the box collider while in play mode.")]
    [SerializeField] private bool showBoundsInPlayMode;
    [Tooltip("The color of the box collider when shown in play mode.")]
    [SerializeField] private Color debugColor;

    private BoxCollider boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    private void OnDrawGizmos()
    {
        if (showBoundsInPlayMode)
        {
            Gizmos.DrawCube(transform.position, boxCollider.size);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
