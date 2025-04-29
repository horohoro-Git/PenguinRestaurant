using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform doorTransform;
    [SerializeField] private float openDuration = 0.4f;
    [SerializeField] private float closeDuration = 0.5f;

    private Vector3 openAngle = new Vector3(0f, 90f, 0f);
    private bool isOpen = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Customer") && !isOpen)
        {
            OpenDoor(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Customer") && isOpen)
        {
            CloseDoor();
        }
    }

    public void OpenDoor(Transform interactor)
    {
        Vector3 direction = (interactor.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(direction, transform.forward);
        Vector3 targetAngle = openAngle * Mathf.Sign(dotProduct);

        doorTransform.DOLocalRotate(targetAngle, openDuration, RotateMode.LocalAxisAdd).OnComplete(() =>
        {
            isOpen = true;
        });
    }

    public void CloseDoor()
    {
        doorTransform.DOLocalRotate(Vector3.zero, closeDuration).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            isOpen = false;
        });
    }
}
