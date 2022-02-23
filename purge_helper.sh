#!/bin/bash

KEYWORD=$1
DIRECTORY=$2
ADD_GREP_PARAM=$3 # Option for additional grep flags

if [[ $ADD_GREP_PARAM ]]; then
    STATEMENT="with grep param $ADD_GREP_PARAM"
else
    STATEMENT=''
fi

echo;echo;echo
echo "Printing all instances of $KEYWORD in $DIRECTORY $STATEMENT"
echo
grep -irn $ADD_GREP_PARAM "$DIRECTORY" -e "$KEYWORD" 2>/dev/null | awk '$0 !~ /\/bin\//' | awk '$0 !~ /\/obj\//' | awk '{print $1}'
