using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamagePopup : MonoBehaviour
{
    public float riseSpeed = 1f;
    public float duration = 1f;
    public float fadeSpeed = 2f;

    [Header("Custom Number Display")]
    public Sprite[] digitSprites; // Indexed 0-9
    public GameObject digitImagePrefab; // Prefab for each digit image
    public Transform digitHolder; // Parent that holds the digits

    private CanvasGroup canvasGroup;

    private void Start()
    {
        // Assign the main camera to the Canvas if found
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Show(float amount, Vector3 worldPosition)
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        }

        Debug.Log("Showing damage popup with value: " + amount);

        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one;

        // 💥 Key line: position the popup above the brick
        transform.position = worldPosition + Vector3.up * 0.5f;

        ClearExistingDigits();

        if (digitSprites == null || digitSprites.Length < 10)
        {
            Debug.LogError("digitSprites array is null or not filled with 10 sprites!");
            return;
        }

        string damageString = Mathf.CeilToInt(amount).ToString();

        foreach (char c in damageString)
        {
            int digit = c - '0';
            if (digit < 0 || digit > 9) continue;

            GameObject digitObj = Instantiate(digitImagePrefab, digitHolder);
            Image img = digitObj.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = digitSprites[digit];
                img.color = Color.white;
            }
        }

        Destroy(gameObject, duration);
    }


    private void ClearExistingDigits()
    {
        foreach (Transform child in digitHolder)
        {
            Destroy(child.gameObject);
        }
    }

    private void Update()
    {
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, fadeSpeed * Time.deltaTime);
        }
    }
}
