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
    class SpaceRadiation
    {
        Position position;
        double[] LLA;
        double[] protonEnergy = new double[10];
        double[] protonFlux = new double[10];
        double[] originProtonFlux;
        double[] electronEnergy = new double[500];
        double[] electronFlux = new double[500];
        double[] cosmicRaysEnergy = new double[100];
        double[] cosmicRaysFlux = new double[100];
        double singleEvent;
        double displacement;
        double deepCharging;
        double totalDose;
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
            for(int i = 0; i < 10; i++)
            {
                protonEnergy[i] = 3 + i;
            }
            for (int i = 0; i < 500; i++)
            {
                electronEnergy[i] = 0.01 + 0.01 * i;
                if (i < 100)
                {
                    cosmicRaysEnergy[i] = 0.1 * (i + 1) * 1000;
                }
            }
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
        private void updateData()
        {
            for (int i = 0; i < 10; i++)
            {
                protonEnergy[i] = 3 + i;
            }
            for (int i = 0; i < 500; i++)
            {
                electronEnergy[i] = 0.01 + 0.01 * i;
                if (i < 100)
                {
                    cosmicRaysEnergy[i] = 0.1 * (i + 1) * 1000;
                }
            }

            LLA = position.getPosition();

            double high = LLA[2];
            maxModel model = new maxModel();
            double[,] spectrum = model.getFlux(high, Spectrum.getTheatM(LLA[1], LLA[0]) * 180 / Math.PI);
            for(int i = 0; i < 10; i++)
            {
                protonFlux[i] = spectrum[3, i];
            }
            for (int i = 0; i < 500; i++)
            {
                electronFlux[i] = spectrum[1, i];
            }
            model.Dispose();

            originProtonFlux = protonFlux;

            SEE.shield(shield, protonEnergy, protonFlux, SEE.ProtonLET);
            SEE.shield(shield, electronEnergy, electronFlux, SEE.ElectronLET);

            for(int i = 0; i < 100; i++)
            {
                cosmicRaysFlux[i] = Spectrum.getAlpha(LLA[2], LLA[1], LLA[0], 0.5,
                        cosmicRaysEnergy[i] / 1000, 1, true) * 10000 * 6.28;
            }

            calculateRadiation();

        }
        private void calculateRadiation()
        {
            double singleEventFromTrapped = SEE.getSEE(protonEnergy, protonFlux, instrument[0]);
            double singleEventFromCosmic = SEE.getSEE(cosmicRaysEnergy, cosmicRaysFlux, instrument[0]);
            singleEvent = singleEventFromCosmic + singleEventFromTrapped;
            Console.WriteLine("高度："+LLA[2]+"，捕获带质子产生SEE：" + singleEventFromTrapped + "，宇宙线质子产生SEE：" + singleEventFromCosmic);

            displacement = displacementDamage.getDisplacementDamage(protonEnergy, protonFlux, instrument[1]);

            double temp = 0;
            for (int i = 0; i < 500; i++)
            {
                if (electronFlux[i] == 0) continue;
                temp = electronFlux[i];
                if (temp != 0) break;
            }
            deepCharging = DeepCharging.deepCharging(temp, instrument[2]);

            totalDose = TotalDose.getTotal(originProtonFlux, shield);

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
        public double[] getProtonEnergy() { return protonEnergy; }
        public double[] getElectronEnergy() { return electronEnergy; }
        public double[] getProtonFlux() { return protonFlux; }
        public double[] getElectronFlux() { return electronFlux; }
        public double[] getPosition() { return LLA; }
        public double getSEE() { return singleEvent; }
        public double getDisplacementDamage() { return displacement; }
        public double getDeepCharging() { return deepCharging; }
        public double getTotalDose() { return totalDose; }
        public double[] getGeomagnetic() { return geomagnetic; }
        public void printInformation()
        {
            Console.WriteLine(String.Format("纬度：{0}°、经度：{1}°、高度：{2}km", LLA[0], LLA[1], LLA[2]));
            Console.WriteLine("单粒子效应： " + singleEvent);
            Console.WriteLine("深层充电效应： " + deepCharging);
            Console.WriteLine("位移损伤效应： " + displacement);
            Console.WriteLine("总剂量效应： " + totalDose);
            Console.WriteLine(String.Format("地磁场的三个分量为{0} nT、{1} nT、{2} nT",
                geomagnetic[0], geomagnetic[1], geomagnetic[2]));
        }

        static void Main(string[] args)
        {
            for(double i = 1000; i < 36000; i += 1000)
            {
                SpaceRadiation p = new SpaceRadiation(i, i, 0);
            }
        }
    }
}
