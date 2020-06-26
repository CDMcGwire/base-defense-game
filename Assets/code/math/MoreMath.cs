namespace math {
public static class MoreMath {
	public static int AvgIter(int total, int next, int n) => total + (next - total) / n;

	public static long AvgIter(long total, long next, long n) => total + (next - total) / n;
}
}