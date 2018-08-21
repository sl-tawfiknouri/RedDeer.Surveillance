#!/bin/bash

# Stop and disable the service.
systemctl stop surveillanceservice
systemctl disable surveillanceservice

# Remove systemd unit file.
rm /lib/systemd/system/surveillanceservice.service

# Reload daemons.
systemctl daemon-reload
systemctl reset-failed

# Remove service directory.
rm -rf /reddeer/surveillanceservice/
