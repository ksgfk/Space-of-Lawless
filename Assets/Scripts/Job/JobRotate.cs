using MPipeline;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;

namespace KSGFK
{
    public class JobRotate //: JobTemplate<DataRotate>
    {
        // [BurstCompile]
        // private struct Rotate : IJobParallelFor
        // {
        //     public NativeList<DataRotate> DataList;
        //     public float DeltaTime;
        //
        //     public void Execute(int index)
        //     {
        //         ref var data = ref DataList[index];
        //         ref var delta = ref data.Delta;
        //         if (math.abs(delta.x) < 0.0000001f && math.abs(delta.y) < 0.0000001f)
        //         {
        //             return;
        //         }
        //
        //         var vec = new float3(delta, 0);
        //         var result = MathExt.FromToRotation(new float3(0, 1, 0), vec);
        //         data.Rotation = math.slerp(data.Rotation, result, data.Speed * DeltaTime);
        //     }
        // }
        //
        // public JobRotate(string name) : base(name) { }
        //
        // public override void OnUpdate(float deltaTime)
        // {
        //     new Rotate
        //     {
        //         DataList = DataList,
        //         DeltaTime = deltaTime
        //     }.Run(DataList.Length);
        // }
    }
}