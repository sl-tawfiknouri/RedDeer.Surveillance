#!/bin/bash

# Change service folder ownership.
chown -R reddeer:reddeer /reddeer/surveillanceapiservice
find /reddeer/surveillanceapiservice -type d -exec chmod g+s {} \;

# Create systemd unit file.
cp /reddeer/surveillanceapiservice/surveillanceapiservice.service /lib/systemd/system/

# Reload daemons and start the new service.
systemctl daemon-reload
systemctl enable surveillanceapiservice
systemctl start surveillanceapiservice
