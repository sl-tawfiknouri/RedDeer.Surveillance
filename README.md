# Surveillance

==================== Surveillance analysis components ====================

To build set up surveillance then relay then test harness.

Data stream runs from (upstream -> downstream) test harness; relay; surveillance service.

==================== A W S ====================

If you're unable to access AWS queues and are not on EC2 you will be in need of a credentials file. For a windows machine the file needs to be in a default location, one of which is 'C:\Users\your.username\.aws\credentials'. A credentials file consists of profiles which are defined by closed square brackets then followed by two lines of text. Below is an example of a plain text credentials files with two profiles. Paste in the relevant access and secret keys.

[development]
aws_access_key_id =
aws_secret_access_key =
[default]
aws_access_key_id =
aws_secret_access_key =

==================== END ====================
