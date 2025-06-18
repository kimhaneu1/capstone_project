using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using UnityEngine;

public class GPGSManager
{
    public event Action<string> OnStatusUpdated;

    public void GPGS_LogIn()
    {
        OnStatusUpdated?.Invoke("Signing in to Google Play...");
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        OnStatusUpdated?.Invoke($"로그인 상태: {status}");

        if (status == SignInStatus.Success)
        {
            Debug.Log($"로그인 성공");

            Managers.Data.LoadData();
        }
        else
        {
            Debug.Log($"로그인 실패");
        }
    }

    public enum LeaderboardType
    {
        MMR,
        Total,
        Black,
        White
    }
    public void ShowLeaderboardUI(LeaderboardType type)
    {
        switch (type)
        {
            case LeaderboardType.MMR:
                PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_mmr_ranking);
                break;
            case LeaderboardType.Total:
                PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_total_wins);
                break;
            case LeaderboardType.Black:
                PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_black_wins);
                break;
            case LeaderboardType.White:
                PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_white_wins);
                break;
        }
    }


    public void AddRank()
    {
        int totlaWins = Managers.Data.dataSettings.BlackWins + Managers.Data.dataSettings.WhiteWins;
        // 리더보드 추가
        PlayGamesPlatform.Instance.ReportScore(Managers.Data.dataSettings.MMR, GPGSIds.leaderboard_mmr_ranking, (bool success) => { });
        PlayGamesPlatform.Instance.ReportScore(totlaWins, GPGSIds.leaderboard_total_wins, (bool success) => { });
        PlayGamesPlatform.Instance.ReportScore(Managers.Data.dataSettings.BlackWins, GPGSIds.leaderboard_black_wins, (bool success) => { });
        PlayGamesPlatform.Instance.ReportScore(Managers.Data.dataSettings.WhiteWins, GPGSIds.leaderboard_white_wins, (bool success) => { });
    }

    public void ShowAchievementsUI()
    {
        // 전체 업적 표시
        PlayGamesPlatform.Instance.ShowAchievementsUI();
    }

    public void IncrementGPGSAchievement()
    {
        // 단계별 업적 증가
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_oddomok, 1, (bool success) => { });
    }

    public void UnlockingGPGSAchievement()
    {
        // 업적 잠금 해제 및 공개
        PlayGamesPlatform.Instance.UnlockAchievement(GPGSIds.achievement_oddomok2, (bool success) => { });
    }
}
