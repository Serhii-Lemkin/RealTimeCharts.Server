namespace RealTimeCharts.Server.Models
{
    public class RegResponce
    {
        public bool Success { get; set; }
        public RegResponce(bool success)
        {
            Success = success;
        }
    }
}
