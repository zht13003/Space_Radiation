/********************************************************************
	created:	2020/12/03
	created:	3:12:2020   12:43
	filename: 	DeepCharging.cs
	file path:	Space_Radiation
	file base:	DeepCharging
	file ext:	cs
	author:		Kaguya
	
	purpose:	输入电子能谱中的最大值和器件编号（1-3），返回深层充电电位
*********************************************************************/

using System;

class DeepCharging : Space_Radiation.IRadiation
{
    double deepCharging;
    public double getRadiation()
    {
        return deepCharging;
    }

    public void calRadiation(double[] energy, double[] flux, int instrument)
    {
        double temp = 0;
        for (int i = 0; i < flux.Length; i++)
        {
            if (flux[i] == 0) continue;
            temp = flux[i];
            if (temp != 0) break;
        }

        double current = temp * 1.602e-19;
        switch (instrument)
        {
            case 1:
                deepCharging = current / 1e-17 * 10;
                break;
            case 2:
                deepCharging = current / 2.5e-15 * 10;
                break;
            case 3:
                deepCharging = current / 1e-15 * 10;
                break;
            default:
                deepCharging = 0;
                break;
        }
    }
    public void calRadiation(double[] energy, double[] flux, double high,
            double latitude, double longitude, int instrument)
    { 
        throw new NotImplementedException();
    }

    public void calRadiation(double[] energy, double[] flux, double shield)
    {
        throw new NotImplementedException();
    }
}

