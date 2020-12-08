/********************************************************************
	created:	2020/12/03
	created:	3:12:2020   12:57
	filename: 	SEE.cs
	file path:	Space_Radiation
	file base:	SEE
	file ext:	cs
	author:		Kaguya
	
	purpose:	通过SEE函数，输入质子能量、通量、电子能量、通量、
                器件编号（1-3），输出SEE（次）。
                也可通过getLET函数，输入粒子能量（MeV）、粒子种类
                （事务ProtonLET或ElectronLET），输出LET(MeV*cm^2*g^-1)。
                同时提供shield函数，输入为屏蔽厚度、能量、通量、粒子种类，
                输出为屏蔽后的能量和通量。
*********************************************************************/
using System;

public delegate Neuron_network particle();
class SEE
{
    static double Weibull_1(double x)
    {
        double[] iw = { -6.94788897658893, 7.06466860254654, 6.48566799525212, -7.16499577994326, -20.0488220313215 };
        double[] lw = { 0.0564593834794398, 0.0888485956702163, 0.0569588186398885, -0.0882099550386574, -6.22203776041643 };
        double[] b1 = { 7.02625749805833, -3.00547732132522, 0.934092266745334, -4.29365188842030, -20.7167007472045 };
        double b2 = -5.49547459813758;
        Neuron_network weibull = new Neuron_network(iw, lw, b1, b2, 5, 112.0, 1.1, -6.8327, -11.1791, false, false);
        return Math.Pow(10, weibull.input(x));
    }
    static double Weibull_2(double x)
    {
        return 5e-8 * (1 - Math.Exp(-Math.Pow((x - 0.001) / 20.0, 1)));
    }
    static double Weibull_3(double x)
    {
        return 1e-8 * (1 - Math.Exp(-Math.Pow((x - 0.001) / 20.0, 1)));
    }
    public static double getSEE(double[] e1, double[] f1, int material)
    {
        Neuron_network n = ProtonLET();
        double result = 0;
        for(int i = 0; i < e1.Length - 1; i++)
        {
            if (e1[i] == 0) continue;
            double[] w = { 0, 0 };
            switch(material)
            {
                case 1:
                    w[0] = Weibull_1(n.input(e1[i]));
                    w[1] = Weibull_1(n.input(e1[i + 1]));
                    break;
                case 2:
                    w[0] = Weibull_2(n.input(e1[i]));
                    w[1] = Weibull_2(n.input(e1[i + 1]));
                    break;
                case 3:
                    w[0] = Weibull_3(n.input(e1[i]));
                    w[1] = Weibull_3(n.input(e1[i + 1]));
                    break;
            }
            result += (f1[i] * w[0] + f1[i + 1] * w[1]) * Math.Abs(n.input(e1[i + 1]) - n.input(e1[i])) / 2;
        }

        return result * 6e6;
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
            
        }
    }
}

