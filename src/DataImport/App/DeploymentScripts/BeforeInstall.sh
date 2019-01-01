#!/bin/bash

# Stop and disable the service.
systemctl stop dataimportservice
systemctl disable dataimportservice

# Remove systemd unit file.
rm /lib/systemd/system/dataimportservice.service

# Reload daemons.
systemctl daemon-reload
systemctl reset-failed

# Remove service directory.
rm -rf /reddeer/dataimportservice/
