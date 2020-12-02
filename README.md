# Space_Radiation  
## 简介
用于计算四种空间辐射，包括单粒子效应、位移损伤、深层充电效应、总剂量效应，基于C#编写。

## 代码框架：  
* [Program](./集合/Program.cs)  主程序
* [DeepCharging](./集合/DeepCharging.cs) 计算深层充电
* [Displacement](./集合/Displacement.cs) 计算位移损伤
* [maxModel](./集合/maxModel.cs) 太阳极大年模型，用于计算电子与质子能谱
* [minModel](./集合/minModel.cs) 太阳极小年模型，用于计算电子与质子能谱
* [Position](./集合/Position.cs) 计算航天器位置，基于项目[one_Sgp4](https://github.com/1manprojects/one_Sgp4)编写
* [SEE](./集合/SEE.cs) 计算单粒子效应
* [Spectrum](./集合/Spectrum.cs) 计算电子与质子能谱
* [TotalDose](./集合/TotalDose.cs) 计算总剂量效应
