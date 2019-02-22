﻿namespace Surveillance.Engine.Rules.Rules
{
    public static class Versioner
    {
        public static string Version(int major, int minor)
        {
            return $"V{major}.{minor}";
        }
    }
}