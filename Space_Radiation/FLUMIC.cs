using System;
using System.Collections.Generic;
using System.Text;

namespace Space_Radiation
{
    class FLUMIC
    {
        public static double getFlumicFlux(double E, double h, double longitude, double latitude)
        {
			double a0 = 9.479, a1 = -0.3977, b1 = 0.4176, a2 = -0.1049, b2 = 0.09918, a3 = -0.04197,
				b3 = 0.06262, a4 = -0.02242, b4 = 0.04196, w = 14.08;
			longitude = longitude * Math.PI / 180;
			latitude = latitude * Math.PI / 180;
			double altrad = Math.Asin(Math.Sin(latitude) * Math.Cos(11.7 * Math.PI / 180)
				+ Math.Cos(latitude) * Math.Sin(11.7 * Math.PI / 180) * Math.Cos(longitude - 291 * Math.PI / 180));
			double RE = (h + 6370) / 6370;
			double BetaValueF = 30610.0 * Math.Sqrt(Math.Cos(1.57 - altrad) * Math.Cos(1.57 - altrad) * 3.0 + 1.0) / (RE * RE * RE);
			double bottomF = Math.Pow(RE, 6);
			bottomF = 4 - (BetaValueF * BetaValueF * bottomF / (3.06e4 * 3.06e4));
			double x = 3 * Math.Sqrt(RE * RE) / bottomF;
			if (x >= 2.5)
			{
				double E_Bigger_Than_2MeV = a0 + a1 * Math.Cos(x * w) + b1 * Math.Sin(x * w) +
					a2 * Math.Cos(2 * x * w) + b2 * Math.Sin(2 * x * w) + a3 * Math.Cos(3 * x * w) + b3 * Math.Sin(3 * x * w) +
					a4 * Math.Cos(4 * x * w) + b4 * Math.Sin(4 * x * w);
				double E0 = 0.25 + 0.11 * Math.Pow(E_Bigger_Than_2MeV - 7, 1.3);
				double fs = 0.85, fo = 0.25;
				double fsc = 8e8 * (0.625 + 0.375 * Math.Sin((2 * 360 * (fs - 0.7) * Math.PI / 180)) + 0.125 * Math.Sin((4 * 360 * (fs - 0.15) * Math.PI / 180)));
				double foy = fsc * (0.625 - 0.375 * Math.Cos((4 * 360 * (fo + 0.03) * Math.PI / 180)) - 0.125 * Math.Cos((2 * 360 * (fo + 0.03) * Math.PI / 180)));
				return foy * Math.Exp((2 - E) / E0) * 16 * Math.Tanh(0.6 * (x - 2.5)) / Math.Cosh(1.5 * (x - 4.3));
			}
			else
			{
				double p = 2.12 + 45.4 / Math.Pow(x + 0.05, 2) - 45.6 / Math.Pow(x + 0.05, 3);
				double E_Bigger_Than_1MeV = 4 * Math.Pow(10, p);
				return E_Bigger_Than_1MeV * Math.Exp((1 - E) / 0.14);
			}
		}
    }
}
