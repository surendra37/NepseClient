namespace NepseApp.DataModels
{
    public class LiveDataModel
    {
        public string Symbol { get; set; }
        public double Ltp { get; set; }
        public double Ltv { get; set; }
        public double PointChange { get; set; }
        public double PercentChange { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
    }
}
