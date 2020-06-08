using UnityEngine;

namespace KSGFK
{
    /// <summary>
    /// TODO:子弹击中消失要手动移除JobTimingTask的任务
    /// </summary>
    public class EntityBulletBallistic : EntityBullet,
        IJobCallback<JobTemplateForTransform<DataMoveWithTrans>>,
        IJobTimingTask
    {
        public string moveJobName = "DefaultMoveWithTrans";
        public string taskJobName = "Task";
        [SerializeField] private int jobDataId = -1;
        [SerializeField] private int taskId = -1;
        private JobTemplateForTransform<DataMoveWithTrans> _moveJob;
        private JobTimingTask _taskJob;

        int IJobCallback<JobTemplateForTransform<DataMoveWithTrans>>.DataId
        {
            get => jobDataId;
            set => jobDataId = value;
        }

        int IJobCallback<JobTimingTask>.DataId { get => taskId; set => taskId = value; }
        JobTimingTask IJobCallback<JobTimingTask>.Job { get => _taskJob; set => _taskJob = value; }

        JobTemplateForTransform<DataMoveWithTrans> IJobCallback<JobTemplateForTransform<DataMoveWithTrans>>.Job
        {
            get => _moveJob;
            set => _moveJob = value;
        }

        void IJobTimingTask.RunTask() { GameManager.Entity.DestroyEntity(this); }

        public override void Launch(Vector2 direction, Vector2 startPos, float speed, float duration)
        {
            var data = new DataMoveWithTrans
            {
                Direction = direction,
                Speed = speed
            };
            _moveJob.AddData(transform, data, this);
            var trans = transform;
            trans.rotation = MathExt.FromToRotation(Vector3.up, direction);
            trans.position = startPos;
            _taskJob.AddTask(duration, this);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            _moveJob = GameManager.Job.GetJob<JobMoveForTransform>(moveJobName);
            _taskJob = GameManager.Job.GetJob<JobTimingTask>(taskJobName);
        }

        public override void OnRemoveFromWorld()
        {
            base.OnRemoveFromWorld();
            _moveJob.RemoveData(this);
        }
    }
}