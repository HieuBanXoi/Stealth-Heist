using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpening : MonoBehaviour
{
    private bool isPlayerNearBy = false;
    private Animator animator;
    private GameManager gameManager;
    [SerializeField] private GameObject canvasText;
    [SerializeField] private int numberOfKeysToOpen;
    [SerializeField] private GameObject circleIndicator; // Biểu tượng chấm chỉ thị
    private bool isChestOpened = false; // Thêm biến để kiểm tra trạng thái mở của rương

    void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        canvasText.SetActive(false);
        circleIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlayerNearBy || isChestOpened) return; // Return sớm nếu player không ở gần hoặc rương đã mở

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (numberOfKeysToOpen <= gameManager.numberOfKeys)
            {
                OpenChest();
            }
            else
            {
                ShowNotEnoughKeysMessage();
            }
        }
    }

    private void OpenChest()
    {
        animator.SetBool("CanOpen", true);
        isChestOpened = true;
        StartCoroutine(DelayedGameWin(1f)); // Gọi GameWin sau 2 giây
    }

    private IEnumerator DelayedGameWin(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameManager.GameWin();
    }

    private void ShowNotEnoughKeysMessage()
    {
        canvasText.SetActive(true);
        StartCoroutine(HideTextAfterSeconds(3f));
    }

    IEnumerator HideTextAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        canvasText.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearBy = true;
            if (circleIndicator != null)
            {
                circleIndicator.SetActive(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearBy = false;
            if (circleIndicator != null)
            {
                circleIndicator.SetActive(false);
            }
        }
    }
}