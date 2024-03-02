using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkHandler : MonoBehaviourPunCallbacks
{
    private float stageTime = 60.0f;
    public int questionStage = 0;
    public PhotonView MainGameLogicHandlerPv;
    public MainGameLogicHandler Mgh;
    public Dictionary<string, int> ScoreBoardDictionary = new Dictionary<string, int>();    
    [Header("First scene")]
    public GameObject FirstScene;
    public TMP_InputField userNameInput;
    public TMP_Text userState;
    public TMP_Text waitText;
    private string UniqueRoom = "Hwempire";
    public Button RegisterBtn;
   

    [Header("Second Scene")]
    public GameObject SecondScene;
    public TMP_Text joinUserInfo;
    public GameObject startBtn;

    [Header("Third Scene")]
    public GameObject ThirdScene;
    public TMP_Text AnswerNotificationTxt;
    public Button YesBtn;
    public Button NoBtn;
    public Button IdonKnowBtn;

     void Start ()
    {
        connect();

    }
 
    private void Update()
    {
        userState.text ="Client State :  " + PhotonNetwork.NetworkClientState.ToString();
        if(!PhotonNetwork.IsMasterClient) joinUserInfo.text = "Current User in lobby   " + Mgh.totalUserCount + "/15" + "  Wait for host to start game";
        else
        {
            joinUserInfo.text = "Current User in lobby   " + Mgh.totalUserCount + "/15";
        }

    }
 
    public  void CreateRoom()
    {
        PhotonNetwork.CreateRoom(UniqueRoom
            );
        Debug.Log("Create room success");
        
    }

    public void connect() => PhotonNetwork.ConnectUsingSettings();
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        base.OnConnectedToMaster();
    }

    public void JoinOrCreateRoom()
    {

        if(PhotonNetwork.IsConnected ==false)
        {
            Debug.Log("Disconnected");
            return;
        }
        PhotonNetwork.JoinRandomOrCreateRoom();
        
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("photon onjoined room Call");
        MainGameLogicHandlerPv.RPC("SynChronizeUserCount", RpcTarget.All, PhotonNetwork.CountOfPlayers);
        FirstScene.SetActive(false);
        SecondScene.SetActive(true);
       
        Mgh.Dbinsert(userNameInput.text);
        if (PhotonNetwork.IsMasterClient)
        {
            startBtn.SetActive(true);
        }
    }

    public void StartGame()
    {
       
        Mgh.totalUserCount = PhotonNetwork.CountOfPlayers;
       SecondScene.SetActive(false);
        ThirdScene.SetActive(true);
    }
    public void OnClickAnswerYes()
    {
       
        MainGameLogicHandlerPv.RPC("CollectAnswer", RpcTarget.MasterClient, "Yes",userNameInput.text);
        YesBtn.gameObject.SetActive(false);
        NoBtn.gameObject.SetActive(false);
        IdonKnowBtn.gameObject.SetActive(false);
        waitText.gameObject.SetActive(true);
    }
    public void OnClickAnswerNo()
    {
        MainGameLogicHandlerPv.RPC("CollectAnswer", RpcTarget.MasterClient, "No", userNameInput.text);
        YesBtn.gameObject.SetActive(false);
        NoBtn.gameObject.SetActive(false);
        IdonKnowBtn.gameObject.SetActive(false);
        waitText.gameObject.SetActive(true);
    }
    public void OnClickAnswerIdonKnow()
    {
        MainGameLogicHandlerPv.RPC("CollectAnswer", RpcTarget.MasterClient, "DonKnow", userNameInput.text);
        YesBtn.gameObject.SetActive(false);
        NoBtn.gameObject.SetActive(false);
        IdonKnowBtn.gameObject.SetActive(false);
        waitText.gameObject.SetActive(true);
    }



    // Start is called before the first frame update
    /*    public Dictionary<string,int> userDatabase = new Dictionary<string,int>();
        void CreateUserData(int playerNumber)
        {
            for  (int i = 0; i <playerNumber
                ; i++)
            {
                userDatabase.Add("player" + i, 0);

            }
        }
    */

}
