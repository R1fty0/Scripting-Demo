using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour
{
    #region Variables
    [Tooltip("Where the door will be when it is closed.")]
    public Transform doorClosedTransform;
    [Tooltip("Where the door will be when it is open.")]
    public Transform doorOpenTransform;
    [Tooltip("How long the door will take to open/close.")]
    public float doorStateChangeDuration;
    #endregion

    #region Door Open/Close
    // Open the door
    public void Open()
    {
        transform.DOMove(doorOpenTransform.position, doorStateChangeDuration);
    }

    // Close the door
    public void Close()
    {
        transform.DOMove(doorClosedTransform.position, doorStateChangeDuration);
    }
    #endregion
}