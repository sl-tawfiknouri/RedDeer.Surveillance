#!/bin/bash

# Change service folder ownership.
chown -R reddeer:reddeer /reddeer/dataimportservice
find /reddeer/dataimportservice -type d -exec chmod g+s {} \;

# Create systemd unit file.
cp /reddeer/dataimportservice/dataimportservice.service /lib/systemd/system/

# Reload daemons and start the new service.
systemctl daemon-reload
systemctl enable dataimportservice
systemctl start dataimportservice
