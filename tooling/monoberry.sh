#!/bin/sh

BASE=`cd "\`dirname "\\\`readlink "$0" || echo $0\\\`"\`" && pwd`

ASSEMBLY=
for i in $BASE/monoberry.exe $BASE/bin/Release/monoberry.exe $BASE/bin/Debug/monoberry.exe; do
  if [ -r $i ]; then
    ASSEMBLY=$i
    break
  fi
done

if [ -z $ASSEMBLY ]; then
  echo "Unable to find monoberry.exe" > /dev/stderr
  exit 1
fi

mono $ASSEMBLY $@
