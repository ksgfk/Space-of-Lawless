using KSGFK;
using MPipeline;
using NUnit.Framework;
using Unity.Collections;

namespace Tests
{
    public class TestMath
    {
        [Test]
        public unsafe void UnsafeFunc()
        {
            using (var arr = new NativeList<int>(10, Allocator.Temp))
            {
                arr.Add(9);
                arr.Add(1);
                arr.Add(4);
                arr.Add(2);
                arr.Add(0);
                arr.Add(6);
                arr.Add(5);
                arr.Add(3);
                arr.Add(7);
                arr.Add(8);

                MathExt.Quicksort(arr.unsafePtr, 0, arr.Length - 1);
                for (var i = 0; i < arr.Length; i++)
                {
                    Assert.True(arr[i] == i);
                }

                for (var i = 0; i < arr.Length; i++)
                {
                    Assert.True(MathExt.BinarySearch(arr.unsafePtr, 0, arr.Length - 1, i) == i);
                }
            }
        }
    }
}