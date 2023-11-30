using Ariston.Enums;

namespace Ariston.Entities;

public record PlantData(
    string Gw,
    bool On, 
    Mode Mode, 
    double Temp, 
    double ProcReqTemp, 
    double ReqTemp, 
    double BoostReqTemp, 
    bool AntiLeg, 
    bool HeatReq, 
    int AvShw
);