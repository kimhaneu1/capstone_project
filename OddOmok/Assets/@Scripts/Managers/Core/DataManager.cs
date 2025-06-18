using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using System.Text;
using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;

public class DataSettings
{
    public string Name = "";
    public int MMR = 1000;
    public int BlackGamePlayed = 0;
    public int WhiteGamePlayed = 0;
    public int BlackWins = 0;
    public int WhiteWins = 0;
    public List<string> CollectedStoneNames = new List<string> { "Slime", "Mong", "Pudding", "Penguin", "Cloud", "Goblin" };
}

public class DataManager
{
    public DataSettings dataSettings = new DataSettings();
    public event Action<string> OnStatusUpdated;
    public event Action OnDataLoaded;

    private string fileName = "file.dat";

    #region Update

    public void UpdateGameCount()
    {
        var playerType = GameServerManager.GameServer.GetPlayerType();

        if (playerType == PlayerCharacter.PlayerType.BlackPlayer) dataSettings.BlackGamePlayed++;
        else if (playerType == PlayerCharacter.PlayerType.WhitePlayer) dataSettings.WhiteGamePlayed++;
    }

    public int opponentMMR; // 임시 저장용
    public void UpdateMMR(int opponentMMR, bool winner)
    {
        int myMMR = Managers.Data.dataSettings.MMR;
        int mmrDifference = Mathf.Min(Mathf.Abs(myMMR - opponentMMR), 1000);
        int mmrChange;

        if (winner)
        {
            // 일단 이전에 깎였던 점수를 복구해줘야 함 (패배 시 계산한 점수)
            int recoverLoss;
            if (opponentMMR < myMMR)
            {
                recoverLoss = 20 + Mathf.RoundToInt((20f * mmrDifference) / 1000f);
            }
            else
            {
                recoverLoss = 20 - Mathf.RoundToInt((19f * mmrDifference) / 1000f);
            }

            Managers.Data.dataSettings.MMR += recoverLoss;

            // 기본 +30, 나보다 높은 상대일수록 보상 증가
            if (opponentMMR > myMMR)
            {
                mmrChange = 30 + Mathf.RoundToInt((30f * mmrDifference) / 1000f); // 최대 +60
            }
            else
            {
                mmrChange = 30 - Mathf.RoundToInt((29f * mmrDifference) / 1000f); // 최소 +1
            }
            Managers.Data.dataSettings.MMR += mmrChange;
        }
        else
        {
            // 기본 -20, 나보다 낮은 상대에게 질수록 손해 증가
            if (opponentMMR < myMMR)
            {
                mmrChange = 20 + Mathf.RoundToInt((20f * mmrDifference) / 1000f); // 최대 -40
            }
            else
            {
                mmrChange = 20 - Mathf.RoundToInt((19f * mmrDifference) / 1000f); // 최소 -1
            }

            Managers.Data.dataSettings.MMR -= mmrChange;

            if (Managers.Data.dataSettings.MMR < 0)
                Managers.Data.dataSettings.MMR = 0;
        }

        SaveData();
    }

    public (int gain, int loss) PredictMMRChanges(int opponentMMR, bool isOpponent)
    {
        this.opponentMMR = opponentMMR;
        int myMMR = dataSettings.MMR;

        if (isOpponent)
        {
            int temp = opponentMMR;
            opponentMMR = myMMR;
            myMMR = temp;
        }

        int mmrDifference = Mathf.Min(Mathf.Abs(myMMR - opponentMMR), 1000);

        // 이겼을 때
        int winGain;
        if (opponentMMR > myMMR)
            winGain = 30 + Mathf.RoundToInt((30f * mmrDifference) / 1000f);
        else
            winGain = 30 - Mathf.RoundToInt((29f * mmrDifference) / 1000f);

        // 졌을 때
        int loss;
        if (opponentMMR < myMMR)
            loss = 20 + Mathf.RoundToInt((20f * mmrDifference) / 1000f);
        else
            loss = 20 - Mathf.RoundToInt((19f * mmrDifference) / 1000f);

        return (winGain, loss);
    }


    #endregion

    #region Save
    public void SaveData()
    {
        OpenSaveGame();
        Managers.GPGS.AddRank();
    }

    private void OpenSaveGame()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(fileName,
                                            DataSource.ReadCacheOrNetwork,
                                            ConflictResolutionStrategy.UseLastKnownGood,
                                            OnSavedGameOpend);
    }

    private void OnSavedGameOpend(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("저장 성공");

            SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();

            // JSON
            var json = JsonUtility.ToJson(dataSettings);
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            Debug.Log("저장 데이터 : " + bytes);

            savedGameClient.CommitUpdate(game, update, bytes, OnSavedGameWritten);
        }
        else
        {
            Debug.Log("저장 실패");
        }
    }

    private void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("저장 성공");
        }
        else
        {
            Debug.Log("저장 실패");
        }
    }
    #endregion

    #region Load
    public void LoadData()
    {
        OpenLoadGame();
    }

    private void OpenLoadGame()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(fileName,
                                            DataSource.ReadCacheOrNetwork,
                                            ConflictResolutionStrategy.UseLastKnownGood,
                                            LoadGameData);
    }

    private void LoadGameData(SavedGameRequestStatus status, ISavedGameMetadata data)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("로드 성공");
            OnStatusUpdated?.Invoke($"게임 데이터 로드 성공: {data.Filename}");

            savedGameClient.ReadBinaryData(data, OnSavedGameDataRead);
        }
        else
        {
            Debug.Log("로드 실패");
            OnStatusUpdated?.Invoke($"게임 데이터 로드 실패: {data?.Filename ?? "파일 없음"}");
        }
    }

    private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] loadedData)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            string data = System.Text.Encoding.UTF8.GetString(loadedData);

            if (string.IsNullOrEmpty(data))
            {
                Debug.Log("데이터 없음 초기 데이터 저장");
                OnStatusUpdated?.Invoke("데이터 없음, 초기 데이터 생성 중");
                Managers.Data.dataSettings.Name = PlayGamesPlatform.Instance.GetUserDisplayName();
                SaveData();
            }
            else
            {
                Debug.Log("로드 데이터 : " + data);
                OnStatusUpdated?.Invoke("게임 데이터 로드 완료");
                dataSettings = JsonUtility.FromJson<DataSettings>(data);
            }

            OnDataLoaded?.Invoke();
        }
        else
        {
            Debug.Log("데이터 읽기 실패");
            OnStatusUpdated?.Invoke("게임 데이터 읽기 실패");
        }
    }
    #endregion

    #region Delete
    public void DeleteData()
    {
        DeleteGameData();
    }

    private void DeleteGameData()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(fileName,
                                            DataSource.ReadCacheOrNetwork,
                                            ConflictResolutionStrategy.UseLastKnownGood,
                                            DeleteSaveGame);
    }

    private void DeleteSaveGame(SavedGameRequestStatus status, ISavedGameMetadata data)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (status == SavedGameRequestStatus.Success)
        {
            savedGameClient.Delete(data);

            Debug.Log("삭제 성공");
        }
        else
        {
            Debug.Log("삭제 실패");
        }
    }
    #endregion
}