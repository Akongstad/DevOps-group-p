#!/bin/bash

IMAGE=$1; 
LOCAL_TAG=$2; 
EXTERNAL_TAG=$3; shift 3
UPDATE_COMMAND=$@

docker pull $IMAGE:$EXTERNAL_TAG

LOCAL_DIGEST=$( docker images --no-trunc --quiet $IMAGE:$LOCAL_TAG )
EXTERNAL_DIGEST=$( docker images --no-trunc --quiet $IMAGE:$EXTERNAL_TAG )

if [[ "$LOCAL_DIGEST" = "$EXTERNAL_DIGEST" ]]; then
    printf "\n\nNothing to push\n"
else
    printf "\n\nRunning update command\n"
    $UPDATE_COMMAND
fi
