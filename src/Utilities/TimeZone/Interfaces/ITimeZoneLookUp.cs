namespace Utilities.TimeZone.Interfaces
{
    public interface ITimeZoneLookUp
    {
        string[] GetLinuxTimeZoneFromMicrosoft(string timezone);
        string[] GetMicrosoftTimeZoneFromLinux(string timezone);
    }
}