#!/bin/bash

KEYWORD=$1          # Keyword to search for
DIRECTORY=$2        # Directory to search in
ADD_GREP_PARAM=$3   # Option for additional grep flags

if [[ $ADD_GREP_PARAM ]]; then
    STATEMENT="with grep param $ADD_GREP_PARAM"
else
    STATEMENT=''
fi


# Search for all instances of pattern in directory
grep -irn $ADD_GREP_PARAM "$DIRECTORY" -e "$KEYWORD" 2>/dev/null |
    
    # Filter out binary files
    awk '$0 !~ /\/bin\//' |

    # Filter out this dotnet dir
    awk '$0 !~ /\/obj\//' |

    # Print the first column (filename + lineno)
    awk '{print $1}'

echo;echo;echo
echo "Printing all instances of $KEYWORD in $DIRECTORY $STATEMENT"
echo
grep -irn $ADD_GREP_PARAM "$DIRECTORY" -e "$KEYWORD" 2>/dev/null | awk '$0 !~ /\/bin\//' | awk '$0 !~ /\/obj\//' | awk '{print $1}'
