#!/bin/bash

# Stop and disable the service.
systemctl stop surveillanceapiservice
systemctl disable surveillanceapiservice

# Remove systemd unit file.
rm /lib/systemd/system/surveillanceapiservice.service

# Reload daemons.
systemctl daemon-reload
systemctl reset-failed

# Remove service directory.
rm -rf /reddeer/surveillanceapiservice/
