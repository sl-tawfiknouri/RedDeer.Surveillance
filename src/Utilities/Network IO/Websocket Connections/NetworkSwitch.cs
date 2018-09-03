using Utilities.Network_IO.Websocket_Connections.Interfaces;
using System;
using System.Timers;
using System.Collections.Generic;

namespace Utilities.Network_IO.Websocket_Connections
{
    public class NetworkSwitch : INetworkSwitch
    {
        private readonly int _failoverScanFrequencyMilliseconds = 5000;
        private volatile bool _hasFailedOverData;
        private readonly Timer _timer;
        
        private readonly INetworkTrunk _trunk;
        private readonly INetworkFailover _failover;
        private readonly object _lock = new object();

        private string _domain;
        private string _port;

        public bool Active { get; private set; }

        public NetworkSwitch(INetworkTrunk trunk, INetworkFailover failover)
        {
            _trunk = trunk ?? throw new ArgumentNullException(nameof(trunk));
            _failover = failover ?? throw new ArgumentNullException(nameof(failover));
            _timer = new Timer();
        }

        public NetworkSwitch(
            INetworkTrunk trunk,
            INetworkFailover failover,
            int failoverScanFrequencyInMilliseconds)
        {
            _trunk = trunk ?? throw new ArgumentNullException(nameof(trunk));
            _failover = failover ?? throw new ArgumentNullException(nameof(failover));
            _failoverScanFrequencyMilliseconds = failoverScanFrequencyInMilliseconds;

            _timer = new Timer();
        }

        private void InitiateFailoverMonitorProcess()
        {
            _timer.AutoReset = true;
            _timer.Elapsed += MonitorFailover;
            _timer.Interval = 
                TimeSpan
                .FromMilliseconds(_failoverScanFrequencyMilliseconds)
                .TotalMilliseconds;

            _timer.Enabled = true;
        }

        public void MonitorFailover(object sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                var itemsToRemove = new List<Tuple<Type, object>>();

                var failedToResend = false;

                foreach (var item in _failover.Retrieve())
                {
                    if (item.Value == null)
                    {
                        continue;
                    }

                    foreach (var dataPoint in item.Value)
                    {
                        if (dataPoint == null)
                        {
                            continue;
                        }

                        var success = AddWithoutFailover(dataPoint);

                        if (success)
                        {
                            itemsToRemove.Add(new Tuple<Type, object>(item.Key, dataPoint));
                        }
                        else
                        {
                            failedToResend = true;
                        }
                    }
                }

                foreach (var item in itemsToRemove)
                {
                    _failover.RemoveItem(item.Item1, item.Item2);
                }

                if (failedToResend)
                {
                    var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromMilliseconds(3000));

                    _trunk.Initiate(_domain, _port, cts.Token);
                }

                _hasFailedOverData = false || failedToResend;
            }
        }

        /// <summary>
        /// Success response if added to trunk and successfully sent only
        /// </summary>
        public bool Send<T>(T value)
        {
            lock (_lock)
            {
                if (value == null)
                {
                    return false;
                }

                var sendToFailover = !_trunk.Active;

                if (!sendToFailover)
                {
                    sendToFailover = !_trunk.Send(value);
                }

                if (sendToFailover)
                {
                    _failover.Store(value);

                    if (!_hasFailedOverData)
                    {
                        _hasFailedOverData = true;
                        InitiateFailoverMonitorProcess();
                    }
                }

                return sendToFailover;
            }
        }

        private bool AddWithoutFailover<T>(T value)
        {
            if (value == null
                || !_trunk.Active)
            {
                return false;
            }

            return _trunk.Send(value);
        }

        public bool Initiate(
            string domain,
            string port,
            System.Threading.CancellationToken token)
        {
            _domain = domain;
            _port = port;
            Active = true;

            var initiated = _trunk.Initiate(domain, port, token);

            return initiated;
        }

        public void Terminate()
        {
            _trunk.Terminate();
            _timer.Stop();
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }
}