using System;


    class DeepCharging
    {

        public static double deepCharging(double flux, int material)
        {
            double current = flux * 1.602e-19;
            switch(material)
            {
                case 1:
                    return current / 1e-17 * 10;
                case 2:
                    return current / 2.5e-15 * 10;
                case 3:
                    return current / 1e-15 * 10;
                default:
                    return 0;
            }
            
        }
        //static void Main(string[] args)
        //{
        //    Console.WriteLine(deepCharging(2.7e8, 3));//输入为电子通量中的最大值、深层充电器件编号（1为聚四氟乙烯，2为聚酰亚胺，3为环氧树脂），输出为深层充电电位（V）
        //}
    }

