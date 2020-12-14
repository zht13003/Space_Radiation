/********************************************************************
	created:	2020/12/03
	created:	3:12:2020   12:57
	filename: 	SEE.cs
	file path:	Space_Radiation
	file base:	SEE
	file ext:	cs
	author:		Kaguya
	
	purpose:	通过getTotalSEE函数，输入质子能量、通量、高度、纬度、经度
                器件编号（1-3），输出SEE（次）。
                也可通过getLET函数，输入粒子能量（MeV）、粒子种类
                （事务ProtonLET或ElectronLET），输出LET(MeV*cm^2*g^-1)。
                同时提供shield函数，输入为屏蔽厚度、能量、通量、粒子种类，
                输出为屏蔽后的能量和通量。
*********************************************************************/

/*
 * 参考输出：
 * 高度：1000，SEE：2015.2154009826404
高度：2000，SEE：806314.5516935752
高度：3000，SEE：5600778.707162271
高度：4000，SEE：13032478.419344783
高度：5000，SEE：33247730.37662786
高度：6000，SEE：34001864.54273734
高度：7000，SEE：42106630.118137814
高度：8000，SEE：37562235.87833919
高度：9000，SEE：14775926.135272097
高度：10000，SEE：23452834.527665514
高度：11000，SEE：14807783.658314323
高度：12000，SEE：9155400.632970823
高度：13000，SEE：6283226.836764731
高度：14000，SEE：2338374.329370017
高度：15000，SEE：410168.3836275123
高度：16000，SEE：535456.7048023776
高度：17000，SEE：84593.69938210722
高度：18000，SEE：55658.14880911982
高度：19000，SEE：9541.295008096138
高度：20000，SEE：10857.812556866593
高度：21000，SEE：5259.855426453562
高度：22000，SEE：5107.710367773661
高度：23000，SEE：5108.34841559512
高度：24000，SEE：5059.936612934252
高度：25000，SEE：4968.02188313004
高度：26000，SEE：4981.418203010129
高度：27000，SEE：4970.557927354687
高度：28000，SEE：4955.924466944166
高度：29000，SEE：4968.90332401779
高度：30000，SEE：4968.758346070373
高度：31000，SEE：4961.672654634827
高度：32000，SEE：4956.566292283609
高度：33000，SEE：4956.2663543550225
高度：34000，SEE：4958.503462543823
高度：35000，SEE：4958.343844037409
 */
using System;

public delegate Neuron_network particle();
class SEE : Space_Radiation.IRadiation
{
    double singleEffectEvent;
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
        return 2e-8 * (1 - Math.Exp(-Math.Pow((x - 0.001) / 20.0, 1)));
    }
    static double Weibull_3(double x)
    {
        return 1e-9 * (1 - Math.Exp(-Math.Pow((x - 0.001) / 20.0, 1)));
    }
    static double Weibull_4(double x)
    {
        return 1e-1 * (1 - Math.Exp(-Math.Pow((x - 0) / 20.0, 1)));
    }

    public void calRadiation(double[] protonEnergy, double[] protonFlux, 
        double high, double latitude, double longitude, int instrument)
    {
        double singleEventFromTrapped = getSEE(protonEnergy, protonFlux, instrument);

        double[] cosmicRaysEnergy = new double[100];
        double[] cosmicRaysFlux = new double[100];
        for (int i = 0; i < 100; i++)
        {
            cosmicRaysEnergy[i] = 0.1 * (i + 1) * 1000;
            cosmicRaysFlux[i] = getAlpha(high, longitude, latitude, 0.5,
                        cosmicRaysEnergy[i] / 1000, 1, true) * 10000 * 6.28;
        }

        double singleEventFromCosmic = getSEE(cosmicRaysEnergy, cosmicRaysFlux, instrument);

        singleEffectEvent = singleEventFromCosmic + singleEventFromTrapped;
    }
    static double getAlpha(double h, double longtitude, double latitude, double phi, double Ek, int Z, bool model)
    /*输入经纬度(角度)、高度km、太阳活动常数（0.5~1.1）、能量GeV，得到宇宙线模型的对应能量的α粒子（Z=2）或质子（Z=1）通量
    通量单位为m^-2*s^-1*sr^-1*MeV^-1
    高度为自地表起的高度
    model为true表示原初宇宙线模型，false表示轨道宇宙线模型
    */
    {
        longtitude = longtitude * Math.PI / 180;
        latitude = latitude * Math.PI / 180;
        latitude = Math.Asin(Math.Sin(latitude) * Math.Cos(11.7 * Math.PI / 180)
            + Math.Cos(latitude) * Math.Sin(11.7 * Math.PI / 180) * Math.Cos(longtitude - 291 * Math.PI / 180));
        double Em = 0.93827 * (3 * Z - 2);
        double REarth = 6371;
        int r = 12;
        double R = Math.Sqrt((Math.Pow(Ek + phi, 2) + 2 * Em * (Ek + phi))) / Z;
        double Unmod = 23.9 * Math.Pow(R, -2.83);
        double numerator = Math.Pow(Ek + Em, 2) - Math.Pow(Em, 2);
        double denominator = Math.Pow(Ek + Em + Z * phi * 2, 2) - Math.Pow(Em, 2);
        double RCut = 14.9 * Math.Pow(1 + h / REarth, -2) * Math.Pow(Math.Cos(latitude), 4);
        double Primary_alpha;
        if (model)
        {
            Primary_alpha = Unmod * (numerator / denominator) / (1 + Math.Pow(R / RCut, -r));
        }
        else
        {
            Primary_alpha = Unmod * (numerator / denominator);
        }
        return Primary_alpha;
    }
    static double getSEE(double[] e1, double[] f1, int material)
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
                case 4:
                    w[0] = Weibull_4(n.input(e1[i]));
                    w[1] = Weibull_4(n.input(e1[i + 1]));
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
    public double getRadiation()
    {
        return singleEffectEvent;
    }

    public void calRadiation(double[] energy, double[] flux, int instrument)
    {
        throw new NotImplementedException();
    }
    public void calRadiation(double[] energy, double[] flux, double shield)
    {
        throw new NotImplementedException();
    }
}

