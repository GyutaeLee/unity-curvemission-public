using System.Collections.Generic;
namespace Util.Csv.Data.Class
{
    public class Car : IData
    {
        public int InfoID { get; set; }
        public string Name { get; set; }
        public float StartSpeed { get; set; }
        public float MinSpeed { get; set; }
        public float MaxSpeed { get; set; }
        public float NormalAccelerationValue { get; set; }
        public float NormalDecelerationValue { get; set; }
        public float CurveDecelerationValue { get; set; }
        public List<float> BoosterValue { get; set; }
        public List<float> BoosterTime { get; set; }
        public List<float> ObstacleValue { get; set; }
    }
}
