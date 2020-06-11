using System.Collections;
using KSGFK;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestJob
    {
        private JobTimingTask _job;

        [UnityTest]
        public IEnumerator TestJobWithEnumeratorPasses()
        {
            _job = new JobTimingTask();
            _job.AddData(new JobTimingTaskInitReq {Duration = 3, Task = () => Debug.Log("3")});
            _job.AddData(new JobTimingTaskInitReq {Duration = 2, Task = () => Debug.Log("2")});
            _job.AddData(new JobTimingTaskInitReq {Duration = 1, Task = () => Debug.Log("1")});
            while (_job.TaskCount > 0)
            {
                Assert.True(_job.JobCount == _job.TaskCount);
                Assert.True(_job.JobCount == _job.DataCount);
                _job.OnUpdate();
                yield return null;
            }

            _job.Dispose();
        }
    }
}