public class Utils {
    public static float InverseLerpUnclamped(float a, float b, float value) {
        return (float)(value - a) / (b - a);
    }
}
