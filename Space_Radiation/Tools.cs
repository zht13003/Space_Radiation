using System;

namespace Space_Radiation
{
    class Tools
    {
        public static double getTheatM(double longtitude, double latitude)
        //地理纬度转化为地磁纬度，输入单位为角度，输出单位为弧度
        {
            longtitude = longtitude * Math.PI / 180;
            latitude = latitude * Math.PI / 180;
            return Math.Asin(Math.Sin(latitude) * Math.Cos(11.7 * Math.PI / 180)
                + Math.Cos(latitude) * Math.Sin(11.7 * Math.PI / 180) * Math.Cos(longtitude - 291 * Math.PI / 180));
        }
    }
}
