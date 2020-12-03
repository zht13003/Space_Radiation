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
}

