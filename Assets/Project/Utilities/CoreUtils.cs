namespace Utilities {
    public static class CoreUtils {
        public static float InverseLerpUnclamped(float a, float b, float value) {
            return (float)(value - a) / (b - a);
        }
    }

    public interface IInitializable {
        bool IsInitialized { get; set; }
        void Initialise();
        void DeInitialise();
    }
}