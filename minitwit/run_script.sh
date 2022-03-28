#!/bin/bash

sudo chmod +x logging/setup_elk.sh
source setup_elk.sh

sudo htpasswd -c .htpasswd devops22

git pull 
echo "Taking down frontend" 
docker stop react_frontend &&
docker rm react_frontend &&
echo "Taking down backend" &&
docker stop dotnet_backend &&
docker rm dotnet_backend &&
echo "Pulling from hub" &&
docker-compose -f compose.prod.yaml pull &&
echo "Building containers" &&
docker-compose -f compose.prod.yaml up --build -d
