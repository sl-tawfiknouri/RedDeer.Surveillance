#!/bin/bash

# Stop and disable the service.
systemctl stop ThirdPartySurveillanceDataSynchroniser
systemctl disable ThirdPartySurveillanceDataSynchroniser


# Remove systemd unit file.
rm /lib/systemd/system/ThirdPartySurveillanceDataSynchroniser.service


# Reload daemons.
systemctl daemon-reload
systemctl reset-failed

# Remove service directory.
rm -rf /reddeer/ThirdPartySurveillanceDataSynchroniser/
