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
    void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        canvasText.SetActive(false);
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerNearBy && Input.GetKeyDown(KeyCode.Space))
        {
            if (numberOfKeysToOpen <= gameManager.numberOfKeys)
            {
                animator.SetBool("CanOpen", true);
                GameManager.Instance.isChestOpened = true; // Thông báo rằng rương đã mở
            }
            else
            {
                canvasText.SetActive(true);
                StartCoroutine(HideTextAfterSeconds(3f));
            }
        }
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
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearBy = false;
        }
    }
}