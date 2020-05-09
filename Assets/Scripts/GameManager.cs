using System;
using System.IO;
using Cinemachine;
using UnityEngine;

namespace KSGFK
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public static LoadManager Load => Instance._load;
        public static JobCenter Job => Instance._job;
        public static DataCenter Data => Instance._data;
        public static Camera MainCamera => Instance.mainCamera;

        public Camera mainCamera;
        public CinemachineVirtualCamera virtualCamera;
        private LoadManager _load;
        private JobCenter _job;
        private DataCenter _data;

        /// <summary>
        /// 该阶段读取游戏数据
        /// </summary>
        public event Action PerInit;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            _load = GetComponent<LoadManager>();
            _load.Init();
            _load.Ready();
            _job = new JobCenter();
            _data = new DataCenter();

            _data.AddDataLoader("WindowsPlayer.csv", new CsvLoader(this));
            _data.AddPath(typeof(ShipFrameData),
                Path.Combine(Application.streamingAssetsPath,
                    "EntityData",
                    "ship_frame.csv"));
            PerInit?.Invoke();
            _data.StartLoad();

            _load.Work();
        }

        private void Update() { _job.OnUpdate(); }

        public static void SetCameraFollowTarget(Transform target) { Instance.virtualCamera.Follow = target; }
    }
}