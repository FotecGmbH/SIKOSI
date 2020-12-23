using System.Threading;

namespace SIKOSI.Sample05_IOT.Services.Interfaces
{
    public interface IHttpHandler
    {
        bool DoorOpen { get; set; }
        bool DoorStateSet { get; set; }
        double LatestDistance { get; }
        bool LatestDistanceSet { get; }
        double LatestHum { get; }
        bool LatestHumSet { get; }
        double LatestTemp { get; }
        bool LatestTempSet { get; }

        void Dispose();
        void Start(CancellationToken token);
    }
}