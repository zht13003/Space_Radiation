/********************************************************************
	created:	2020/12/07
	created:	7:12:2020   13:05
	filename: 	Geomagnetic.cs
	file path:	Space_Radiation
	file base:	Geomagnetic
	file ext:	cs
	author:		Kaguya
	
	purpose:	用于计算地磁场XYZ三个分量，单位 nT，基于IGRF2010模型
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

class Geomagnetic
{
    #region
    const double PI = 3.1415926535;
    const double a = 6378.16;   /* major radius (km) IAU66 ellipsoid */
    const double f = 1.0 / 298.25;  /* inverse flattening IAU66 ellipsoid */
    const double b = 6378.16 * (1.0 - 1.0 / 298.25);
    /* minor radius b=a*(1-f) */
    const double r_0 = 6371.2;  /* "mean radius" for spherical harmonic expansion */

    static double[,] gnm_igrf2010 = {
    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
    { -29496.5,-1585.9,0,0,0,0,0,0,0,0,0,0,0,0 },
    { -2396.6,3026,1668.6,0,0,0,0,0,0,0,0,0,0,0 },
    { 1339.7,-2326.3,1231.7,634.2,0,0,0,0,0,0,0,0,0,0 },
    { 912.6,809,166.6,-357.1,89.7,0,0,0,0,0,0,0,0,0 },
    { -231.1,357.2,200.3,-141.2,-163.1,-7.7,0,0,0,0,0,0,0,0 },
    { 72.8,68.6,76,-141.4,-22.9,13.1,-77.9,0,0,0,0,0,0,0 },
    { 80.4,-75,-4.7,45.3,14,10.4,1.6,4.9,0,0,0,0,0,0 },
    { 24.3,8.2,-14.5,-5.7,-19.3,11.6,10.9,-14.1,-3.7,0,0,0,0,0 },
    { 5.4,9.4,3.4,-5.3,3.1,-12.4,-0.8,8.4,-8.4,-10.1,0,0,0,0 },
    { -2,-6.3,0.9,-1.1,-0.2,2.5,-0.3,2.2,3.1,-1,-2.8,0,0,0 },
    { 3,-1.5,-2.1,1.6,-0.5,0.5,-0.8,0.4,1.8,0.2,0.8,3.8,0,0 },
    { -2.1,-0.2,0.3,1,-0.7,0.9,-0.1,0.5,-0.4,-0.4,0.2,-0.8,0,0 },
    { -0.2,-0.9,0.3,0.4,-0.4,1.1,-0.3,0.8,-0.2,0.4,0,0.4,-0.3,0 }
    };
    static double[,] gtnm_igrf2010 = {
    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
    { 11.4,16.7,0,0,0,0,0,0,0,0,0,0,0,0 },
    { -11.3,-3.9,2.7,0,0,0,0,0,0,0,0,0,0,0 },
    { 1.3,-3.9,-2.9,-8.1,0,0,0,0,0,0,0,0,0,0 },
    { -1.4,2,-8.9,4.4,-2.3,0,0,0,0,0,0,0,0,0 },
    { -0.5,0.5,-1.5,-0.7,1.3,1.4,0,0,0,0,0,0,0,0 },
    { -0.3,-0.3,-0.3,1.9,-1.6,-0.2,1.8,0,0,0,0,0,0,0 },
    { 0.2,-0.1,-0.6,1.4,0.3,0.1,-0.8,0.4,0,0,0,0,0,0 },
    { -0.1,0.1,-0.5,0.3,-0.3,0.3,0.2,-0.5,0.2,0,0,0,0,0 },
    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0 }
    };
    static double[,] hnm_igrf2010 = {
    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
    { 0,4945.1,0,0,0,0,0,0,0,0,0,0,0,0 },
    { 0,-2707.7,-575.4,0,0,0,0,0,0,0,0,0,0,0 },
    { 0,-160.5,251.7,-536.8,0,0,0,0,0,0,0,0,0,0 },
    { 0,286.4,-211.2,164.4,-309.2,0,0,0,0,0,0,0,0,0 },
    { 0,44.7,188.9,-118.1,0.1,100.9,0,0,0,0,0,0,0,0 },
    { 0,-20.8,44.2,61.5,-66.3,3.1,54.9,0,0,0,0,0,0,0 },
    { 0,-57.8,-21.2,6.6,24.9,7,-27.7,-3.4,0,0,0,0,0,0 },
    { 0,10.9,-20,11.9,-17.4,16.7,7.1,-10.8,1.7,0,0,0,0,0 },
    { 0,-20.5,11.6,12.8,-7.2,-7.4,8,2.2,-6.1,7,0,0,0,0 },
    { 0,2.8,-0.1,4.7,4.4,-7.2,-1,-4,-2,-2,-8.3,0,0,0 },
    { 0,0.1,1.7,-0.6,-1.8,0.9,-0.4,-2.5,-1.3,-2.1,-1.9,-1.8,0,0 },
    { 0,-0.8,0.3,2.2,-2.5,0.5,0.6,0,0.1,0.3,-0.9,-0.2,0.8,0 },
    { 0,-0.8,0.3,1.7,-0.6,-1.2,-0.1,0.5,0.1,0.5,0.4,-0.2,-0.5,0 }
    };
    static double[,] htnm_igrf2010 = {
    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
    { 0,-28.8,0,0,0,0,0,0,0,0,0,0,0,0 },
    { 0,-23,-12.9,0,0,0,0,0,0,0,0,0,0,0 },
    { 0,8.6,-2.9,-2.1,0,0,0,0,0,0,0,0,0,0 },
    { 0,0.4,3.2,3.6,-0.8,0,0,0,0,0,0,0,0,0 },
    { 0,0.5,1.5,0.9,3.7,-0.6,0,0,0,0,0,0,0,0 },
    { 0,-0.1,-2.1,-0.4,-0.5,0.8,0.5,0,0,0,0,0,0,0 },
    { 0,0.6,0.3,-0.2,-0.1,-0.8,-0.3,0.2,0,0,0,0,0,0 },
    { 0,0,0.2,0.5,0.4,0.1,-0.1,0.4,0.4,0,0,0,0,0 },
    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0 }
    };
    #endregion
    public static long yymmdd_to_julian_days(int yy, int mm, int dd)
    {
        long jd;

        yy = (yy < 50) ? (2000 + yy) : (1900 + yy);
        jd = dd - 32075L + 1461L * (yy + 4800L + (mm - 14) / 12) / 4;
        jd = jd + 367L * (mm - 2 - (mm - 14) / 12 * 12) / 12;
        jd = jd - 3 * ((yy + 4900L + (mm - 14) / 12) / 100) / 4;

        return jd;
    }
    public static double deg_to_rad(double deg)
    {
        return deg * PI / 180.0;
    }
    public static double rad_to_deg(double rad)
    {
        return rad * 180.0 / PI;
    }
    /*****************************************************************************
    * @function name : getGeomagnetic
    * @author : Kaguya
    * @date : 2020/12/7 13:06
    * @inparam : 分别为纬度、经度、高度、年份后两位、月份、日期
    * @outparam : 地磁场的三个分量
    * @last change : 
    * @usage : 
    *****************************************************************************/
    public static double[] getGeomagnetic(double lat, double lon, double h, int yy, int mm, int dd)
    {

        lat = deg_to_rad(lat);
        lon = deg_to_rad(lon);

        const int nmax = 12;
        double[,] P = new double[13, 13];
        double[,] DP = new double[13, 13];
        double[,] gnm = new double[13, 13];
        double[,] hnm = new double[13, 13];
        double[] sm = new double[13];
        double[] cm = new double[13];
        double[] root = new double[13];
        double[,,] roots = new double[13, 13, 2];
        bool been_here = false;

        long dat = yymmdd_to_julian_days(yy, mm, dd);
        /* output field B_r,B_th,B_phi,B_x,B_y,B_z */
        int n, m, nmaxl;
        double yearfrac, sr, r, theta, c, s, psi, fn, fn_0, B_r, B_theta, B_phi, X, Y, Z;
        double sinpsi, cospsi, inv_s;

        //static int been_here = 0;

        double sinlat = Math.Sin(lat);
        double coslat = Math.Cos(lat);

        /* convert to geocentric */
        sr = Math.Sqrt(a * a * coslat * coslat + b * b * sinlat * sinlat);
        /* sr is effective radius */
        theta = Math.Atan2(coslat * (h * sr + a * a), sinlat * (h * sr + b * b));

        /* theta is geocentric co-latitude */

        r = h * h + 2.0 * h * sr +
            (a * a * a * a - (a * a * a * a - b * b * b * b) * sinlat * sinlat) /
            (a * a - (a * a - b * b) * sinlat * sinlat);

        r = Math.Sqrt(r);

        /* r is geocentric radial distance */
        c = Math.Cos(theta);
        s = Math.Sin(theta);
        /* protect against zero divide at geographic poles */
        //inv_s =  1.0 / (s +(s == 0.0)*1.0e-8); 
        if (s == 0)
            inv_s = 1.0 / (s + 1 * 1.0e-8);
        else
            inv_s = 1.0 / (s + 0 * 1.0e-8);

        /*zero out arrays */
        for (n = 0; n <= nmax; n++)
        {
            for (m = 0; m <= n; m++)
            {
                P[n, m] = 0;
                DP[n, m] = 0;
            }
        }

        /* diagonal elements */
        P[0, 0] = 1;
        P[1, 1] = s;
        DP[0, 0] = 0;
        DP[1, 1] = c;
        P[1, 0] = c;
        DP[1, 0] = -s;

        /* these values will not change for subsequent function calls */
        if (!been_here)
        {
            for (n = 2; n <= nmax; n++)
            {
                root[n] = Math.Sqrt((2.0 * n - 1) / (2.0 * n));
            }

            for (m = 0; m <= nmax; m++)
            {
                double mmm = m * m;
                for (n = Math.Max(m + 1, 2); n <= nmax; n++)
                {
                    roots[m, n, 0] = Math.Sqrt((n - 1) * (n - 1) - mmm);
                    roots[m, n, 1] = 1.0 / Math.Sqrt(n * n - mmm);
                }
            }
            been_here = true;
        }

        for (n = 2; n <= nmax; n++)
        {
            /*  double root = sqrt((2.0*n-1) / (2.0*n)); */
            P[n, n] = P[n - 1, n - 1] * s * root[n];
            DP[n, n] = (DP[n - 1, n - 1] * s + P[n - 1, n - 1] * c) * root[n];
        }

        /* lower triangle */
        for (m = 0; m <= nmax; m++)
        {
            /*  double mm = m*m;  */
            for (n = Math.Max(m + 1, 2); n <= nmax; n++)
            {
                /* double root1 = sqrt((n-1)*(n-1) - mm); */
                /* double root2 = 1.0 / sqrt( n*n - mm);  */
                P[n, m] = (P[n - 1, m] * c * (2.0 * n - 1) -
                    P[n - 2, m] * roots[m, n, 0]) * roots[m, n, 1];
                DP[n, m] = ((DP[n - 1, m] * c - P[n - 1, m] * s) *
                    (2.0 * n - 1) - DP[n - 2, m] * roots[m, n, 0]) * roots[m, n, 1];
            }
        }

        /* compute gnm, hnm at dat */
        nmaxl = 12;  /* models except IGRF2005 */
        yearfrac = (dat - yymmdd_to_julian_days(10, 1, 1)) / 365.25;
        //nmaxl = 14;
        for (n = 1; n <= nmaxl; n++)
            for (m = 0; m <= nmaxl; m++)
            {
                gnm[n, m] = gnm_igrf2010[n, m] + yearfrac * gtnm_igrf2010[n, m];
                hnm[n, m] = hnm_igrf2010[n, m] + yearfrac * htnm_igrf2010[n, m];
            }
        for (m = 0; m <= nmaxl; m++)
        {
            sm[m] = Math.Sin(m * lon);
            cm[m] = Math.Cos(m * lon);
        }

        /* compute B fields */
        B_r = 0.0;
        B_theta = 0.0;
        B_phi = 0.0;
        fn_0 = r_0 / r;
        fn = fn_0 * fn_0;

        for (n = 1; n <= nmaxl; n++)
        {
            double c1_n = 0;
            double c2_n = 0;
            double c3_n = 0;
            for (m = 0; m <= n; m++)
            {
                double tmp = (gnm[n, m] * cm[m] + hnm[n, m] * sm[m]);
                c1_n += tmp * P[n, m];
                c2_n += tmp * DP[n, m];
                c3_n += m * (gnm[n, m] * sm[m] - hnm[n, m] * cm[m]) * P[n, m];
            }
            /* fn=pow(r_0/r,n+2.0);   */
            fn *= fn_0;
            B_r += (n + 1) * c1_n * fn;
            B_theta -= c2_n * fn;
            B_phi += c3_n * fn * inv_s;
        }



        /* Find geodetic field components: */
        psi = theta - (PI / 2.0 - lat);
        sinpsi = Math.Sin(psi);
        cospsi = Math.Cos(psi);
        X = -B_theta * cospsi - B_r * sinpsi;
        Y = B_phi;
        Z = B_theta * sinpsi - B_r * cospsi;

        /* output fields */
        /* find variation in radians */
        /* return zero variation at magnetic pole X=Y=0. */
        /* E is positive */
        double[] result = { X, Y, Z };
        return result;
    }
}