using UnityEngine;

namespace KSGFK
{
    /// <summary>
    /// TODO:子弹击中消失要手动移除JobTimingTask的任务
    /// </summary>
    [DisallowMultipleComponent]
    public class EntityBulletBallistic : EntityBullet
    {
        public string moveJobName = "DefaultMoveWithTrans";
        public string taskJobName = "Task";
        [SerializeField] private JobInfo moveInfo = JobInfo.Default;
        [SerializeField] private JobInfo taskInfo = JobInfo.Default;
        private JobWrapperImpl<JobMoveForTransformInitReq, DataMoveForTransform> _moveJob;
        private JobWrapperImpl<JobTimingTaskInitReq, float> _taskJob;

        public override void Launch(Vector2 direction, Vector2 startPos, float speed, float duration)
        {
            var trans = transform;
            moveInfo = _moveJob.AddData(new JobMoveForTransformInitReq
            {
                Direction = direction,
                Speed = speed,
                Trans = trans
            });
            trans.rotation = MathExt.FromToRotation(Vector3.up, direction);
            trans.position = startPos;
            taskInfo = _taskJob.AddData(new JobTimingTaskInitReq
            {
                Task = () => GameManager.Instance.Entity.DestroyEntity(this),
                Duration = duration
            });
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            if (_moveJob == null)
            {
                _moveJob = GameManager.Instance
                    .Job
                    .GetJob<JobMoveForTransformInitReq, DataMoveForTransform>(moveJobName);
            }

            if (_taskJob == null)
            {
                _taskJob = GameManager.Instance.Job.GetJob<JobTimingTaskInitReq, float>(taskJobName);
            }
        }

        public override void OnRemoveFromWorld()
        {
            base.OnRemoveFromWorld();
            _moveJob.RemoveData(moveInfo);
        }
    }
}