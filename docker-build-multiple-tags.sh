#!/bin/bash

# Example usage:
# ./docker-build-multiple-tags.sh reg.io/usr/img Dockerfile . v1 latest production
# 
# Will build image with following tags:
#   reg.io/usr/img:v1
#   reg.io/usr/img:latest
#   reg.io/usr/img:production

IMAGE=$1
FILE=$2
CONTEXT=$3
FIRST_TAG=$4; shift 4
OTHER_TAGS=$@

printf "\n\nBUILD IMAGE\n"
docker build -f $FILE -t $IMAGE:$FIRST_TAG $CONTEXT

printf "\n\nADD TAGS TO IMAGE\n"
for TAG in $OTHER_TAGS;
do
    docker tag $IMAGE:$FIRST_TAG $IMAGE:$TAG
done

