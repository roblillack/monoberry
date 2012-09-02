#!/bin/sh

BASE=`cd "\`dirname "\\\`readlink "$0" || echo $0\\\`"\`" && pwd`

ASSNAME=monoberry.exe
ASSEMBLY=`find $BASE/bin -name $ASSNAME | xargs ls -t | head -1`

if [ -z $ASSEMBLY ]; then
  echo "Unable to find $ASSNAME" > /dev/stderr
  exit 1
fi

mono $ASSEMBLY $@
