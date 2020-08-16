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
            Vector3 start = Vector3.right;
            Vector3 axis = Vector3.forward;
            Vector3 endU = new Vector3(-1, -1, -1);
            float3 endN = new float3(-1, -1, -1);
            var resU = Vector3.SignedAngle(start, endU, axis);
            var resN = MathExt.SignedAngle(start, endN, axis);
            Assert.True(resU - resN <= 0.000001f);
            Debug.Log(resU);
            Debug.Log(resN);
        }
    }
}