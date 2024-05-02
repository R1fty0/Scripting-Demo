using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorTrigger : MonoBehaviour
{
    #region Variables
    [Tooltip("CASE SENSITIVE.The tag of the object that will trigger this thing upon collision.")]
    public string objectTag;
    [Tooltip("The door this trigger will open.")]
    public Door door;
    [Tooltip("Does the door close if nothing is in the trigger zone.")]
    public bool changeStateOnExit;
    #endregion

    #region isTrigger Warning 
    private void Start()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null )
        {
            if (!collider.isTrigger)
            {
                Debug.LogError("This collider: " + collider.name + " is not set to isTrigger!");
            }
        }
    }
    #endregion

    #region Trigger Door Open & Close
    // Trigger door opening 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(objectTag))
        {
            door.Open();
        }
    }

    // Trigger door closing 
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(objectTag))
        {
            if (changeStateOnExit)
            {
                door.Close();
            }
        }
    }
    #endregion
}