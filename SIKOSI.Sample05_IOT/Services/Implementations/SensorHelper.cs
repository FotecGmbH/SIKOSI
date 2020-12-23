using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Text;
using Iot.Device.Vl53L0X;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SIKOSI.Sample05_IOT.Services.Interfaces;

namespace SIKOSI.Sample05_IOT.Services.Implementations
{
    public class SensorHelper : ISensorHelper
    {
        private readonly ILogger<SensorHelper> _logger;
        private readonly Vl53L0X _distanceSensor;
        private readonly bool _initialized;

        public SensorHelper(ILogger<SensorHelper> logger, IConfiguration config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            int address = 0;
            try
            {
                var addressString = config.GetSection("Sensor")["i2cAddress"];
                addressString = addressString.Contains('x') ? addressString.Split('x')[1] : addressString;
                address = int.Parse(addressString, System.Globalization.NumberStyles.HexNumber);
                _distanceSensor = new Vl53L0X(I2cDevice.Create(new I2cConnectionSettings(1, address))) {MeasurementMode = MeasurementMode.Continuous};
                _initialized = true;
                _distanceSensor.HighResolution = true;
                _distanceSensor.Precision = Precision.ShortRange;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                _logger.LogError($"Could not connect to Sensor on ADDRESS: 0x{address:X}");
            }
        }

        public int GetDistance()
        {
            if (!_initialized) return -1;
            var distance = _distanceSensor.Distance;
            _logger.LogInformation($"Measured {distance}mm");
            return distance;
        }
    }
}
