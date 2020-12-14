/********************************************************************
	created:	2020/12/03
	created:	3:12:2020   13:07
	filename: 	SpaceRadiation.cs
	file path:	Space_Radiation
	file base:	SpaceRadiation
	file ext:	cs
	author:		Kaguya
	
	purpose:	主类
*********************************************************************/
using System;

namespace Space_Radiation
{
    public interface IRadiation
    {
        double getRadiation();
        void calRadiation(double[] energy, double[] flux, int instrument);
        void calRadiation(double[] energy, double[] flux, double high, 
            double latitude, double longitude, int instrument);
        void calRadiation(double[] energy, double[] flux, double shield);
    }
    class SpaceRadiation
    {
        Position position;
        double[] LLA;
        Spectrum spectrum = new Spectrum();
        SEE singleEffectEvent = new SEE();
        DisplacementDamage displacement = new DisplacementDamage();
        DeepCharging deepCharging = new DeepCharging();
        TotalDose totalDose = new TotalDose();
        double shield = 0;
        double[] geomagnetic;
        int[] instrument = { 1, 1, 1 };
        /*****************************************************************************
        * @function name : SpaceRadiation
        * @author : Kaguya
        * @date : 2020/12/2 17:33
        * @inparam : apogee——远地点，单位km
        * @inparam : perigee——近地点，单位km
        * @inparam : dip——轨道倾角，单位°
        * @outparam : 
        * @last change : 
        * @usage : 根据给定的近地点、远地点、轨道倾角，初始化对象
        *****************************************************************************/
        public SpaceRadiation(double apogee, double perigee, double dip)
        {
            double c = (apogee - perigee) / 2;
            double a = c + 6371 + perigee;
            int[] time = { 2020, 1, 1, 1, 1, 1 };
            position = new Position(dip, 0, c / a, 0, a * 1000, 0, time);
            updateData();
        }
        /*****************************************************************************
        * @function name : addTime
        * @author : Kaguya
        * @date : 2020/12/2 17:30
        * @inparam : second 需要增加的时间，单位：秒
        * @outparam : 
        * @last change : 
        * @usage : 让航天器运行给定的时间后，重新计算轨道位置和辐射效应
        *****************************************************************************/
        public void addTime(int second)
        {
            position.addTime(second);
            updateData();
        }
        public void setTime(int[] time)
        {
            position.setTime(time);
            updateData();
        }
        private void updateData()
        {
            spectrum.reflashEnergy();

            LLA = position.getPosition();

            spectrum.calFlux(LLA[2], LLA[0], LLA[1]);

            //由于总剂量效应需要输入未经屏蔽的电子能谱，所以先于shield计算
            totalDose.calRadiation(spectrum.getProtonEnergy(), spectrum.getProtonFlux(), shield);

            SEE.shield(shield, getProtonEnergy(), getProtonFlux(), SEE.ProtonLET);
            SEE.shield(shield, getElectronEnergy(), getElectronFlux(), SEE.ElectronLET);

            calculateRadiation();

        }
        private void calculateRadiation()
        {
            singleEffectEvent.calRadiation(spectrum.getProtonEnergy(), spectrum.getProtonFlux(), LLA[2], LLA[0], LLA[1], instrument[0]);
            //Console.WriteLine("高度："+LLA[2]+"，捕获带质子产生SEE：" + singleEventFromTrapped + "，宇宙线质子产生SEE：" + singleEventFromCosmic);

            displacement.calRadiation(spectrum.getProtonEnergy(), spectrum.getProtonFlux(), instrument[1]);

            deepCharging.calRadiation(spectrum.getElectronEnergy(), spectrum.getElectronFlux(), instrument[2]);

            //totalDose.calRadiation(spectrum.getProtonEnergy(), originProtonFlux, shield);

            int[] time = position.getTime();
            geomagnetic = Geomagnetic.getGeomagnetic(LLA[0], LLA[1], LLA[2], time[0] % 2000, time[1], time[2]);
        }
        /*****************************************************************************
        * @function name : setShield
        * @author : Kaguya
        * @date : 2020/12/3 12:22
        * @inparam : shield——等效铝屏蔽厚度
        * @outparam : 
        * @last change : 
        * @usage : 根据给定的等效屏蔽厚度，更新粒子能谱和辐射效应
        *****************************************************************************/
        public void setShield(double shield) { 
            this.shield = shield; 
            updateData();
        }
        public void setInstrument(int[] instrument)
        {
            this.instrument = instrument;
            updateData();
        }
        /*****************************************************************************
        * @function name : getter
        * @author : Kaguya
        * @date : 2020/12/3 13:08
        * @inparam : 
        * @outparam : 
        * @last change : 
        * @usage : 分别获取质子、电子能量，质子、电子通量，位置，单粒子效应，位移损伤，
        *          深层充电电位，总剂量效应，地磁场
        *****************************************************************************/
        public double[] getProtonEnergy() { return spectrum.getProtonEnergy(); }
        public double[] getElectronEnergy() { return spectrum.getElectronEnergy(); }
        public double[] getProtonFlux() { return spectrum.getProtonFlux(); }
        public double[] getElectronFlux() { return spectrum.getElectronFlux(); }
        public double[] getPosition() { return LLA; }
        public double getSEE() { return singleEffectEvent.getRadiation(); }
        public double getDisplacementDamage() { return displacement.getRadiation(); }
        public double getDeepCharging() { return deepCharging.getRadiation(); }
        public double getTotalDose() { return totalDose.getRadiation(); }
        public double[] getGeomagnetic() { return geomagnetic; }
        public void printInformation()
        {
            Console.WriteLine(String.Format("纬度：{0}°、经度：{1}°、高度：{2}km", LLA[0], LLA[1], LLA[2]));
            Console.WriteLine("单粒子效应： " + getSEE());
            Console.WriteLine("深层充电效应： " + getDeepCharging());
            Console.WriteLine("位移损伤效应： " + getDisplacementDamage());
            Console.WriteLine("总剂量效应： " + getTotalDose());
            Console.WriteLine(String.Format("地磁场的三个分量为{0} nT、{1} nT、{2} nT",
                geomagnetic[0], geomagnetic[1], geomagnetic[2]));
        }

        static void Main(string[] args)
        {
            for(double i =1000;i<36000;i+=1000)
            {
                SpaceRadiation p = new SpaceRadiation(i, i, 90);
                p.setInstrument(new int[] { 2, 2, 2 });
                //Console.WriteLine(String.Format("{0} {1} {2} {3}", p.getSEE(), p.getDeepCharging(), p.getDisplacementDamage(), p.getTotalDose()));
                Console.WriteLine(String.Format("{0} {1} {2}",  p.getSEE(), p.getDeepCharging(),p.getDisplacementDamage()));
            }
        }
    }
}
