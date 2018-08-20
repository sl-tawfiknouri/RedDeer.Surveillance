#!/bin/bash

# Change service folder ownership.
chown -R reddeer:reddeer /reddeer/relayservice
find /reddeer/relayservice -type d -exec chmod g+s {} \;

# Create systemd unit file.
cp /reddeer/relayservice/relayservice.service /lib/systemd/system/

# Reload daemons and start the new service.
systemctl daemon-reload
systemctl enable relayservice
systemctl start relayservice
