using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;

namespace Com.Pinz.Client.Wpf.App.Insights
{
    public class ApplicationInsightHelper
    {
        private readonly TelemetryClient _telemetryClient;

        private string _sessionKey;
        private string _userName;
        private string _osName;
        private string _version;
        private string _application;
        private string _manufacturer;
        private string _model;

        public ApplicationInsightHelper()
        {
            _telemetryClient = new TelemetryClient();
            _telemetryClient.InstrumentationKey = "18b0b26d-5839-4f6a-8fb9-0da40f68a001";
            GatherDetails();
            SetupTelemetry();
        }

        public void TrackPageView(string pageName)
        {
            _telemetryClient.TrackPageView(pageName);
        }

        public void TrackNonFatalExceptions(Exception ex)
        {
            var metrics = new Dictionary<string, double> { { "Non-fatal Exception", 1 } };
            _telemetryClient.TrackException(ex, null, metrics);
        }

        public void FlushData()
        {
            _telemetryClient.Flush();
        }

        public void TrackFatalException(Exception ex)
        {
            var exceptionTelemetry = new Microsoft.ApplicationInsights.DataContracts.ExceptionTelemetry(new Exception());
            exceptionTelemetry.HandledAt = Microsoft.ApplicationInsights.DataContracts.ExceptionHandledAt.Unhandled;
            _telemetryClient.TrackException(exceptionTelemetry);
        }

        private void GatherDetails()
        {
            _sessionKey = Guid.NewGuid().ToString();
            _userName = Environment.UserName;
            _osName = GetWindowsFriendlyName();
            _version = $"v.{ Assembly.GetEntryAssembly().GetName().Version}";
            _application = $"{ Assembly.GetEntryAssembly().GetName().Name}        {_version}";
            _manufacturer = ""; // (from x in new ManagementObjectSearcher("SELECT Manufacturer FROM Win32ComputerSystem").Get().OfType() select x.GetPropertyValue("Manufacturer")).FirstOrDefault()?.ToString() ?? "Unknown";
            _model = ""; // (from x in new ManagementObjectSearcher("SELECT Model FROM Win32ComputerSystem").Get().OfType() select x.GetPropertyValue("Model")).FirstOrDefault()?.ToString() ?? "Unknown";
        }

        private string GetWindowsFriendlyName()
        {
            var name = ""; // (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().OfType() select x.GetPropertyValue("Caption")).FirstOrDefault();
            return name?.ToString() ?? Environment.OSVersion.ToString();
        }

        private void SetupTelemetry()

        {
            _telemetryClient.Context.Properties.Add("Application Version", _version);
            _telemetryClient.Context.User.Id = _userName;
            _telemetryClient.Context.User.UserAgent = _application;
            _telemetryClient.Context.Component.Version = _version;
            _telemetryClient.Context.Session.Id = _sessionKey;
            _telemetryClient.Context.Device.OemName = _manufacturer;
            _telemetryClient.Context.Device.Model = _model;
            _telemetryClient.Context.Device.OperatingSystem = _osName;
        }
    }
}
