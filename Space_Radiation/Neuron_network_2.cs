/********************************************************************
	created:	2020/12/03
	created:	3:12:2020   12:54
	filename: 	Neuron_network_2.cs
	file path:	Space_Radiation
	file base:	Neuron_network
	file ext:	cs
	author:		Kaguya
	
	purpose:	神经网络框架，适用于多输入
*********************************************************************/
using System;

internal struct Neuron_2
{
    public double[] w;
    public double b;
};

public class Neuron_network_2
{
    private int len;
    private Neuron_2[] n;
    private double[] out_w;
    private double out_b;
    private double YMAX = 1;
    private double YMIN = -1;
    private double[] X_XMAX;
    private double[] X_XMIN;
    private double Y_XMAX;
    private double Y_XMIN;

    private static double inspiration(double x)
    {
        return (Math.Exp(x) - Math.Exp(-x)) / (Math.Exp(x) + Math.Exp(-x));
    }

    public Neuron_network_2(double[,] w1, double[] w2, double[] b1, double b2, int len, double[] XMAX, double[] XMIN, double YMAX, double YMIN)
    {
        this.len = len;
        n = new Neuron_2[len];
        for (int i = 0; i < len; i++)
        {
            n[i].w = new double[w1.GetLength(1)];
            for (int j = 0; j < w1.GetLength(1); j++)
            {
                n[i].w[j] = w1[i, j];
            }
            n[i].b = b1[i];
        }
        out_w = w2;
        out_b = b2;
        X_XMAX = XMAX;
        X_XMIN = XMIN;
        Y_XMAX = YMAX;
        Y_XMIN = YMIN;
    }

    public double input(double[] x)
    {
        for (int i = 0; i < x.Length; i++)
        {
            x[i] = Math.Log10(x[i]);
            x[i] = (YMAX - YMIN) * (x[i] - X_XMIN[i]) / (X_XMAX[i] - X_XMIN[i]) + YMIN;
        }

        double result = 0;
        double[] preResult = new double[len];
        for (int i = 0; i < len; i++)
        {
            preResult[i] = 0;
            for (int j = 0; j < n[i].w.Length; j++)
            {
                preResult[i] += n[i].w[j] * x[j];
            }
            preResult[i] = inspiration(preResult[i] + n[i].b);
            result += preResult[i] * out_w[i];
        }

        result += out_b;
        result = (result - YMIN) * (Y_XMAX - Y_XMIN) / (YMAX - YMIN) + Y_XMIN;

        return Math.Pow(10, result);
    }
};
