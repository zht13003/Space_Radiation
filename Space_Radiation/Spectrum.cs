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
	double[] protonEnergy;
	double[] protonFlux;
    double[] electronEnergy;
    double[] electronFlux;

	public Spectrum()
    {
        protonEnergy = new double[10];
        protonFlux = new double[10];
        electronEnergy = new double[500];
        electronFlux = new double[500];
    }
	public void reflashEnergy()
    {

        for (int i = 0; i < 10; i++)
        {
            protonEnergy[i] = 3 + i;
        }

        for (int i = 0; i < 500; i++)
        {
            electronEnergy[i] = 0.01 + 0.01 * i;
        }
    }
	public void calFlux(double high, double latitude, double longitude)
    {
		maxModel model = new maxModel();
		double[,] spectrum = model.getFlux(high, Space_Radiation.Tools.getTheatM(longitude, latitude) * 180 / Math.PI);

        for (int i = 0; i < 10; i++)
        {
            protonEnergy[i] = 3 + i;
            protonFlux[i] = spectrum[3, i];
        }

        for (int i = 0; i < 500; i++)
        {
            electronEnergy[i] = 0.01 + 0.01 * i;
            electronFlux[i] = spectrum[1, i];
        }
    }
    public double[] getProtonFlux()
    {
        return protonFlux;
    }
    public double[] getProtonEnergy()
    {
        return protonEnergy;
    }
    public double[] getElectronFlux()
    {
        return electronFlux;
    }
    public double[] getElectronEnergy()
    {
        return electronEnergy;
    }
}

