using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using ExitGames.Client.Photon;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
class PlayerAnswer
{
    public string playerName;
    public string playerAnswerChoice;
}

public class MainGameLogicHandler : MonoBehaviourPun
{
    private int yesCount = 0;
    private int noCount = 0;
    private int donCount = 0;
    private int submitAnswerCount = 0;
    public int totalUserCount;
    private int OngoingQuestion = 0;
    public List<string> QuestionSheet;
    public Dictionary<string, string>  tempAnswerSheet = new Dictionary<string, string>();

   

    public int _submitAnswerCount => submitAnswerCount;
    public GameObject yesbtn;
    public GameObject nobtn;
    public GameObject donKnowbtn2;
    public NetworkHandler Nwh;
    public TMP_Text waitBox;

    public TMP_Text questionShowSheet;
    public TMP_Text answerShowSheet;
    public TMP_Text ScoreBoard;
    public Text quizbar_num;
    public GameObject ScoreLeaderboard;
    // Start is called before the first frame update
    public void ThirdSceneExecute()
    {

        PhotonView pv = photonView;
        pv.RPC("ThirdSceneChangeRpc", RpcTarget.All,OngoingQuestion);
        
        _ThirdExecute();
    }

    [PunRPC]
    public void ThirdSceneChangeRpc(int n)
    {
        questionShowSheet.text = QuestionSheet[n];
        Nwh.SecondScene.SetActive(false);
        Nwh.ThirdScene.SetActive(true);
        answerShowSheet.gameObject.transform.parent.gameObject.SetActive(false);
        answerShowSheet.gameObject.SetActive(false);
        ScoreBoard.gameObject.SetActive(false);
        

    }
    [PunRPC]
public Tuple <string,string>SelectAnswer()
    {

        var tempList = new List<int>()
        {
            yesCount
            ,noCount
            ,donCount
        };
        var resultList = new List<int>();
        var stringResultList = new List<string>();
        int maxValue = tempList.Max
            (x => x);
        if(yesCount == maxValue)
        {
            resultList.Add
                  (0);
            stringResultList.Add("Yes");
        }
        if (noCount == maxValue)
        {
            resultList.Add
                  (1);
            stringResultList.Add("No");

        }
        if (donCount == maxValue)
        {
            resultList.Add
                  (2);
            stringResultList.Add("DonKnow");
        }
        var stringResult = stringResultList.Select(x => x).Aggregate((text, next) => text + "  " + next);
        return Tuple.Create(" YES :   " + yesCount.ToString() + "   " + "NO : " + noCount.ToString() + "  " +  "I DONT KNOW : " + donCount.ToString() , stringResult);

    }
    [PunRPC]
    public void CollectAnswer(string message,string username)
    {
        var Answer = message;

        switch(Answer
            )
        {
            case "Yes": yesCount ++; break;
                case "No": noCount ++; break;
                case "DonKnow": donCount ++; break;
        }
        Debug.Log("Yes Count :  " + yesCount + " No Count :  "+  noCount + " Don Count :  " +  donCount);
        submitAnswerCount++;
        tempAnswerSheet.Add(username, message);
            
    }

    [PunRPC]
    public void StartGame()
    {
      
    }

    [PunRPC]
    public void AnswerRefresh()
    {
        waitBox.gameObject.SetActive(false);
        questionShowSheet.gameObject.transform.parent.gameObject.SetActive(true);
        questionShowSheet.gameObject.SetActive(true);
        yesbtn.SetActive(true);
        nobtn.SetActive(true);
        donKnowbtn2.SetActive(true);
        questionShowSheet.text = QuestionSheet[OngoingQuestion];
        answerShowSheet.gameObject.transform.parent.gameObject.SetActive(false);
        answerShowSheet.gameObject.SetActive(false);
        quizbar_num.text=(OngoingQuestion +1).ToString();
    }

    [PunRPC]
 public void _Show(string message)
    {
        Showing(message);
    }
    [PunRPC]
    public void _ThirdExecute()
    {
        StartCoroutine(ThirdExecute());
    }
    public void  Showing(string message)
    {
        questionShowSheet.gameObject.transform.parent.gameObject.SetActive(false);
        answerShowSheet.gameObject.transform.parent.gameObject.SetActive(true);
        questionShowSheet.gameObject.SetActive(false);
        answerShowSheet.text = message;
        answerShowSheet.gameObject.SetActive(true);
       waitBox.gameObject.SetActive(false);  
        OngoingQuestion++;
        submitAnswerCount = 0;
    

    }
    IEnumerator ThirdExecute()
    {
        yield return new WaitUntil(() => _submitAnswerCount == totalUserCount);
        PhotonView pv = photonView;
        UpdateScoreBoard();
        pv.RPC("_Show", RpcTarget.All, SelectAnswer().Item1);
        yield return new WaitForSeconds(5);
        yesCount = 0;
        noCount = 0;
        donCount = 0;
        tempAnswerSheet = new Dictionary<string, string>();

        if (OngoingQuestion== QuestionSheet.Count)
        {
            var temp = Nwh.ScoreBoardDictionary.Select(x => x.ToString()).Aggregate((x, y) => x + " :  " + y);
            pv.RPC("ShowScoreBoard", RpcTarget.All,Nwh.ScoreBoardDictionary);
        }
        else {
        pv.RPC("AnswerRefresh", RpcTarget.All);
        _ThirdExecute();
        }
    }
    [PunRPC]  
    public void Dbinsert(string username)
    {
        if(PhotonNetwork.IsMasterClient == false) { 
        PhotonView pv = photonView;
        pv.RPC("Dbinsert", RpcTarget.MasterClient, username);
        }
        Nwh.ScoreBoardDictionary.Add(username, 0);
    }
    [PunRPC]
    void UpdateScoreBoard()
    {
        var answerSuccess = tempAnswerSheet.Where(x => x.Value == SelectAnswer().Item2).Select(x => x.Key).ToList();
        foreach( string element  in Nwh.ScoreBoardDictionary.Where(x=>answerSuccess.Contains(x.Key)).Select(x => x.Key).ToList())
        {
            Debug.Log(element);
            Nwh.ScoreBoardDictionary[element]= Nwh.ScoreBoardDictionary[element] + 1; 
        }
    }
    [PunRPC]
    public void ShowScoreBoard(Dictionary<string,int> scoredata)
    {
        answerShowSheet.gameObject.transform.parent.gameObject.SetActive(false);
        answerShowSheet.gameObject.SetActive(false);
        ScoreLeaderboard.transform.parent.gameObject.SetActive(true);
        ScoreLeaderboard.SetActive(true);
        ScoreLeaderboard.GetComponent<ScoreLeaderboard_cs>().inputData(scoredata);
      
        
    }
    [PunRPC]
    public void SynChronizeUserCount(int number)
    {
        totalUserCount = number;
    }
}
