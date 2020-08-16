using Unity.Jobs;

namespace KSGFK
{
    public static class Jobs
    {
        public static readonly JobTranslate Translate;
        public static readonly JobRotate RotateSingle;

        static Jobs()
        {
            Translate = new JobTranslate();
            RotateSingle = new JobRotate();
        }

        public static void Release()
        {
            Translate.Dispose();
            RotateSingle.Dispose();
        }

        public static unsafe void Update()
        {
            const int count = 2;
            var handlers = stackalloc JobHandle[count];
            handlers[0] = Translate.OnUpdate();
            handlers[1] = RotateSingle.OnUpdate();

            for (var i = 0; i < count; i++)
            {
                ref var handle = ref handlers[i];
                handle.Complete();
            }

            Translate.AfterUpdate();
            RotateSingle.AfterUpdate();
        }
    }
}