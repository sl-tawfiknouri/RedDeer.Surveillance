#!/bin/bash

# Stop and disable the service.
systemctl stop thirdpartysurveillancedatasynchroniserservice
systemctl disable thirdpartysurveillancedatasynchroniserservice

# Remove systemd unit file.
rm /lib/systemd/system/thirdpartysurveillancedatasynchroniserservice.service

# Reload daemons.
systemctl daemon-reload
systemctl reset-failed

# Remove service directory.
rm -rf /reddeer/thirdpartysurveillancedatasynchroniserservice/
