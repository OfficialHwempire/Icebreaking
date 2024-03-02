using System.Collections;
using System.Collections.Generic;
using CarterGames.Assets.LeaderboardManager;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreLeaderboard_cs: MonoBehaviour
{
    // Start is called before the first frame update

    public string ScoreboardId = "ScoreLeaderBoard";

    public void Start()
    {
        if (LeaderboardManager.BoardExists(ScoreboardId))
        {
            LeaderboardManager.DeleteLeaderboard(ScoreboardId);
            Debug.Log("Leaderboard exist");
        }
        LeaderboardManager.CreateLeaderboard(ScoreboardId);
        LeaderboardManager.ClearLeaderboard(ScoreboardId);
    }
    // Update is called once per frame
    public void inputData(Dictionary<string,int> inp)
    {
       
        foreach (string element in inp.Keys)
        {
            LeaderboardManager.AddEntryToBoard(ScoreboardId, element, inp[element]);
        }
        gameObject.GetComponent<LeaderboardDisplay>().UpdateDisplay();

    }
    public void ClearBoard()
    {
        LeaderboardManager.ClearLeaderboard(ScoreboardId);
    }
}
