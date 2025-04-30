using UnityEngine;
using System.Collections.Generic;

public class HamburgerDelivery : MonoBehaviour
{
    public Transform startPos;
    public Transform initialDestination;
    public Transform deliveryPoint;
    public GameObject hamburgerSetPrefab;
    public float moveSpeed = 0.5f;
    public float orderQuantity = 3f;
    public GameObject penguin;
    public GameObject delieverPenguin;

    private Vector3 initialDestinationPosition; // 최초의 도착 지점 위치 값
    private Vector3 penguinInitialPosition; // 팽귄의 최초 위치 저장
    private Vector3 deliverPenguinInitialPosition; // 배달 팽귄의 최초 위치 저장
    private List<GameObject> hamburgerSets = new List<GameObject>();
    private int index = 0;

    void Start()
    {
        initialDestinationPosition = initialDestination.position; // 초기 위치 저장
        penguinInitialPosition = penguin.transform.position; // 팽귄의 최초 위치 저장
        deliverPenguinInitialPosition = delieverPenguin.transform.position; // 배달 팽귄의 최초 위치 저장
        delieverPenguin.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (index < orderQuantity)
            {
                CreateAndMoveHamburgerSet();
            }
            else
            {
                DeliverHamburgerSets();
            }
        }
    }

    void CreateAndMoveHamburgerSet()
    {
        GameObject hamburgerSet = Instantiate(hamburgerSetPrefab, startPos.position, Quaternion.identity);
        hamburgerSets.Add(hamburgerSet);
        StartCoroutine(MoveToPosition(hamburgerSet, initialDestination.position + Vector3.up * (0.3f * index), moveSpeed));

        index++;
    }

    System.Collections.IEnumerator MoveToPosition(GameObject obj, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = obj.transform.position;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float height = 2 * (1 - Mathf.Pow(2 * t - 1, 2));
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * height;
            yield return null;
        }
    }

    void DeliverHamburgerSets()
    {
        foreach (GameObject hamburgerSet in hamburgerSets)
        {
            StartCoroutine(MoveToDeliveryPoint(hamburgerSet, deliveryPoint.position, 1f));
        }
    }

    System.Collections.IEnumerator MoveToDeliveryPoint(GameObject obj, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = obj.transform.position;
        float elapsed = 0;
        delieverPenguin.SetActive(true);
        penguin.SetActive(false);

        yield return new WaitForSeconds(1f);

        // 배달 팽귄의 원래 위치에서 배달 위치로 이동
        Vector3 penguinStartPos = penguin.transform.position;
        Vector3 deliverPenguinStartPos = deliverPenguinInitialPosition; // 배달 팽귄의 초기 위치

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float height = 2 * (1 - Mathf.Pow(2 * t - 1, 2));
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * height;

            // 배달 팽귄 이동            
            delieverPenguin.transform.position = Vector3.Lerp(deliverPenguinStartPos, targetPosition, t) + Vector3.up * height;

            yield return null;
        }

        obj.SetActive(false);
        delieverPenguin.SetActive(false); // 배달 후 팽귄 비활성화

        // 모든 햄버거 세트가 비활성화되었으면 초기화
        if (hamburgerSets.TrueForAll(h => !h.activeSelf))
        {
            ResetDelivery();
        }
    }

    void ResetDelivery()
    {
        index = 0;
        initialDestination.position = initialDestinationPosition; // 도착 지점을 최초 위치로 초기화
        hamburgerSets.Clear(); // 리스트 초기화
        delieverPenguin.SetActive(false); // 배달 팽귄 비활성화
        penguin.SetActive(true); // 원래 팽귄 활성화

        // 팽귄과 배달 팽귄을 원래 위치로 이동
        penguin.transform.position = penguinInitialPosition; // 팽귄을 최초 위치로 돌아감
        delieverPenguin.transform.position = deliverPenguinInitialPosition; // 배달 팽귄을 최초 위치로 돌아감
    }
}
