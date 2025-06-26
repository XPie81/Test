public static class RandomExtensions
{
    public static double NextGaussian(this Random random, double mean = 0, double standardDeviation = 1)
    {
        double u1 = random.NextDouble(); // [0, 1)
        double u2 = random.NextDouble(); // [0, 1)
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); // N(0, 1)
        return mean + standardDeviation * randStdNormal; // N(mean, stdDev^2)
    }
}