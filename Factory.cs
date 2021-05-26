using System.ComponentModel;
using System;

namespace LW2
{
    public class Factory
    {
        private IListSource _listSource;
        private ITelemetryReporter _telemetryReporter;

        public Factory() { }

        public Factory WithListSource(IListSource src)
        {
            _listSource = src;
            return this;
        }

        public Factory WithTelemetryReporter(ITelemetryReporter rep)
        {
            _telemetryReporter = rep;
            return this;
        }

        public CountingExceptions Build()
        {
            return new CountingExceptions(_listSource, _telemetryReporter);
        }
    }
}