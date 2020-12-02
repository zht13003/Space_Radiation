using System;

namespace Space_Radiation
{
    class Program
    {
        Position position;
        double[] LLA;
        double[] protonEnergy = new double[10];
        double[] protonFlux = new double[10];
        double[] electronEnergy = new double[500];
        double[] electronFlux = new double[500];
        double singleEvent;
        double displacement;
        double deepCharging;
        double totalDose;
        double shield = 0;
        /*****************************************************************************
        * @function name : Program
        * @author : Kaguya
        * @date : 2020/12/2 17:33
        * @inparam : yuan——远地点，单位km
        * @inparam : jin——近地点，单位km
        * @inparam : qing——轨道倾角，单位°
        * @outparam : 
        * @last change : 
        * @usage : 
        *****************************************************************************/
        public Program(double yuan, double jin, double qing)
        {
            double c = (yuan - jin) / 2;
            double a = c + 6371 + jin;
            int[] time = { 2020, 1, 1, 1, 1, 1 };
            position = new Position(qing, 0, c / a, 0, a * 1000, 0, time);
            for(int i = 0; i < 10; i++)
            {
                protonEnergy[i] = 3 + i;
            }
            for (int i = 0; i < 500; i++)
            {
                electronEnergy[i] = 0.01 + 0.01 * i;
            }
            updataData();
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
            updataData();
        }
        private void updataData()
        {
            for (int i = 0; i < 10; i++)
            {
                protonEnergy[i] = 3 + i;
            }
            for (int i = 0; i < 500; i++)
            {
                electronEnergy[i] = 0.01 + 0.01 * i;
            }
            position.getDate();
            position.getECIDateAndVelocity();
            LLA = Position.XYZ2LLA(Position.getECEF(position.ECI, position.data));
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

            SEE.shield(shield, protonEnergy, protonFlux, SEE.ProtonLET);
            SEE.shield(shield, electronEnergy, electronFlux, SEE.ElectronLET);

            calculateRadiation();
        }
        private void calculateRadiation()
        {
            singleEvent = SEE.getSEE(protonEnergy, protonFlux, electronEnergy, electronFlux, 1);
            displacement = displacementDamage.Integral(protonEnergy, protonFlux, 1);

            double temp = 0;
            for (int i = 0; i < 500; i++)
            {
                if (electronFlux[i] == 0) continue;
                temp = electronFlux[i];
                if (temp != 0) break;
            }
            deepCharging = DeepCharging.deepCharging(temp, 1);

            totalDose = TotalDose.getTotal(protonFlux, 0);
        }
        public void setShield(double shield) { 
            this.shield = shield; 
            updataData();
        }
        public double[] getProtonEnergy() { return protonEnergy; }
        public double[] getElectronEnergy() { return electronEnergy; }
        public double[] getProtonFlux() { return protonFlux; }
        public double[] getElectronFlux() { return electronFlux; }
        public double[] getPosition() { return LLA; }
        public double getSEE() { return singleEvent; }
        public double getDisplacementDamage() { return displacement; }
        public double getDeepCharging() { return deepCharging; }
        public double getTotalDose() { return totalDose; }
        static void Main(string[] args)
        {
            Program p = new Program(25000, 25000, 0);
            p.setShield(0);
            for (int i = 0; i < 20; i++)
            {
                double[] position = p.getPosition();
                Console.WriteLine(String.Format("当前位置为{0}°、{1}°、{2}km", position[0], position[1], position[2]));
                Console.WriteLine("单粒子效应为 " + p.getSEE());
                Console.WriteLine("深层充电效应为 " + p.getDeepCharging());
                Console.WriteLine("位移损伤效应为 " + p.getDisplacementDamage());
                Console.WriteLine("总剂量效应为 " + p.getTotalDose());
                p.addTime(7200);
            }
        }
    }
}
