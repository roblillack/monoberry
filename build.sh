#!/bin/sh

set -e

NDK=/Developer/SDKs/bbndk-10.0.4-beta
PREFIX=/accounts/devuser/mono
ARCH=x86

if [ "x$1" = "xarm" ]; then
  ARCH=armle-v7
  HOST=arm-unknown-nto-qnx8.0.0eabi
else
  ARCH=x86
  HOST=i486-pc-nto-qnx8.0.0
fi

source $NDK/bbndk-env.sh

echo "*** building for arch $ARCH ***"

ORIGDIR=`pwd`
BASE=`cd "\`dirname "\\\`readlink "$0" || echo $0\\\`"\`" && pwd`
DEST="$BASE/build-$ARCH"

if [ ! -d "$BASE/mono-qnx" ]; then
  echo "*** Downloading MONO ..."
  git clone git://github.com/rockpiper/mono.git "$BASE/mono-qnx"
  cd "$BASE/mono-qnx"
  git co qnx-2-10-9
  echo "*** Done."
fi

rm -rf "$DEST"

mkdir -p "$DEST/lib"
ln -s "$NDK/target/qnx6/$ARCH/lib/libstdc++.so.6" "$DEST/lib/libstdc++.so"

cd "$BASE/mono"
export LDFLAGS="-L$DEST/lib -L$NDK/target/qnx6/$ARCH/lib -L$NDK/target/qnx6/$ARCH/usr/lib" 
./autogen.sh\
  --host=$HOST\
  --prefix=$PREFIX\
  --with-xen-opt=no\
  --with-large-heap=no\
  --disable-mcs-build\
  --enable-small-config=yes

make
make install DESTDIR="$DEST"

#echo "*** Cleaning ..."
#make clean 2>&1 > /dev/null
#echo "*** Done."

#./configure --prefix=$PREFIX

#cd mcs
#make RUNTIME=mono MCS=gmcs

#make install DESTDIR="$BASE/target"

mkdir -p $DEST$PREFIX/lib/mono
cp -R /Library/Frameworks/Mono.framework/Versions/2.10.9/lib/mono/2.0 $DEST$PREFIX/lib/mono

cd "$ORIGDIR"
