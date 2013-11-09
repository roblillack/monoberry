#!/bin/sh
PREFIX=/usr/local

if [ `id -u` -gt 0 ]; then
	echo "Please run this command as root." > /dev/stderr
	exit 1
fi

if [ "x`uname`" = "xDarwin" ]; then
	DESTDIR=/Developer/SDKs/MonoBerry
else
	DESTDIR="$PREFIX"/share/monoberry
fi

mkdir -p "$DESTDIR"
cp -r target/* "$DESTDIR"
printf '#!/bin/sh\nmono '"$DESTDIR"'/tool/monoberry.exe $@\n' > "$PREFIX"/bin/monoberry
chmod a+rx "$PREFIX"/bin/monoberry