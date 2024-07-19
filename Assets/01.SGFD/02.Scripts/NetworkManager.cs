using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;

    public CharImage charImage; // CharImage -> Image로 수정

    public CharManager charManager;

    public GameObject fadeImage;

    [Header("DisconnectPanel")]
    public TMP_InputField NickNameInput;
    public GameObject DisconnectPanel;

    [Header("LobbyPanel")]
    public GameObject LobbyPanel;
    public InputField RoomInput;
    public Text WelcomeText;
    public Text LobbyInfoText;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    public Text ListText;
    public Text RoomInfoText;
    public TMP_Text[] ChatText;
    public InputField ChatInput;

    [Header("ETC")]
    public TMP_Text StatusText;
    public PhotonView PV;

    [Header("Spawn Positions")]
    public Transform[] spawnPositions;

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;

    public GameObject startGameButton;

    [SerializeField] Transform playerLisContent;
    [SerializeField] GameObject playerListItemPrefab;
    internal object playerList;

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
        {
            PV.RPC("StartGameRPC", RpcTarget.All);
        }
        else
        {
            Debug.LogError("Not connected to Photon master client or not in a room.");
        }
    }

    [PunRPC]
    void StartGameRPC()
    {
        StartCoroutine(StartGameCoroutine());
    }

    IEnumerator StartGameCoroutine()
    {
        Spawn();
        Fade();
        yield return new WaitForSeconds(2f);
        DisconnectPanel.SetActive(false);
        RoomPanel.SetActive(false);
        LobbyPanel.SetActive(false);
        yield return new WaitForSeconds(2f);
        fadeImage.SetActive(false);
    }
    public void Fade()
    {
        StartCoroutine(FadeCor());
    }

    public IEnumerator FadeCor()
    {
        fadeImage.SetActive(true);
        yield return new WaitForSeconds(4f);
        fadeImage.SetActive(false);
    }

    public void Spawn()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            Vector3 spawnPosition = spawnPositions[playerIndex % spawnPositions.Length].position;
            PhotonNetwork.Instantiate(CharManager.instance.currentCharacter.ToString(), spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Not connected to Photon server.");
        }
    }

    #region RoomList Management

    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();
    }

    void MyListRenewal()
    {
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count);
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }

    #endregion

    #region Photon Callbacks

    void Awake()
    {
        instance = this;
        Screen.SetResolution(960, 540, false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Send();
        }
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
        LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + " 로비 / " + PhotonNetwork.CountOfPlayers + " 접속";
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        LobbyPanel.SetActive(true);
        RoomPanel.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "님 환영합니다";
        myList.Clear();
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
    }

    public void CreateRoom() => PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text, new RoomOptions { MaxPlayers = 4 });

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnJoinedRoom()
    {
        RoomPanel.SetActive(true);
        RoomRenewal();

        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Count(); i++)
        {
            GameObject playerItem = Instantiate(playerListItemPrefab, playerLisContent);
            playerItem.GetComponent<PlayerListItem>().Setup(players[i]);
            charImage = playerItem.GetComponent<CharImage>();
        }

        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }

        ChatInput.text = "";
        for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        RoomInput.text = "";
        CreateRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomInput.text = "";
        CreateRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");
        Instantiate(playerListItemPrefab, playerLisContent).GetComponent<PlayerListItem>().Setup(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");
    }

    void RoomRenewal()
    {
        ListText.text = "";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            ListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
        RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "최대";
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }
    }

    #endregion

    #region Chat

    public void Send()
    {
        if (!string.IsNullOrEmpty(ChatInput.text))
        {
            PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
            ChatInput.text = "";
        }

        ChatInput.ActivateInputField();
        ChatInput.Select();
    }

    [PunRPC]
    void ChatRPC(string msg)
    {
        for (int i = 0; i < ChatText.Length - 1; i++)
        {
            ChatText[i].text = ChatText[i + 1].text;
        }

        ChatText[ChatText.Length - 1].text = msg;
    }

    #endregion
}
