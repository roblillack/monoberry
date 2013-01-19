TARGET=target
MONOSRC=mono
PREFIX=/usr/local
SYSTEM:=$(shell uname)
BASE:=$(shell pwd)

ifeq (${SYSTEM}, Darwin)
  DESTDIR=/Developer/SDKs/MonoBerry
  NDK=/Applications/bbndk
else
  DESTDIR=/usr/local/share/monoberry
  NDK=/opt/bbndk
endif

all:	cli mono libs

cli:	${TARGET}/tool/monoberry.exe

${TARGET}/tool/monoberry.sh: tooling/monoberry.sh
	cp $< $@

target/tool/monoberry.exe: tooling/*.cs tooling/tool.csproj
	@rm -rf tooling/bin/Release
	@echo Building MonoBerry CLI tool ...
	@xbuild tooling/tool.csproj /p:Configuration=Release > /dev/null
	@mkdir -p target/tool
	@cp tooling/bin/Release/*.exe tooling/bin/Release/*.dll tooling/monoberry.sh ${TARGET}/tool

clean:
	@rm -rf tooling/bin/ target/ libblackberry/bin
	@rm -f ${PREFIX}/bin/monoberry

install:
	@mkdir -p ${DESTDIR}
	@cp -r ${TARGET}/* ${DESTDIR}
	@printf '#!/bin/sh\nmono '${DESTDIR}'/tool/monoberry.exe $$@\n' > ${PREFIX}/bin/monoberry
	@chmod a+rx ${PREFIX}/bin/monoberry

libs:	${TARGET}/lib/mono/4.0/libblackberry.dll

${TARGET}/lib/mono/4.0/libblackberry.dll: libblackberry/*.cs libblackberry/libblackberry.csproj
	@echo Building libblackberry ...
	@xbuild libblackberry/libblackberry.csproj /p:Configuration=Release > /dev/null
	@mkdir -p ${TARGET}/lib/mono/4.0
	@cp libblackberry/bin/Release/*.dll target/lib/mono/4.0

#helloworld: helloworld/*.cs helloworld/*.xml
#	@xbuild helloworld/helloworld.csproj /p:Configuration=Release

mono:	${TARGET}/lib/mono/2.0/mscorlib.dll ${TARGET}/lib/mono/4.0/mscorlib.dll ${TARGET}/target/armle-v7/bin/mono ${TARGET}/target/armle-v7/lib/libgdiplus.so.0 ${TARGET}/target/x86/bin/mono

${TARGET}/target/x86/bin/mono: ${MONOSRC}/autogen.sh
	echo "SKIPPING X86 BUILD -- NO SIMULATOR SUPPORT RIGHT NOW."
	#cd ${MONOSRC} && . ${NDK}/bbndk-env.sh && env LDFLAGS="-L${NDK}/target/qnx6/x86/lib -L${NDK}/target/qnx6/x86/usr/lib" ./autogen.sh --host=i486-pc-nto-qnx8.0.0 --with-xen-opt=no --with-large-heap=no --disable-mcs-build --enable-small-config=yes && make clean && make
	#mkdir -p `dirname $@`
	#install ${MONOSRC}/mono/mini/mono $@

libffi/arm-unknown-nto-qnx8.0.0eabi/.libs/libffi.a: libffi/configure
	cd libffi &&\
	. ${NDK}/bbndk-env.sh &&\
	env LDFLAGS="-L${NDK}/target/qnx6/armle-v7/lib -L${NDK}/target/qnx6/armle-v7/usr/lib" \
	./configure --host=arm-unknown-nto-qnx8.0.0eabi && make

glib/glib/libglib-2.0.la: glib/autogen.sh libffi/arm-unknown-nto-qnx8.0.0eabi/.libs/libffi.a
	cp glib.cache glib/config.cache
	cd glib &&\
	. ${NDK}/bbndk-env.sh &&\
	env LDFLAGS="-lsocket"\
		CFLAGS="-DSA_RESTART=0 -D_QNX_SOURCE"\
		LIBFFI_CFLAGS="-I${BASE}/libffi/arm-unknown-nto-qnx8.0.0eabi/include"\
		LIBFFI_LIBS="${BASE}/libffi/arm-unknown-nto-qnx8.0.0eabi/libffi.la"\
		./autogen.sh --enable-static=yes --enable-shared=no --host=arm-unknown-nto-qnx8.0.0eabi\
		--cache-file=config.cache &&\
	make clean &&\
	make

${TARGET}/target/armle-v7/lib/libgdiplus.so.0: libgdiplus/autogen.sh glib/glib/libglib-2.0.la
	cd libgdiplus &&\
	. ${NDK}/bbndk-env.sh &&\
	env CFLAGS="-I$$QNX_TARGET/usr/include/freetype2 -I../cairo/src -I${BASE}/glib -I${BASE}/glib/glib"\
		GDIPLUS_LIBS="${BASE}/glib/glib/.libs/libglib-2.0.a -lintl -liconv"\
		./autogen.sh --enable-static=no --enable-shared=yes --host=arm-unknown-nto-qnx8.0.0eabi &&\
	make clean &&\
	make
	mkdir -p `dirname $@`
	install libgdiplus/src/.libs/libgdiplus.so.0 $@

${TARGET}/target/armle-v7/bin/mono: ${MONOSRC}/autogen.sh
	cd ${MONOSRC} && . ${NDK}/bbndk-env.sh && env LDFLAGS="-L${NDK}/target/qnx6/armle-v7/lib -L${NDK}/target/qnx6/armle-v7/usr/lib" ./autogen.sh --host=arm-unknown-nto-qnx8.0.0eabi --with-xen-opt=no --with-large-heap=no --disable-mcs-build --enable-small-config=yes && make clean && make
	mkdir -p `dirname $@`
	install ${MONOSRC}/mono/mini/mono $@

${TARGET}/lib/mono/2.0/mscorlib.dll: ${MONOSRC}/autogen.sh
	cd ${MONOSRC} && ./autogen.sh && make
	mkdir -p `dirname $@`
	install ${MONOSRC}/mcs/class/lib/net_2_0/mscorlib.dll $@

${TARGET}/lib/mono/4.0/mscorlib.dll: ${TARGET}/lib/mono/2.0/mscorlib.dll
	mkdir -p `dirname $@`
	install ${MONOSRC}/mcs/class/lib/net_4_0/mscorlib.dll $@

${MONOSRC}/autogen.sh: submodules
glib/autogen.sh: submodules
libffi/configure: submodules

submodules: .gitmodules
	git submodule update --init --recursive

.PHONY: clean all install
