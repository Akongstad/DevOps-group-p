#!/bin/bash

sudo chmod +x logging/setup_elk.sh
source logging/setup_elk.sh

git pull 

for service in {frontend,backend}; do
    docker-compose -f compose.prod.yaml rm -sf $service && docker-compose -f compose.prod.yaml pull $service && docker-compose -f compose.prod.yaml up -d --no-deps $service;
done

for service in {db,prometheus,letsencrypt,elasticsearch,kibana,filebeat}; do
    docker-compose -f compose.prod.yaml restart $service;
done
