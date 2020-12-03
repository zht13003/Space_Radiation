/********************************************************************
	created:	2020/12/03
	created:	3:12:2020   13:00
	filename: 	Spectrum.cs
	file path:	Space_Radiation
	file base:	Spectrum
	file ext:	cs
	author:		Kaguya
	
	purpose:	提供getTheatM函数
*********************************************************************/
using System;

class Spectrum
{

    public static double getTheatM(double longtitude, double latitude)
    //地理纬度转化为地磁纬度，输入单位为角度，输出单位为弧度
    {
        longtitude = longtitude * Math.PI / 180;
        latitude = latitude * Math.PI / 180;
        return Math.Asin(Math.Sin(latitude) * Math.Cos(11.7 * Math.PI / 180) 
            + Math.Cos(latitude) * Math.Sin(11.7 * Math.PI / 180) * Math.Cos(longtitude - 291 * Math.PI / 180));
    }

    static double getAlpha(double h, double longtitude, double latitude, double phi, double Ek, int Z, bool model)
        /*输入经纬度(角度)、高度km、太阳活动常数（0.5~1.1）、能量GeV，得到宇宙线模型的对应能量的α粒子（Z=2）或质子（Z=1）通量
        通量单位为m^-2*s^-1*sr^-1*MeV^-1
        高度为自地表起的高度
        model为true表示原初宇宙线模型，false表示轨道宇宙线模型
        */
    {
        latitude = getTheatM(longtitude, latitude);
        double Em = 0.93827 * (3 * Z - 2);
        double REarth = 6371;
        int r = 12;
        double R = Math.Sqrt((Math.Pow(Ek + phi, 2) + 2 * Em * (Ek + phi))) / Z;
        double Unmod = 23.9 * Math.Pow(R, -2.83);
        double numerator = Math.Pow(Ek + Em, 2) - Math.Pow(Em, 2);
        double denominator = Math.Pow(Ek + Em + Z * phi * 2, 2) - Math.Pow(Em, 2);
        double RCut = 14.9 * Math.Pow(1 + h / REarth, -2) * Math.Pow(Math.Cos(latitude), 4);
        double Primary_alpha;
        Console.Out.WriteLine(Math.Pow(R / RCut, -r));
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
    //static void Main(string[] args)
    //{

    //    /*
    //     MAX模型
    //     输入为高度（km，自地表起）与地磁纬度（°）
    //     输出为4*510的矩阵，第一行为500项递增的电子能量（MeV），第二行为对应的电子通量（cm^-2*s^-1），
    //     第三行为质子能量，第四行为对应的质子通量
    //     */
    //    double high = 25000;//高度较低时（大约<1000左右），会出现输出全为1的情况
    //    double latitude = 0, longtitude = 50;

    //    maxModel model1 = new maxModel();
    //    double[,] flux = model1.getFlux(high, getTheatM(longtitude, latitude) * 180 / Math.PI); ;
    //    for (int i = 0; i < 20; i++)
    //    {
                
    //            flux = model1.getFlux(high, getTheatM(0, 0) * 180 / Math.PI);
    //            Console.Out.WriteLine(String.Format("{0}    {1}",
    //                flux[0, i], flux[1, i]));
                
    //    }
    //    //MIN模型，同上
    //    minModel model2 = new minModel();
    //    flux = model2.getFlux(high, getTheatM(longtitude, latitude) * 180 / Math.PI);
    //    for (int i = 0; i < 500; i++)
    //    {
    //        if (i <= 9)
    //        {
    //            //flux[3, i] *= 1.5e3;
    //        }
    //        //Console.Out.WriteLine(String.Format("{0} {1}",flux[2, i], flux[3, i]));
    //    }
    //    //宇宙线模型
    //    double E = 1;
    //    for (high = 10000; high < 20000; high += 1000)
    //    {
    //        //Console.Out.WriteLine(String.Format("原初宇宙线模型能量为{0}GeV的α粒子的通量为{1}m^-2*s^-1*sr^-1*MeV^-1，{2}km",
    //            //E, getAlpha(high, longtitude, latitude, 0.8, E, 2, true), high));
    //    }

    //}
}

