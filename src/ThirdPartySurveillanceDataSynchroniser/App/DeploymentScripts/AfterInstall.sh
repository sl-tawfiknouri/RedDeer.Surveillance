#!/bin/bash

# Change service folder ownership.
chown -R reddeer:reddeer /reddeer/thirdpartysurveillancedatasynchroniserservice
find /reddeer/thirdpartysurveillancedatasynchroniserservice -type d -exec chmod g+s {} \;

# Create systemd unit file.
cp /reddeer/thirdpartysurveillancedatasynchroniserservice/thirdpartysurveillancedatasynchroniserservice.service /lib/systemd/system/

# Reload daemons and start the new service.
systemctl daemon-reload
systemctl enable thirdpartysurveillancedatasynchroniserservice
systemctl start thirdpartysurveillancedatasynchroniserservice
