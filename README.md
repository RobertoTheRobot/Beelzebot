# Beelzebot
Discord application for testing


## Docker compose
```yaml
version: "2"

services:
  beelzebot:
    container_name: beelzebot
    image: robe137/beelzebotwebapi:latest
    ports:
      - "4445:4445/tcp" 
    environment:
      Secret_DiscordBotToken: '{TOKEN}'
      TZ: 'Europe/Amsterdam'
    restart: unless-stopped
```
