# Surveillance
Surveillance analysis components

To build set up surveillance then relay then test harness.

Data stream runs from (upstream -> downstream) test harness; relay; surveillance service.

Both web socket client services (test harness and relay) have automated recovery and failover web socket connections which write to local memory when the downstream service they write to is not available.

This will resync once the downstream service comes online provided that the upstream service has not been shut down whilst buffering to local memory.

This means we are now able to start/stop the services in any order and the wider surveillance system will sort itself out. :)
