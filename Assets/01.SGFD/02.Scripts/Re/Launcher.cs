using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{

    public static Launcher Instance;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemtPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemtPrefab;
    [SerializeField] GameObject startGameButton;
    [SerializeField] TMP_InputField NickNameInput;

    [SerializeField] GameObject NickNamePanel;
    // Start is called before the first frame update

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        PhotonNetwork.NickName = NickNameInput.text;

    }
    void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }
    public void OnTitle()
    {
        if (string.IsNullOrEmpty(NickNameInput.text))
        {
            return;
        }
        MenuManager.Instance.OpenMenu("title");
        NickNamePanel.SetActive(false);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("nickname");
        Debug.Log("Joined Lobby");
        //PhotonNetwork.NickName = "Player" + Random.Range(0, 1000).ToString("0000");
    }
    public void CreateRoom()
    {

        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("loading");

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnJoinedRoom()
    {
      
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;


        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemtPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");


    }
   
    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }
    public void Connect() => PhotonNetwork.ConnectUsingSettings();
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemtPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);

        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemtPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(newPlayer);
    }
}
