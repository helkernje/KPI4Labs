using System;

namespace LW2
{
    public interface ITelemetryReporter
    {
        bool Report(String kind);
    }
}