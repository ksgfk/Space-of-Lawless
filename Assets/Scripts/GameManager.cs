using UnityEngine;

namespace KSGFK
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public static LoadManager Load => Instance._load;
        public static JobCenter Job => Instance._job;

        private LoadManager _load;
        private JobCenter _job;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            _load = GetComponent<LoadManager>();
            _job = new JobCenter();

            _load.Init();
            _load.Ready();

            _load.Work(() => Debug.Log("complete"));
        }

        private void Update() { _job.OnUpdate(); }
    }
}