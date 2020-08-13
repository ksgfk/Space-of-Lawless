// using MPipeline;
// using Unity.Burst;
// using Unity.Jobs;
// using Unity.Mathematics;
// using UnityEngine;
//
// namespace KSGFK
// {
//     public struct TranslateData
//     {
//         public float2 Velocity;
//         public float2 Position;
//     }
//
//     public class JobTranslate : ListJob<TranslateData>
//     {
//         [BurstCompile]
//         private struct Translate : IJobParallelFor
//         {
//             public NativeList<TranslateData> Data;
//             public float DeltaTime;
//
//             public void Execute(int index)
//             {
//                 ref var value = ref Data[index];
//                 var mov = value.Velocity * DeltaTime;
//                 value.Position += mov;
//             }
//         }
//
//         public override void OnUpdate()
//         {
//             var handle = new Translate
//             {
//                 Data = DataList,
//                 DeltaTime = Time.deltaTime
//             }.Schedule(DataList.Length, 4);
//             handle.Complete();
//         }
//     }
// }