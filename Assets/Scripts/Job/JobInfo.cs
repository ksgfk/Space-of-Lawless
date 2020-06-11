using System;
using UnityEngine;

namespace KSGFK
{
    [Serializable]
    public class JobInfo
    {
        [SerializeField] private int id;

        public IJobWrapper Job { get; internal set; }
        public int Id { get => id; internal set => id = value; }

        public static JobInfo Default => new JobInfo(default, -1);

        public JobInfo(IJobWrapper job, int initId)
        {
            Job = job;
            Id = initId;
        }
    }
}