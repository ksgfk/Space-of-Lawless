//源码来源:https://github.com/MaxwellGengYF/Unity-MPipeline
//作者MaxwellGengYF

using System;
using System.Runtime.CompilerServices;
using KSGFK;
using Unity.Collections;
using static Unity.Collections.LowLevel.Unsafe.UnsafeUtility;
using Unity.Collections.LowLevel.Unsafe;

namespace MPipeline
{
    public interface IFunction<A, R>
    {
        R Run(ref A a);
    }

    public static unsafe class MUnsafeUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Resize<T>(ref this NativeArray<T> arr, int targetLength, Allocator alloc) where T : unmanaged
        {
            if (targetLength <= arr.Length) return;
            NativeArray<T> newArr = new NativeArray<T>(targetLength, alloc);
            MemCpy(newArr.GetUnsafePtr(), arr.GetUnsafePtr(), sizeof(T) * arr.Length);
            arr.Dispose();
            arr = newArr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* Ptr<T>(this NativeArray<T> arr) where T : unmanaged { return (T*) arr.GetUnsafePtr(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char* Ptr(this string arr)
        {
            fixed (char* c = arr)
            {
                return c;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPtr<T>(ref this NativeArray<T> arr, void* targetPtr) where T : unmanaged
        {
            ulong* ptr = (ulong*) (AddressOf(ref arr));
            *ptr = (ulong) targetPtr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Element<T>(this NativeArray<T> arr, int index) where T : unmanaged
        {
            return ref *((T*) arr.GetUnsafePtr() + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyFrom<T>(this T[] array, T* source, int length) where T : unmanaged
        {
            fixed (T* dest = array)
            {
                MemCpy(dest, source, length * sizeof(T));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeFree(ref void* ptr, Allocator alloc)
        {
            if (ptr != null)
            {
                UnsafeUtility.Free(ptr, alloc);
                ptr = null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeFree<T>(ref T* ptr, Allocator alloc) where T : unmanaged
        {
            if (ptr != null)
            {
                UnsafeUtility.Free(ptr, alloc);
                ptr = null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* Ptr<T>(this T[] array) where T : unmanaged { return (T*) AddressOf(ref array[0]); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* Ptr<T>(ref this T array) where T : unmanaged { return (T*) AddressOf(ref array); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<T>(this T[] array, T* dest, int length) where T : unmanaged
        {
            fixed (T* source = array)
            {
                MemCpy(dest, source, length * sizeof(T));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* Malloc<T>(long size, Allocator allocator) where T : unmanaged
        {
            long align = size % 16;
            return (T*) UnsafeUtility.Malloc(size, align == 0 ? 16 : (int) align, allocator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* Malloc(long size, Allocator allocator)
        {
            long align = size % 16;
            return UnsafeUtility.Malloc(size, align == 0 ? 16 : (int) align, allocator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* Cast<T>(void* ptr) where T : unmanaged { return (T*) ptr; }
    }
}