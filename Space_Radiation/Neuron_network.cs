/********************************************************************
	created:	2020/12/03
	created:	3:12:2020   12:54
	filename: 	Neuron_network.cs
	file path:	Space_Radiation
	file base:	Neuron_network
	file ext:	cs
	author:		Kaguya
	
	purpose:	神经网络框架，适用于单输入
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

struct Neuron_1
{
    public double w;
    public double b;
};
public class Neuron_network
{
    int len;
    Neuron_1[] n;
    double[] out_w;
    double out_b;
    double YMAX = 1;
    double YMIN = -1;
    double X_XMAX;
    double X_XMIN;
    double Y_XMAX;
    double Y_XMIN;
    bool log;
    bool power;
    static double inspiration(double x)
    {
        return (Math.Exp(x) - Math.Exp(-x)) / (Math.Exp(x) + Math.Exp(-x));
    }
    public Neuron_network(double[] w1, double[] w2, double[] b1, double b2, int len, double XMAX, double XMIN, double YMAX, double YMIN, bool log, bool power)
    {
        this.len = len;
        n = new Neuron_1[len];
        for (int i = 0; i < len; i++)
        {
            n[i].w = w1[i];
            n[i].b = b1[i];
        }
        out_w = w2;
        out_b = b2;
        X_XMAX = XMAX;
        X_XMIN = XMIN;
        Y_XMAX = YMAX;
        Y_XMIN = YMIN;
        this.log = log;
        this.power = power;
    }
    public double input(double x)
    {
        if(log) x = Math.Log10(x);
        x = (YMAX - YMIN) * (x - X_XMIN) / (X_XMAX - X_XMIN) + YMIN;
        double[] sum = new double[len];
        double result = 0;
        for (int i = 0; i < len; i++)
        {
            sum[i] = inspiration(n[i].w * x + n[i].b);
            result += sum[i] * out_w[i];
        }
        result += out_b;
        result = (result - YMIN) * (Y_XMAX - Y_XMIN) / (YMAX - YMIN) + Y_XMIN;

        if(power) return Math.Pow(10, result);
        return result;
    }
};

