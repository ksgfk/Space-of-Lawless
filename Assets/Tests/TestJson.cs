using KSGFK;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class TestJson
    {
        [Test]
        public void TestJsonSimplePasses()
        {
            // var o = new DataReadPath
            // {
            //     Datas = new[]
            //     {
            //         new DataReadPath.Data("a", typeof(string).FullName)
            //     }
            // };
            // var str = JsonUtility.ToJson(o);
            // Debug.Log(str);

            var o = JsonUtility.FromJson<MetaData>("{\"DataFiles\":[{\"Path\":\"a\",\"Type\":\"System.String\"}]}");
            Assert.IsTrue(o.EntityInfo.Length == 1);
        }
    }
}