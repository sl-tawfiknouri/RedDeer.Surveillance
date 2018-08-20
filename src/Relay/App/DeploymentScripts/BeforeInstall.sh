#!/bin/bash

# Stop and disable the service.
systemctl stop relayservice
systemctl disable relayservice

# Remove systemd unit file.
rm /lib/systemd/system/relayservice.service

# Reload daemons.
systemctl daemon-reload
systemctl reset-failed

# Remove service directory.
rm -rf /reddeer/relayservice/
