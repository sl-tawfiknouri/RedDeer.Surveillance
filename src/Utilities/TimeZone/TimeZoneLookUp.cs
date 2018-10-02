using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneDb.UseCases;
using Utilities.TimeZone.Interfaces;

namespace Utilities.TimeZone
{
    /// <summary>
    /// Time zone look up singleton
    /// 20? second load for the time zone db use case hence the lazy behaviour
    /// </summary>
    public class TimeZoneLookUp : ITimeZoneLookUp
    {
        private readonly object _lock = new object();
        private volatile Task _startupTask;
        private volatile bool _hasInitiated;

        private ITimeZoneDbUseCases _dbUseCases;
        private IDictionary<string, string[]> _microsoftKeyLinuxValue;
        private IDictionary<string, string[]> _linuxKeyMicrosoftValue;

        public TimeZoneLookUp()
        {
            _startupTask = Task.Run(() => { _dbUseCases = new TimeZoneDbUseCases(); });
        }

        private void Initialise()
        {
            lock (_lock)
            {
                if (_hasInitiated)
                {
                    return;
                }

                if (!_startupTask.IsCompleted)
                {
                    _startupTask.Wait();
                }

                var allZones = _dbUseCases.GetAllTimeZones().ToList();

                _microsoftKeyLinuxValue =
                    allZones
                        .Where(az => !string.IsNullOrWhiteSpace(az.MicrosoftId))
                        .GroupBy(az => az.MicrosoftId)
                        .Select(az => new KeyValuePair<string, string[]>(az.Key.ToLower(), az.Select(azz => azz.IanaId).ToArray()))
                        .ToDictionary(dic => dic.Key, dic => dic.Value);

                _linuxKeyMicrosoftValue =
                    allZones
                        .Where(az => !string.IsNullOrWhiteSpace(az.IanaId))
                        .GroupBy(az => az.IanaId)
                        .Select(az => new KeyValuePair<string, string[]>(az.Key.ToLower(), az.Select(azz => azz.MicrosoftId).ToArray()))
                        .ToDictionary(dic => dic.Key, dic => dic.Value);

                _hasInitiated = true;
            }
        }

        public string[] GetLinuxTimeZoneFromMicrosoft(string timezone)
        {
            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(timezone))
                {
                    return new string[0];
                }

                timezone = timezone.ToLower();

                if (!_hasInitiated)
                {
                    Initialise();
                }

                if (!_microsoftKeyLinuxValue.ContainsKey(timezone))
                {
                    return new string[0];
                }

                _microsoftKeyLinuxValue.TryGetValue(timezone, out var linuxZone);

                return linuxZone ?? new string[0];
            }
        }

        public string[] GetMicrosoftTimeZoneFromLinux(string timezone)
        {
            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(timezone))
                {
                    return new string[0];
                }

                if (!_hasInitiated)
                {
                    Initialise();
                }

                timezone = timezone.ToLower();

                if (!_linuxKeyMicrosoftValue.ContainsKey(timezone))
                {
                    return new string[0];
                }

                _linuxKeyMicrosoftValue.TryGetValue(timezone, out var microsoftZone);

                return microsoftZone ?? new string[0];
            }
        }
    }
}
