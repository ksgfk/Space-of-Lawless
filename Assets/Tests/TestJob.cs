// using KSGFK;
// using NUnit.Framework;
// using Unity.Mathematics;
//
// namespace Tests
// {
//     public class TestJob
//     {
//         [Test]
//         public void TestJobWithEnumeratorPasses()
//         {
//             var job = new JobTranslate();
//             var a = job.AddValue(new TranslateData {Velocity = new float2(1, 0)});
//             var b = job.AddValue(new TranslateData {Velocity = new float2(0, 1)});
//             var c = job.AddValue(new TranslateData {Velocity = new float2(-1, 0)});
//             var d = job.AddValue(new TranslateData {Velocity = new float2(0, -1)});
//             Assert.True(a.Index == 0);
//             Assert.True(b.Index == 1);
//             Assert.True(c.Index == 2);
//             Assert.True(d.Index == 3);
//             
//             job.OnUpdate();
//             b.Release();
//             
//             job.OnUpdate();
//             Assert.True(a.Index == 0);
//             Assert.True(b.Index == -1);
//             Assert.True(c.Index == 2);
//             Assert.True(d.Index == 1);
//             
//             job.Dispose();
//         }
//     }
// }