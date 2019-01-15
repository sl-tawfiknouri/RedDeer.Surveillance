#!/bin/bash

# Change service folder ownership.
chown -R reddeer:reddeer /reddeer/ ThirdPartySurveillanceDataSynchroniser
find /reddeer/ ThirdPartySurveillanceDataSynchroniser -type d -exec chmod g+s {} \;

# Create systemd unit file.
cp /reddeer/ ThirdPartySurveillanceDataSynchroniser/ ThirdPartySurveillanceDataSynchroniser.service /lib/systemd/system/

# Reload daemons and start the new service.
systemctl daemon-reload
systemctl enable  ThirdPartySurveillanceDataSynchroniser
systemctl start  ThirdPartySurveillanceDataSynchroniser
