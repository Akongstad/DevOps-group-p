#!/bin/bash

IMAGE=$1; shift
TAGS=$@

printf "\n\nPUSH IMAGE WITH MULTIPLE TAGS\n"
for TAG in $TAGS;
do
    docker image push $IMAGE:$TAG
done
