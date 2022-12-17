#!/bin/sh
git pull
sudo docker-compose --env-file `pwd`/.env up -d --build
