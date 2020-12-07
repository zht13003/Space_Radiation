/********************************************************************
	created:	2020/12/03
	created:	3:12:2020   12:26
	filename: 	Position.cs
	file path:	Space_Radiation
	file base:	Position
	file ext:	cs
	author:		Kaguya
	
	purpose:	输入轨道根数和时间初始化对象，然后通过getPosition函数
				返回航天器纬度（°）、经度（°）、高度（km）
*********************************************************************/
using System;
using One_Sgp4;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;

class Position
{
	String dip;
	String meridian;
	String e;
	String perihelion;
	String a;
	String flat;
	double aDouble;
	double T;
	String TTime;
	String first;
	String second;
	public double[] ECI;
	public int[] data;
	double[] velocity;
	private static double EARTH_1 = 6378.137;
	private static double Temp_1 = EARTH_1 * (1.0 - (1.0 / 298.257223563));
	private static double Eccsq = 1 - Math.Pow(Temp_1, 2) / Math.Pow(EARTH_1, 2);
	private static double Dtr = Math.PI / 180.0;

	public Position(double dip, double meridian, double e, double perihelion, double a, double flat, int[] time)
    {
        this.dip = Convert.ToString(dip);
        this.meridian = Convert.ToString(meridian);
        this.e = Convert.ToString(e);
        this.perihelion = Convert.ToString(perihelion);
        this.a = Convert.ToString(a);
        this.flat = Convert.ToString(flat);
        data = time;
    }
    public void calculateDate()
	{
		aDouble = Convert.ToDouble(a);
		T = 2 * Math.PI * Math.Sqrt(Math.Pow(aDouble, 3) / 6.67e-11 / 5.965e24);
		T = 24 * 60 * 60 / T;
		TTime = T.ToString("#0.00000000");

		first = "1 25544U 98067A   19364.04305556 -.00001219  00000-0 -13621-4 0  9993";
		second = "2 25544  " + dip + " " + meridian + " " + e + "  " + perihelion + " " + flat
			+ " " + TTime + "00000";
		int checksum = 0;
		int sum1 = 0;
		for (int i = 0; i < second.Count(); i++)
		{
			if (char.IsNumber(second[i]))
			{
				sum1 += (int)Char.GetNumericValue(second[i]);
			}
		}
		checksum = sum1 % 10;
		second += checksum.ToString();
	}
	public void addTime(int second)
    {
        int minute = second / 60;
        second -= minute * 60;
        int hour = minute / 60;
        minute -= hour * 60;
        data[5] += second;
        data[4] += minute;
        data[3] += hour;
        if (data[5] >= 60)
        {
            data[5] = 0;
            data[4]++;
        }
        if (data[4] >= 60)
        {
            data[4] = 0;
            data[3]++;
        }
        if (data[3] >= 24)
        {
            data[3] = 0;
            data[2]++;
        }
    }
	/*****************************************************************************
	* @function name : calECIDateAndVelocity
	* @author : Kaguya
	* @date : 2020/12/3 12:38
	* @inparam : 
	* @outparam : 
	* @last change : 
	* @usage : 基于one_sgp4项目，计算航天器基于ECI坐标系下的位置和速度
	*****************************************************************************/
    public void calECIDateAndVelocity()
	{
		Tle tleISS = ParserTLE.parseTle(first, second, "ISS 1");
		DateTime t = new DateTime(data[0], data[1], data[2], data[3], data[4], data[5]);
		EpochTime startTime = new EpochTime(t);
		Sgp4Data sate = SatFunctions.getSatPositionAtTime(tleISS, startTime, Sgp4.wgsConstant.WGS_84);
		One_Sgp4.Point3d position = sate.getPositionData();
		ECI = new double[3];
		ECI[0] = position.x;
		ECI[1] = position.y;
		ECI[2] = position.z;

		velocity = new double[3];
		velocity[0] = sate.getVelocityData().x;
		velocity[1] = sate.getVelocityData().y;
		velocity[2] = sate.getVelocityData().z;
		velocity = getECEF(velocity, data);
	}

	static double juliandate(int[] time)
	{
		int year;
		int month;
		int A;
		int B;
		double C;

		if (time[1] < 3)
		{
			year = time[0] - 1;
			month = time[1] + 12;
		}
		else
		{
			year = time[0];
			month = time[1];
		}
		A = (int)(year / 100.0);
		B = 2 - A + (int)(A / 4.0);
		C = ((time[5] / 60.0 + time[4]) / 60.0 + time[3]) / 24.0;
		return (int)(365.25 * (year + 4716)) + (int)(30.6001 * (month + 1)) + time[2] + B - 1524.5 + C;
	}

