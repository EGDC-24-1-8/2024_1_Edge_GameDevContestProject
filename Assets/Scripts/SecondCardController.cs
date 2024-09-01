using System;
using UnityEngine;

public class SecondCardController : MonoBehaviour
{
    [SerializeField] private Transform secondCard; 
    [SerializeField] private Transform secondArrow;
    [SerializeField] private GameObject gaugeGreen;
    [SerializeField] private GameObject gaugeYellow;
    [SerializeField] private GameObject gaugeRed;
    //[SerializeField] private float suspicionThreshold = 2.0f; // 의심 증가 임계값
    //[SerializeField] private float suspicionIncreaseRate = 20f; // 의심 수치 증가율

    private float dragStartTime;
    [SerializeField] private bool isDragging = false;
    //[SerializeField] private float suspicionLevel = 0;
    private Vector3 initialOffset;
    private Vector3 mouseOffset;
    private float delayTime = 1.5f; // 의심 수치 증가 시작 시간
    private float GreenTopY;
    private float GreenBottomY;
    private float YellowTopY;
    private float YellowBottomY;
    private float RedTopY;
    private float RedBottomY;
    private float ArrowTopY;
    private float ArrowBottomY;

    private Vector3 origin_Position;
    private Vector3 origin_ArrowPosition;

    public event Action<bool> secondCardMoved;
    

    private void Start()
    {
        SetPosition();
    }
    
    private void SetPosition()
    {
        GreenTopY = gaugeGreen.GetComponent<BoxCollider2D>().bounds.max.y;
        GreenBottomY = gaugeGreen.GetComponent<BoxCollider2D>().bounds.min.y;
        YellowTopY = gaugeYellow.GetComponent<BoxCollider2D>().bounds.max.y;
        YellowBottomY = gaugeYellow.GetComponent<BoxCollider2D>().bounds.min.y;
        RedTopY = gaugeRed.GetComponent<BoxCollider2D>().bounds.max.y;
        RedBottomY = gaugeRed.GetComponent<BoxCollider2D>().bounds.min.y;
        ArrowTopY = secondArrow.GetComponent<BoxCollider2D>().bounds.max.y;
        ArrowBottomY = secondArrow.GetComponent<BoxCollider2D>().bounds.min.y;
    }
    private void Update()
    {
        if (isDragging)
        {
            if (Time.time - dragStartTime > delayTime)
            {
                GameManager.Instance.IncreaseSuspicionByDragTime();
                //suspicionLevel += suspicionIncreaseRate * Time.deltaTime;
            }
        }
    }
    void OnMouseDown()
    {
        if (DialogManager.Instance.isDialogHighPriority)
            return;
        if (GameManager.Instance.gameState != GameManager.GameState.deal)
            return;
        if (!(GameManager.Instance.mousePointState == GameManager.MousePointState.normal))
            return;

        //if (DialogSystem.Instance.isDialog)
        //    return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        Collider2D collider = GetComponent<Collider2D>();
        origin_Position = transform.position;
        origin_ArrowPosition = secondArrow.position;
        if (mousePos.y >= ArrowBottomY && mousePos.y <= ArrowTopY)
        {
            isDragging = true;
            dragStartTime = Time.time;

            // 오브젝트와 마우스 위치 간의 오프셋 계산
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(secondCard.position).z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            mouseOffset = secondCard.position - worldPosition;

            initialOffset = secondArrow.position - secondCard.position;
        }
        else
        {
            isDragging = false;
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(secondCard.position).z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // 새로운 y 위치 계산 및 제한 적용
            float newY = worldPosition.y + mouseOffset.y;

            if (newY < origin_Position.y) //밑으로 못 당기게
                newY = origin_Position.y;
            // x, z 위치 고정
            secondCard.position = new Vector3(secondCard.position.x, newY, secondCard.position.z);
            secondArrow.position = secondCard.position + initialOffset;
        }
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;
        float dragDuration = Time.time - dragStartTime;

        // 드래그 시간이 일정 시간을 초과하면 의심 수치 증가
        /*
        if (dragDuration > suspicionThreshold)
        {
            //suspicionLevel += suspicionIncreaseRate;
            Debug.Log("dragDuration > suspicionThreshold");
        }
        */
        if (transform.position.y - origin_Position.y > 0.2f)
        {
            CompareWithGauge(secondArrow.position.y);
            gameObject.SetActive(false);
            secondArrow.gameObject.SetActive(false);
            Destroy(gameObject.transform.parent.gameObject);
        }
        else
        {
            GameManager.Instance.IncreaseSuspicionByDragButDontDeal();
            transform.position = origin_Position;
            secondArrow.position = origin_ArrowPosition;
        }
    }

    void CompareWithGauge(float objectY)
    {
        if (GameManager.Instance.gameState != GameManager.GameState.deal)
        {
            secondCardMoved?.Invoke(true);
            return;
        }
        GameManager.Instance.SecondDeal();
        DealingManager.Instance.SecondSetImage();
        if (objectY >= RedBottomY)
        {
            if (objectY >= YellowBottomY && objectY <= YellowTopY)
            {
                if (objectY >= GreenBottomY && objectY <= GreenTopY)
                {
                    secondCardMoved?.Invoke(true);
                    Debug.Log("Green");
                    GameManager.Instance.IncreaseSuspicionByGauge(0);
                    return;
                }
                secondCardMoved?.Invoke(true);
                Debug.Log("Yellow");
                GameManager.Instance.IncreaseSuspicionByGauge(1);
                return;
            }
            secondCardMoved?.Invoke(true);
            Debug.Log("Red");
            GameManager.Instance.IncreaseSuspicionByGauge(2);
            return;
        }
    }
}
