# Email Notifier
Simple service to send an email with the IP address on boot.

## Instructions
- `cp secrets.template.js secrets.js`
- Fill in secret values (Create a Google App Password)
- `npm start`

## Make the script run on startup
- `sudo cp email-notify-up /etc/network/if-up.d`

