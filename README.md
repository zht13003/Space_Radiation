# Space_Radiation  
## 简介
用于计算四种空间辐射，包括单粒子效应、位移损伤、深层充电效应、总剂量效应，基于C#编写，Visual Studio 2019测试通过。

## 代码框架
* [SpaceRadiation](./Space_Radiation/SpaceRadiation.cs)  主程序
* [DeepCharging](./Space_Radiation/DeepCharging.cs) 计算深层充电
* [Displacement](./Space_Radiation/Displacement.cs) 计算位移损伤
* [Geomagnetic](./Space_Radiation/Geomagnetic.cs) 计算地磁场
* [maxModel](./Space_Radiation/maxModel.cs) 太阳极大年模型，用于计算电子与质子能谱
* [minModel](./Space_Radiation/minModel.cs) 太阳极小年模型，用于计算电子与质子能谱
* [Position](./Space_Radiation/Position.cs) 计算航天器位置，基于项目[one_Sgp4](https://github.com/1manprojects/one_Sgp4)编写
* [SEE](./Space_Radiation/SEE.cs) 计算单粒子效应
* [Spectrum](./Space_Radiation/Spectrum.cs) 计算电子与质子能谱
* [TotalDose](./Space_Radiation/TotalDose.cs) 计算总剂量效应

## 安装
克隆该项目后，在NuGet控制台输入：

    Install-Package one_sgp4   
    Install-Package MathNet.Numerics 

然后将Data目录下的数据文件复制到可执行文件的同一目录下

## 更新日志
2020-12-08 V1.2 更新了单粒子效应的计算，现在的输入是捕获带质子与宇宙线质子  
2020-12-07 V1.1 添加了计算地磁场模块  
2020-12-02 V1.0 上传原始版本