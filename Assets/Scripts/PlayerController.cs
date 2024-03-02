using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    public GameObject MainGameLogicHandler;
    private  enum playerChoice
        {
        Yes,
        No,
        DonKnow
         }
    // Start is called before the first frame update
  
/* void SendResult(playerChoice _playerChoice)
    {
        PhotonView pv = MainGameLogicHandler.GetPhotonView();
        pv.RPC("TestRpc", RpcTarget.MasterClient, _playerChoice.ToString());
    }*/

/*    private void Start()
    {
        SendResult
            (playerChoice.Yes);
    }
    public void OnClickYes()
    {
        SendResult (playerChoice.Yes);
    }
    public void OnClickNo()
    {
        SendResult(playerChoice.No);
    }
    public  void OnClickIdonKnow()
    {
        SendResult(playerChoice.DonKnow);
    }

    public void OnClickStart()
    {
        PhotonView pv = MainGameLogicHandler.GetPhotonView();

    }*/
}
