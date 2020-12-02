# Space_Radiation  
## 简介
用于计算四种空间辐射，包括单粒子效应、位移损伤、深层充电效应、总剂量效应，基于C#编写。

## 代码框架：  
* [Program](./Space_Radiation/Program.cs)  主程序
* [DeepCharging](./Space_Radiation/DeepCharging.cs) 计算深层充电
* [Displacement](./Space_Radiation/Displacement.cs) 计算位移损伤
* [maxModel](./Space_Radiation/maxModel.cs) 太阳极大年模型，用于计算电子与质子能谱
* [minModel](./Space_Radiation/minModel.cs) 太阳极小年模型，用于计算电子与质子能谱
* [Position](./Space_Radiation/Position.cs) 计算航天器位置，基于项目[one_Sgp4](https://github.com/1manprojects/one_Sgp4)编写
* [SEE](./Space_Radiation/SEE.cs) 计算单粒子效应
* [Spectrum](./Space_Radiation/Spectrum.cs) 计算电子与质子能谱
* [TotalDose](./Space_Radiation/TotalDose.cs) 计算总剂量效应
