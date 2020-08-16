using Unity.Jobs;

namespace KSGFK
{
    public static class Jobs
    {
        private static readonly JobWrapper[] Wrappers;
        public static readonly PersistentJobTranslate TranslatePersist;
        public static readonly TempJobRotate RotateTemp;

        static Jobs()
        {
            Wrappers = new JobWrapper[]
            {
                TranslatePersist = new PersistentJobTranslate(),
                RotateTemp = new TempJobRotate()
            };
        }

        public static void Release()
        {
            foreach (var wrapper in Wrappers)
            {
                wrapper.Dispose();
            }
        }

        public static unsafe void Update()
        {
            var handles = stackalloc JobHandle[Wrappers.Length];
            for (var i = 0; i < Wrappers.Length; i++)
            {
                handles[i] = Wrappers[i].OnUpdate();
            }

            for (var i = 0; i < Wrappers.Length; i++)
            {
                ref var handle = ref handles[i];
                handle.Complete();
            }

            foreach (var wrapper in Wrappers)
            {
                wrapper.AfterUpdate();
            }
        }
    }
}