	static double greenwichsrt(double Jdate)
	{
		double tUT1;
		double gmst_sec;
		tUT1 = (Jdate - 2451545.0) / 36525.0;
		gmst_sec = 67310.54841 + (876600.0 * 3600.0 + 8640184.812866) * tUT1
			+ 0.093104 * Math.Pow(tUT1, 2) - 6.2e-6 * Math.Pow(tUT1, 3);
		return gmst_sec * (2 * Math.PI) / 86400.0 % (2 * Math.PI);
	}
	/*****************************************************************************
	* @function name : getECEF
	* @author : Kaguya
	* @date : 2020/12/3 12:39
	* @inparam : ECI——ECI坐标系下的位置
	* @inparam : time——时间，分别为年月日时分秒
	* @outparam : 
	* @last change : 
	* @usage : 将ECI坐标系根据时间转化为ECEF坐标系
	*****************************************************************************/
	public static double[] getECEF(double[] ECI, int[] time)
	{
		var eci = new DenseMatrix(1, 3);
		eci[0, 0] = ECI[0];
		eci[0, 1] = ECI[1];
		eci[0, 2] = ECI[2];
		double[] zero = { 0, 0, 0 };
		var ecef = new DenseMatrix(1, 3, zero);
		double gst = greenwichsrt(juliandate(time));

		ecef = (DenseMatrix)(R3(gst).Transpose() * eci.Transpose());

		double[] result = new double[3];
		result[0] = ecef[0, 0];
		result[1] = ecef[1, 0];
		result[2] = ecef[2, 0];
		return result;
	}


	static DenseMatrix R3(double x)
	{
		double[] r = { Math.Cos(x), Math.Sin(x), 0, -Math.Sin(x), Math.Cos(x), 0, 0, 0, 1 };
		var a = new DenseMatrix(3, 3, r);
		return a;
	}
	private static double[] Radcur(double lati)
	{
		double slat, dsq, rn, rho, z;
		slat = Math.Sin(Dtr * lati);
		dsq = 1.0 - Eccsq * Math.Pow(slat, 2);
		rn = EARTH_1 / Math.Sqrt(dsq);
		rho = rn * Math.Cos(Dtr * lati);
		z = (1.0 - Eccsq) * rn * slat;
		return (new double[] { Math.Sqrt(Math.Pow(rho, 2) + Math.Pow(z, 2)), rn, rn * (1.0 - Eccsq) / dsq });
	}

	private static double Gc2gd(double flatgci, double altkmi)
	{
		double[] rrnrm = Radcur(flatgci);
		double ratio = 1 - Math.Pow(Math.Sqrt(Eccsq), 2) * rrnrm[1] / (rrnrm[1] + altkmi);
		double tlat = Math.Tan(Dtr * flatgci) / ratio;
		rrnrm = Radcur((1 / Dtr) * Math.Atan(tlat));
		ratio = 1 - Math.Pow(Math.Sqrt(Eccsq), 2) * rrnrm[1] / (rrnrm[1] + altkmi);
		tlat = Math.Tan(Dtr * flatgci) / ratio;
		return (1 / Dtr) * Math.Atan(tlat);
	}
	/*****************************************************************************
	* @function name : XYZ2LLA
	* @author : Kaguya
	* @date : 2020/12/3 12:41
	* @inparam : ECEF——ECEF坐标系下的位置
	* @outparam : 航天器纬度、经度、高度
	* @last change : 
	* @usage : 将ECEF坐标系下的位置转化为纬度、经度、高度
	*****************************************************************************/
	public static double[] XYZ2LLA(double[] ECEF)
	{
		double flatgc, flatn, dlat, rp, x, y, z, p, tangd, rn, clat, slat, flat, flon, altkm;
		x = ECEF[0];
		y = ECEF[1];
		z = ECEF[2];

		rp = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
		flatgc = Math.Asin(z / rp) / Dtr;
		flon = ((Math.Abs(x) + Math.Abs(y)) < 1.0e-10) ? 0.0 : Math.Atan2(y, x) / Dtr;
		flon = (flon < 0.0) ? flon + 360.0 : flon;
		p = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

		if (p < 1.0e-10)
		{
			flat = ((z < 0.0)) ? -90.0 : 90.0;
			altkm = rp - Radcur(flat)[0];
		}
		else
		{
			altkm = rp - Radcur(flatgc)[0];
			flat = Gc2gd(flatgc, altkm);
			rn = Radcur(flat)[1];
			for (double kount = 0; kount < 5; kount++)
			{
				slat = Math.Sin(Dtr * flat);
				tangd = (z + rn * Eccsq * slat) / p;
				flatn = Math.Atan(tangd) / Dtr;
				dlat = flatn - flat;
				flat = flatn;
				clat = Math.Cos(Dtr * flat);
				rn = Radcur(flat)[1];
				altkm = (p / clat) - rn;
				if (Math.Abs(dlat) < 1.0e-12)
				{
					break;
				}
			}
		}
		return new double[] { flat, flon, altkm };
	}
	public double[] getPosition()
    {
		calculateDate();
		calECIDateAndVelocity();
		return XYZ2LLA(getECEF(ECI, data));
    }

	public int[] getTime() { return data; }
}


