using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Cinemachine;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

namespace KSGFK
{
    public enum GameState
    {
        PreInit,
        Init,
        PostInit,
        Running,
        Pause,
        Exit
    }

    /// <summary>
    /// TODO:World管理场景中实体，而不是用EntityManager
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static readonly string DataRoot = Path.Combine(Application.streamingAssetsPath, "Data");
        public static GameManager Instance { get; private set; }
        public LoadManager Load => _load;
        public JobCenter Job => _job;
        public DataCenter TempData => _data;
        public Camera MainCamera => mainCamera;
        public InputActionAsset InputAsset => playerInput;
        public EntityManager Entity => _entity;
        public InputCenter Input => _input;
        public PoolCenter Pool => _pool;
        public Canvas UiCanvas => uiCanvas;
        public GameState NowState => nowState;
        public MetaData MetaData => _meta;
        public RegisterCenter Register => _register;

        [SerializeField] private GameState nowState = GameState.PreInit;
        [SerializeField] private AssetReference playerInputAddr = null;
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private CinemachineVirtualCamera virtualCamera = null;
        [SerializeField] private Canvas uiCanvas = null;
        private LoadManager _load;
        private JobCenter _job;
        private DataCenter _data;
        private EntityManager _entity;
        private InputCenter _input;
        [SerializeField] private InputActionAsset playerInput = null;
        private PoolCenter _pool;
        private MetaData _meta;
        private RegisterCenter _register;

        /// <summary>
        /// 读取游戏数据
        /// </summary>
        public event Action<GameManager> PerInit;

        /// <summary>
        /// 加载所需资源
        /// </summary>
        public event Action<GameManager> Init;

        /// <summary>
        /// 后期处理
        /// </summary>
        public event Action<GameManager> PostInit;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            using (var reader = new StreamReader(Path.Combine(Application.streamingAssetsPath, "metadata.json")))
            {
                var str = reader.ReadToEnd();
                _meta = JsonUtility.FromJson<MetaData>(str);
            }

            _load = GetComponent<LoadManager>();
            _job = new JobCenter(this);
            _data = new DataCenter(this);
            _pool = new PoolCenter();
            _data.AddDataLoader(new CsvWinLoader());
            _entity = GetComponent<EntityManager>();
            AddAllDataPath();
            _register = new RegisterCenter(this);

            InvokePerInit();
        }

        private void Update()
        {
            switch (nowState)
            {
                case GameState.Running:
                    _job.OnUpdate();
                    break;
                case GameState.Init when _load.NowState == LoadState.Sleep:
                    OnPerInitComplete();
                    break;
                case GameState.PostInit when _load.NowState == LoadState.Sleep:
                    OnInitComplete();
                    break;
            }
        }

        public static void SetCameraFollowTarget(Transform target) { Instance.virtualCamera.Follow = target; }

        private void InvokePerInit()
        {
            _load.Init();
            _load.Ready();
            _entity.Init(this);
            PerInit?.Invoke(this);
            _load.Complete += () => nowState = GameState.Init;
            _data.StartLoad();
            _load.Work();
        }

        private void OnPerInitComplete()
        {
            _load.Ready();
            LoadPlayerInput();
            _load.Complete += () => nowState = GameState.PostInit;
            Init?.Invoke(this);
            _load.Request("panel.debug",
                (GameObject prefab) =>
                {
                    var go = Instantiate(prefab, Instance.uiCanvas.transform);
                    var debug = go.GetComponent<PanelDebug>();
                    debug.Init();
                });
            _load.Work();
        }

        private void OnInitComplete()
        {
            PostInit?.Invoke(this);
            _input = new InputCenter();
            nowState = GameState.Running;
            _input.Enable();
            PerInit = null;
            Init = null;
            PostInit = null;
            _data = null;
        }

        private void LoadPlayerInput()
        {
            var req = playerInputAddr.LoadAssetAsync<InputActionAsset>();
            req.Completed += request => playerInput = request.Result;
            _load.Request(req);
        }

        private void AddAllDataPath()
        {
            var metadataType = typeof(MetaData);
            var publicFields = metadataType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in publicFields.Where(field => field.FieldType == typeof(MetaData.Info[])))
            {
                AddDataPath((MetaData.Info[]) field.GetValue(_meta));
            }
        }

        private void AddDataPath(IEnumerable<MetaData.Info> infos)
        {
            foreach (var info in infos)
            {
                try
                {
                    _data.AddPath(Type.GetType(info.Type), GetDataPath(info.Path));
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        private void OnDestroy() { _job.Dispose(); }

        public static string GetDataPath(string fileName) { return Path.Combine(DataRoot, fileName); }
    }
}