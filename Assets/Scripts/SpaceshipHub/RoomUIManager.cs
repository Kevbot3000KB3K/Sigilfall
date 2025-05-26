using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomUIManager : MonoBehaviour
{
    public GameObject confirmButton;
    public Vector2 visiblePosition = new Vector2(0f, -450f);
    public float tweenDuration = 0.5f;

    private RectTransform confirmRect;

    private void Awake()
    {
        confirmRect = confirmButton.GetComponent<RectTransform>();
        confirmRect.anchoredPosition = new Vector2(0, -800); // Start off-screen
        confirmButton.SetActive(false);
    }

    public void ShowConfirmButton()
    {
        confirmButton.SetActive(true);
        LeanTween.moveLocalY(confirmButton, visiblePosition.y, tweenDuration).setEaseOutExpo();
    }
}
