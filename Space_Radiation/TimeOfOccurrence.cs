using System;

namespace Space_Radiation
{
    class TimeOfOccurrence
    {
        //正态分布
        public static double normalDistribution(double x, double miu, double sigma)
        {
            double y = 1 / (Math.Sqrt(2 * Math.PI) * sigma) * Math.Exp(-(Math.Pow((x - miu), 2)) / (2 * sigma * sigma));
            double max = 1 / (Math.Sqrt(2 * Math.PI) * sigma);
            return x >= 0 ? (x <= miu ? y : max) : 0;
        }
        //输入效应值、效应阈值、可能的最大效应值
        //输出效应发生的年份
        public static double timeOfOccurrence(double effectValue, double threshold, double maxEffectValue)
        {
            if(effectValue < threshold)
            {
                //不发生效应
                return -1;
            }
            double miu = -10 / (maxEffectValue - threshold) * (effectValue - threshold) + 10;
            Random r = new Random();
            //寿命为10年，每隔0.01年进行一次是否发生效应的判定
            for(double time = 0; time <= 10; time += 0.01)
            {
                double possible = r.NextDouble() * 0.3;
                if(possible < normalDistribution(time, miu, 2))
                {
                    return time;
                }
            }
            return 10;
        }
        public static void Main(string[] args)
        {
            SpaceRadiation s = new SpaceRadiation(4000, 4000, 0);
            for(int i = 0;i < 10;i++)
                Console.WriteLine(timeOfOccurrence(s.getSEE(), 4300, 5e7));
        }
    }
}
