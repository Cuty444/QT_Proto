using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Events;

namespace QT.Ranking
{
    [Serializable]
    public class RankingData
    {
        public string Name;
        public float TotalTime;

        public RankingData(string name, float time)
        {
            Name = name;
            TotalTime = time;
        }
    }
    [Serializable]
    public class RankingAllData
    {
        public List<RankingData> Data = new List<RankingData>();
    }
    public class RankingManager
    {

        private float _currentRankingTime = 0f;
        public RankingAllData _rankingAllData { get; private set; } = new();
        private readonly string rankingDataPath = "Ranking.json";

        public UnityEvent<float> RankingDeltaTimeUpdate { get;} = new();

        private bool isPlayerOn = false;
        public UnityEvent<bool> PlayerOn { get; } = new();
        public void Initialize()
        {
            RankingDeltaTimeUpdate.AddListener(RankingDeltaTime);
            //PlayerOn.AddListener((on) =>
            //{
            //    isPlayerOn = on;
            //});
        }

        //private void Update()
        //{
        //    if (isPlayerOn)
        //    {
        //        _currentRankingTime += Time.deltaTime;
        //    }
        //}

        public void DataLoad()
        {
            string path = Path.Combine(Application.dataPath, rankingDataPath);
            if (!Directory.Exists(Application.dataPath))
            {
                Directory.CreateDirectory(Application.dataPath);
            }

            if (!File.Exists(path))
            {
                return;
            }

            string loadData = File.ReadAllText(path);
            _rankingAllData = JsonUtility.FromJson<RankingAllData>(loadData);
        }

        public void DataSave()
        {
            string path = Path.Combine(Application.dataPath, rankingDataPath);
            string saveData = JsonUtility.ToJson(_rankingAllData);
            File.WriteAllText(path,saveData);
        }

        public void AddRankingData(string inputName)
        {
            _rankingAllData.Data.Add(new RankingData(inputName,_currentRankingTime));
            _currentRankingTime = 0f;
            DataSave();
        }

        public void ResetRankingTime()
        {
            _currentRankingTime = 0f;
        }

        public float GetRankingTime()
        {
            return _currentRankingTime;
        }
        public void RankingDeltaTime(float time)
        {
            _currentRankingTime += time;
        }
    }
}
