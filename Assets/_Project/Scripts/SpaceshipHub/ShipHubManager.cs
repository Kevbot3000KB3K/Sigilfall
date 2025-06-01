using UnityEngine;
using TMPro;
using System.Collections;

public class ShipHubManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI roomLabel;

    [Header("Floating Motion")]
    public Transform shipRoot;       // The parent GameObject of the ship
    public RectTransform canvasRoot; // The UI Canvas (rect transform)

    public float floatAmplitude = 10f;   // Pixels for UI, units for ship
    public float floatFrequency = 0.5f;  // Speed of the floating

    private Vector3 shipStartPos;
    private Vector2 canvasStartPos; // ✅ Should be Vector2

    private void Start()
    {
        StartCoroutine(DelayedInit());
    }
    private IEnumerator DelayedInit()
    {
        yield return null; // ⏱ wait 1 frame AFTER the scene is fully loaded
        RoomSelector.SetRoomLabel(roomLabel);
        RoomSelector.SetUIManager(GetComponent<RoomUIManager>());
    }
    private void Update()
    {
        float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        if (shipRoot != null)
            shipRoot.localPosition = shipStartPos + new Vector3(0f, floatOffset, 0f);

        if (canvasRoot != null)
            canvasRoot.anchoredPosition = canvasStartPos + new Vector2(0f, floatOffset);
    }
}
