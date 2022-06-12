using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject createRoomScreen;
    public GameObject lobbyBrowserScreen;
    public GameObject lobbyScreen;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button findRoomButton;

    [Header("Lobby Browswer Screen")]
    public RectTransform roomListContainer;
    public GameObject roomButtonPrefab;

    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public TextMeshProUGUI roomInfoText;
    public Button startGameButton;

    private List<GameObject> roomButtons = new List<GameObject>();
    private List<RoomInfo> roomList = new List<RoomInfo>();

    private void Start()
    {
        // Disable buttons until connected to server
        createRoomButton.interactable = false;
        findRoomButton.interactable = false;

        // Check if in game and jump to that room
        if (PhotonNetwork.InRoom)
        {
            // Jump to lobby
            SetScreen(lobbyScreen);
            UpdateLobbyUI();

            // Make the room visible again
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
    }

    private void SetScreen(GameObject screen)
    {
        mainScreen.SetActive(false);
        createRoomScreen.SetActive(false);
        lobbyBrowserScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        screen.SetActive(true);

        if (screen == lobbyBrowserScreen)
            UpdateLobbyBrowserUI();
    }

    #region Main Screen
    public void PlayerName_ValueChanged(TMP_InputField playerName)
    {
        PhotonNetwork.NickName = playerName.text;
    }

    public void JoinLobbyButton_Click()
    {
        SetScreen(lobbyBrowserScreen);
    }

    public void CreateRoomMenuButton_Click()
    {
        SetScreen(createRoomScreen);
    }

    public void PlayerName_Selected(TMP_InputField inputField)
    {
        TouchScreenKeyboard.Open(inputField.text, TouchScreenKeyboardType.Default);
    }
    #endregion

    #region Create Room Screen
    public void CreateRoomButton_Click(TMP_InputField roomName)
    {
        NetworkManager.instance.CreateRoom(roomName.text);
    }

    public void BackToMainButton_Click()
    {
        SetScreen(mainScreen);
    }

    [PunRPC]
    private void UpdateLobbyUI()
    {
        // Enable Start Game only if Master Client
        startGameButton.interactable = PhotonNetwork.IsMasterClient;

        // Refresh players
        playerListText.text = "";
        foreach (Player player in PhotonNetwork.PlayerList)
            playerListText.text += player.NickName + "\n";

        // Set the Room Info
        roomInfoText.text = "<b>Room Name</b>\n" + PhotonNetwork.CurrentRoom.Name;
    }
    #endregion

    #region Lobby Browser Screen
    // Use OnBackToMainButton_Click for the back button on this screen

    public void RefreshButton_Click()
    {
        UpdateLobbyBrowserUI();
    }

    private void RoomButton_Click(string roomName)
    {
        NetworkManager.instance.JoinRoom(roomName);
    }

    private GameObject CreateNewRoomButton()
    {
        GameObject button = Instantiate(roomButtonPrefab, roomListContainer.transform);
        roomButtons.Add(button);
        return button;
    }

    private void UpdateLobbyBrowserUI()
    {
        // Disable room buttons
        foreach (GameObject button in roomButtons)
            button.SetActive(false);

        // Display all current rooms in Master Server
        for (int x = 0; x < roomList.Count; x++)
        {
            // Get or create the button
            GameObject button = x >= roomButtons.Count ? CreateNewRoomButton() : roomButtons[x];
            button.SetActive(true);

            // Set Room Name and Player Counts
            button.transform.Find("Room Name Text").GetComponent<TextMeshProUGUI>().text = roomList[x].Name;
            button.transform.Find("Player Count Text").GetComponent<TextMeshProUGUI>().text = roomList[x].PlayerCount + " / " + roomList[x].MaxPlayers;

            // Set button OnClick event
            Button buttonComp = button.GetComponent<Button>();
            string roomName = roomList[x].Name;
            buttonComp.onClick.RemoveAllListeners();
            buttonComp.onClick.AddListener(() => { RoomButton_Click(roomName); });
        }
    }
    #endregion

    #region Lobby Screen
    public void StartButton_Click()
    {
        // Hide the current room
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game Scene");
    }

    public void LeaveButton_Click()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(lobbyBrowserScreen);
    }
    #endregion

    #region Network Events
    public override void OnConnectedToMaster()
    {
        // Enable menu buttons once we are connected
        createRoomButton.interactable = true;
        findRoomButton.interactable = true;
        Debug.Log("Connected to Master");
    }

    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateLobbyUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    public override void OnRoomListUpdate(List<RoomInfo> rooms)
    {
        roomList = rooms;
    }
    #endregion
}
