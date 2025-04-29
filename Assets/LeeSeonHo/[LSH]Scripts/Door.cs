#if HAS_DOTWEEN
using DG.Tweening;
#endif
using UnityEngine;

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

#if HAS_DOTWEEN
        doorTransform.DOLocalRotate(targetAngle, openDuration, RotateMode.LocalAxisAdd).OnComplete(() =>
        {
            isOpen = true;
        });
#endif
    }

    public void CloseDoor()
    {
#if HAS_DOTWEEN
        doorTransform.DOLocalRotate(Vector3.zero, closeDuration).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            isOpen = false;
        });
#endif
    }
}
