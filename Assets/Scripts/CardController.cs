using System;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [SerializeField] private Transform object1; // 첫 번째 오브젝트
    [SerializeField] private Transform object2; // 두 번째 오브젝트
    [SerializeField] private GameObject gaugeGreen;
    [SerializeField] private GameObject gaugeYellow;
    [SerializeField] private GameObject gaugeRed;
    [SerializeField] private float suspicionThreshold = 2.0f; // 의심 증가 임계값
    [SerializeField] private float suspicionIncreaseRate = 20f; // 의심 수치 증가율
    private float dragStartTime;
    private bool isDragging = false;
    private float suspicionLevel = 0;
    private Vector3 initialOffset;
    private float delayTime;

    private void Update()
    {
        if (isDragging)
        {
            if (Time.time - dragStartTime > delayTime)
            {
                suspicionLevel += suspicionIncreaseRate * Time.deltaTime;
            }
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        dragStartTime = Time.time; 
        initialOffset = object2.position - object1.position;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(object1.position).z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            
            object1.position = new Vector3(object1.position.x, worldPosition.y, object1.position.z);
            object2.position = object1.position + initialOffset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        float dragDuration = Time.time - dragStartTime; 

        // 드래그 시간이 일정 시간을 초과하면 의심 수치 증가
        if (dragDuration > suspicionThreshold)
        {
            suspicionLevel += suspicionIncreaseRate;
            Debug.Log("dragDuration > suspicionThreshold");
        }
        CompareWithGauge(object2.position.y);
    }

    void CompareWithGauge(float objectY)
    {
        float GreenTopY = gaugeGreen.GetComponent<BoxCollider2D>().bounds.max.y;
        float GreenBottomY = gaugeGreen.GetComponent<BoxCollider2D>().bounds.min.y;
        float YellowTopY = gaugeYellow.GetComponent<BoxCollider2D>().bounds.max.y;
        float YellowBottomY = gaugeYellow.GetComponent<BoxCollider2D>().bounds.min.y;
        float RedTopY = gaugeRed.GetComponent<BoxCollider2D>().bounds.max.y;
        float RedBottomY = gaugeRed.GetComponent<BoxCollider2D>().bounds.min.y;

        if (objectY >= RedBottomY && objectY <= RedTopY)
        {
            if (objectY >= YellowBottomY && objectY <= YellowTopY)
            {
                if (objectY >= GreenBottomY && objectY <= GreenTopY)
                {
                    Debug.Log("Green");
                    return;
                }
                Debug.Log("Yellow");
                return;
            }
            Debug.Log("Red");
            return;
        }
    }
}
