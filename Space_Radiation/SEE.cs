using System;

    public delegate Neuron_network particle();
    class SEE
    {
        public static double Weibull_1(double x)
        {
            double[] iw = { -6.94788897658893, 7.06466860254654, 6.48566799525212, -7.16499577994326, -20.0488220313215 };
            double[] lw = { 0.0564593834794398, 0.0888485956702163, 0.0569588186398885, -0.0882099550386574, -6.22203776041643 };
            double[] b1 = { 7.02625749805833, -3.00547732132522, 0.934092266745334, -4.29365188842030, -20.7167007472045 };
            double b2 = -5.49547459813758;
            Neuron_network weibull = new Neuron_network(iw, lw, b1, b2, 5, 112.0, 1.1, -6.8327, -11.1791, false, false);
            return Math.Pow(10, weibull.input(x));
        }
        public static double Weibull_2(double x)
        {
            return 5e-8 * (1 - Math.Exp(-Math.Pow((x - 0.001) / 20.0, 1)));
        }
        public static double Weibull_3(double x)
        {
            return 1e-8 * (1 - Math.Exp(-Math.Pow((x - 0.001) / 20.0, 1)));
        }
        public static double getSEE(double[] e1, double[] f1, double[] e2, double[] f2, int material)
        {
            Neuron_network n1 = ElectronLET();
            Neuron_network n2 = ProtonLET();
            double result = 0;
            for(int i = 0; i < e1.Length - 1; i++)
            {
                if (e1[i] == 0) continue;
                double[] w = { 0, 0 };
                switch(material)
                {
                    case 1:
                        w[0] = Weibull_1(n1.input(e1[i]));
                        w[1] = Weibull_1(n1.input(e1[i + 1]));
                        break;
                    case 2:
                        w[0] = Weibull_2(n1.input(e1[i]));
                        w[1] = Weibull_2(n1.input(e1[i + 1]));
                        break;
                    case 3:
                        w[0] = Weibull_3(n1.input(e1[i]));
                        w[1] = Weibull_3(n1.input(e1[i + 1]));
                        break;
                }
                result += (f1[i] * w[0] + f1[i + 1] * w[1]) * Math.Abs(n1.input(e1[i + 1]) - n1.input(e1[i])) / 2;
            }
            for (int i = 0; i < e2.Length - 1; i++)
            {
                if (e2[i] == 0) continue;
                double[] w = { 0, 0 };
                switch (material)
                {
                    case 1:
                        w[0] = Weibull_1(n2.input(e2[i]));
                        w[1] = Weibull_1(n2.input(e2[i + 1]));
                        break;
                    case 2:
                        w[0] = Weibull_2(n2.input(e2[i]));
                        w[1] = Weibull_2(n2.input(e2[i + 1]));
                        break;
                    case 3:
                        w[0] = Weibull_3(n2.input(e2[i]));
                        w[1] = Weibull_3(n2.input(e2[i + 1]));
                        break;
                }
                result += (f2[i] * w[0] + f2[i + 1] * w[1]) * Math.Abs(n2.input(e2[i + 1]) - n2.input(e2[i])) / 2;
            }

            return result;
        }
        public static Neuron_network ElectronLET()
        {
            double[] iw2 = { 4.40554824978072, -2.81064364523242, -3.03512283712394, 9.91858288567110, 2.60301088165564 };
            double[] lw2 = { 11.4180773930414, -2.74604567663715, -0.0162365504655352, -0.721185700576192, -1.80969138100576 };
            double[] b12 = { -6.13262944318743, 3.39853818000505, -0.415136851360895, 12.1386316411607, 3.32427909745692 };
            double b22 = 15.6828753002702;
            Neuron_network LET_electron = new Neuron_network(iw2, lw2, b12, b22, 5, 3.0, -2.0, 46.7, 1.53, true, false);
            return LET_electron;
        }
        public static Neuron_network ProtonLET()
        {
            double[] iw = { 6.86733640551202, 5.08313763261494, -44.3418366892818, -1.99877898702244, -4.72829769037351 };
            double[] lw = { -0.0273795368754960, 0.0398208891139150, 0.00355504412093733, 2.24473564071721, -1.59960731323085 };
            double[] b1 = { -4.63511389379558, -3.35984406955077, 4.47247412693705, -1.27467476368094, -4.14729630697173 };
            double b2 = -0.367810983024666;
            Neuron_network LET_proton = new Neuron_network(iw, lw, b1, b2, 5, 3.0, -2.0, 538, 1.805, true, false);
            return LET_proton;
        }
        public static double getLET(double x, particle getParticle)
        {
            if (x == 0)
                return 0;
            return getParticle().input(x);
        }
        public static void shield(double s, double[] e, double[] f, particle getParticle)
        {
            for (int i = 0; i < e.Length; i++)
            {
                e[i] -= s * getParticle().input(e[i]);
                if (e[i] <= 0)//如果被完全屏蔽
                {
                    e[i] = 0;
                    f[i] = 0;
                }
            
                //Console.Out.WriteLine(e[i]);
            }
    }
        //static void Main(string[] args)
        //{

        //    double[] e1 = { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        //    double[] f1 = { 3474554.201,2003286.918,1210558.131,731523.2661,485458.0051,322162.651,231458.8502,166292.3966,124765.9677,93609.4915};
        //    double[] e2 = { 0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.07, 0.08, 0.09, 0.1, 0.11, 0.12, 0.13, 0.14, 0.15, 0.16, 0.17, 0.18, 0.19, 0.2 };
        //    double[] f2 = { 99832423.24, 93194235.55, 86997442.9, 81212695.46, 75812594.02, 70771565.05, 66065728.99, 61672801.17, 57571970.62, 53743820.9, 51104932.75, 48595617.27, 46209512.29, 43940566.37, 41783028.58, 39731431.63, 37780567.73, 35925493.72, 34161508.78, 32484135.18 };
        //    double LET = getLET(10, ProtonLET);//输入为粒子能量（MeV）、粒子种类（ProtonLET或ElectronLET），输出为LET(MeV*cm^2*g^-1)
        //    //Console.Out.WriteLine(LET);
        //    //增加屏蔽厚度会减小电子和质子的能量，具体如下
        //    double s = 2;//屏蔽厚度，mm
        //    //shield(s, ref e1, ref f1, ProtonLET);//输入为屏蔽厚度、能量、通量、粒子种类，输出为屏蔽后的能量和通量
        //    //shield(s, ref e2, ref f2, ElectronLET);
        //    Console.Out.WriteLine(getSEE(e1, f1, e2, f2, 1));//输入为质子能量、通量、电子能量、通量、器件编号（1-3），输出为SEE（次）
        //}
    }

