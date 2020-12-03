/********************************************************************
	created:	2020/12/03
	created:	3:12:2020   12:50
	filename: 	Displacement.cs
	file path:	Space_Radiation
	file base:	Displacement
	file ext:	cs
	author:		Kaguya
	
	purpose:	通过Integral函数，输入质子能量和通量、器件编号（1-3），
                输出CCD相机的噪点数量（位移损伤的表现）。此处相机的
                像素为256*256，可自行在return处进行修改。
                也可通过getNIEL函数输入质子能量，输出质子非电离能损
*********************************************************************/
using System;

class displacementDamage
{
    public static double getDisplacementDamage(double[] energy, double[] flux, int material)
    {
        double result = 0;
        for(int i = 0; i < energy.Length - 1; i++)
        {
            //Console.Out.WriteLine(n.input(energy[i]));
            if (energy[i] == 0) continue;
            double[] w = { 0, 0 };
            switch (material)
            {
                case 1:
                    w[0] = Section_1(getNIEL(energy[i]));
                    w[1] = Section_1(getNIEL(energy[i + 1]));
                    break;
                case 2:
                    w[0] = Section_2(getNIEL(energy[i]));
                    w[1] = Section_2(getNIEL(energy[i + 1]));
                    break;
                case 3:
                    w[0] = Section_3(getNIEL(energy[i]));
                    w[1] = Section_3(getNIEL(energy[i + 1]));
                    break;
            }
            result += (flux[i] * w[0] + flux[i + 1] * w[1]) * Math.Abs(getNIEL(energy[i + 1]) - getNIEL(energy[i])) / 2;

        }
        return result * 256 * 256;
    }
    static double Section_1(double x)
    {
        //return 5.05e-7 * (1 - Math.Exp(Math.Pow((-x + 0.1725) / 60.0, 0.0975)));
        return 1e-3 * (1 - Math.Exp(-Math.Pow((x - 0.001) / 60.0, 1)));
    }

    static double Section_2(double x)
    {
        return 2e-4 * (1 - Math.Exp(-Math.Pow((x - 0.001) / 60.0, 1)));
    }
    static double Section_3(double x)
    {
        return 1e-4 * (1 - Math.Exp(-Math.Pow((x - 0.001) / 60.0, 1)));
    }
    public static double getNIEL(double x)
    {
        double[] iw = { -11.5772178845514, -8.88127201605640, -3.59371936318553, 3.15880004896414, 7.85575805562293 };
        double[] lw = { 0.0221103684350823, 0.130030957979784, 0.470082360480904, -0.426894841764735, 5.00525322882412 };
        double[] b1 = { 10.5172229042891, 4.41766107430249, 0.0947560961092802, 1.34147991249607, 9.45152940865414 };
        double b2 = -4.96372881763975;
        Neuron_network NIEL_network = new Neuron_network(iw, lw, b1, b2, 5, 4.0, -3.699, 0.7497, -2.9431, true, true);
        return NIEL_network.input(x);
    }
}

