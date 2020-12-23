using System;
using System.Collections.Generic;
using System.Text;

namespace SIKOSI.Exchange.Model
{
    public class IotMeasurementModel
    {
        public int DistanceMM { get; set; }
        public double TempC { get; set; }
        public double HumPercent { get; set; }
    }
}
