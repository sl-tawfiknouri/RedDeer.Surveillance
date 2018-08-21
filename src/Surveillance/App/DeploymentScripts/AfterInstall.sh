#!/bin/bash

# Change service folder ownership.
chown -R reddeer:reddeer /reddeer/surveillanceservice
find /reddeer/surveillanceservice -type d -exec chmod g+s {} \;

# Create systemd unit file.
cp /reddeer/surveillanceservice/surveillanceservice.service /lib/systemd/system/

# Reload daemons and start the new service.
systemctl daemon-reload
systemctl enable surveillanceservice
systemctl start surveillanceservice
