using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RoomSelector : MonoBehaviour
{
    [Header("Room Settings")]
    public string roomName;
    public GameObject selectionVisual;
    private static RoomUIManager roomUIManager;
    private static RoomSelector currentRoom;
    private static List<RoomSelector> allRooms = new List<RoomSelector>();
    private static TextMeshProUGUI roomLabel;
    [Header("Scene To Load")]
    public string sceneToLoad; // Set this in Inspector, e.g., "SigilLab"
    private static string selectedScene;
    [Header("Lighting")]
    public GameObject unselectedLight;
    public GameObject selectedLight;
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip selectSFX;


    private void Awake()
    {
        allRooms.Add(this);
        if (selectionVisual != null)
            selectionVisual.SetActive(false); // Start hidden
    }

    private void OnDestroy()
    {
        allRooms.Remove(this);
    }
    public static void SetUIManager(RoomUIManager manager)
    {
        roomUIManager = manager;
    }
    public static void SetRoomLabel(TextMeshProUGUI label)
    {
        roomLabel = label;
    }

    private void OnMouseDown()
    {
        if (audioSource != null && selectSFX != null)
        {
            audioSource.PlayOneShot(selectSFX);
        }
        Debug.Log("Clicked room: " + roomName);

        if (roomLabel != null)
        {
            roomLabel.text = roomName;
            Debug.Log("Room label updated.");
        }
        else
        {
            Debug.LogWarning("Room label is NULL");
        }

        // Reset all other rooms
        foreach (RoomSelector room in allRooms)
        {
            if (room.selectionVisual != null)
                room.selectionVisual.SetActive(false);

            if (room.unselectedLight != null)
                room.unselectedLight.SetActive(true);

            if (room.selectedLight != null)
                room.selectedLight.SetActive(false);
        }

        // Enable this room's visual
        if (selectionVisual != null)
        {
            selectionVisual.SetActive(true);
            Debug.Log("Enabled selection visual for " + roomName);
        }

        // Enable selected light and disable unselected one
        if (unselectedLight != null)
            unselectedLight.SetActive(false);

        if (selectedLight != null)
            selectedLight.SetActive(true);

        currentRoom = this;

        if (roomUIManager != null)
        {
            roomUIManager.ShowConfirmButton();
        }

        selectedScene = sceneToLoad;
    }
    public static string GetSelectedScene()
    {
        return selectedScene;
    }
    public static string GetCurrentRoomName()
    {
        return currentRoom != null ? currentRoom.roomName : "";
    }

}
