using System;

namespace Space_Radiation
{
    class TimeOfOccurrence
    {
        //正态分布，只取单调递增的前半部分，后半部分保持为最大值
        public static double normalDistribution(double x, double miu, double sigma)
        {
            double y = 1 / (Math.Sqrt(2 * Math.PI) * sigma) * Math.Exp(-(Math.Pow((x - miu), 2)) / (2 * sigma * sigma));
            double max = 1 / (Math.Sqrt(2 * Math.PI) * sigma);
            return x >= 0 ? (x <= miu ? y : max) : 0;
        }
        //输入效应值、效应阈值、该效应可能的最大值
        //输出效应发生的年份
        public static double timeOfOccurrence(double effectValue, double threshold, double maxEffectValue)
        {
            if(effectValue < threshold)
            {
                //不发生效应
                return -1;
            }
            //效应值超过阈值越多，μ越大，代表效应发生的年份越可能早
            //效应值=阈值，μ=10，代表在第10年发生效应的概率最大
            //效应值=该效应可能的最大值，μ=0，代表在第0年发生效应的概率最大
            double miu = -10 / (maxEffectValue - threshold) * (effectValue - threshold) + 10;
            Random r = new Random();
            //寿命为10年，每隔0.01年进行一次是否发生效应的判定
            for(double time = 0; time <= 10; time += 0.01)
            {
                //0~0.3的随机数。正态分布的最大值为0.2，若该随机数小于正态分布即表示发生效应
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
                Console.WriteLine("发生效应的年份：" + timeOfOccurrence(s.getSEE(), 4300, 5e7));
        }
    }
}
