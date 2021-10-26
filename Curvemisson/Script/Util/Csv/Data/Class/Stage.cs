using System.Collections.Generic;
namespace Util.Csv.Data.Class
{
    public class Stage : IData
    {
        public int InfoID { get; set; }
        public string Name { get; set; }
        public int InitialVehicleState { get; set; }
        public List<float> InitialVehiclePosition { get; set; }
        public List<float> InitialCameraPosition { get; set; }
        public int FinishLapCount { get; set; }
    }
}
