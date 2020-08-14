using KSGFK;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

namespace Tests
{
    public class TestJob
    {
        [Test]
        public void TestJobPasses()
        {
            var go = new GameObject();
            var t = go.transform;
            t.position = Vector3.zero;
            t.rotation = Quaternion.AngleAxis(90, new Vector3(0, 0, 1));
            var up = new Vector3(0, 1, 0);
            var p = t.TransformDirection(up);
            Debug.Log($"{p.x},{p.y},{p.z}");
            
            var a = new Vector3(0, 1, 0);
            var c = MathExt.TransformDirection(t.rotation, a);
            Debug.Log($"{c.x},{c.y},{c.z}");

            var mq = quaternion.AxisAngle(new float3(0, 0, 1), 90);
            var mp = new float3(0, 1, 0);
            Debug.Log(MathExt.TransformDirection(mq, mp));
        }
    }
}