TARGET=target
DESTDIR=/Developer/SDKs/MonoBerry
MONOSRC=mono
NDK=/Developer/SDKs/bbndk-10.0.4-beta
PREFIX=/usr/local

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

tmp/x86/libstdc++.so: ${NDK}/target/qnx6/x86/lib/libstdc++.so.6
	@mkdir -p `dirname $@`
	@ln -s $< $@

tmp/armle-v7/libstdc++.so: ${NDK}/target/qnx6/armle-v7/lib/libstdc++.so.6
	@mkdir -p `dirname $@`
	@ln -s $< $@

mono:	${TARGET}/lib/mono/2.0/mscorlib.dll ${TARGET}/lib/mono/4.0/mscorlib.dll ${TARGET}/target/armle-v7/bin/mono ${TARGET}/target/x86/bin/mono

${TARGET}/target/x86/bin/mono: ${MONOSRC}/autogen.sh tmp/x86/libstdc++.so
	cd ${MONOSRC} && source ${NDK}/bbndk-env.sh && env LDFLAGS="-L`pwd`/../tmp/x86 -L${NDK}/target/qnx6/x86/lib -L${NDK}/target/qnx6/x86/usr/lib" ./autogen.sh --host=i486-pc-nto-qnx8.0.0 --with-xen-opt=no --with-large-heap=no --disable-mcs-build --enable-small-config=yes && make clean && make
	mkdir -p `dirname $@`
	install ${MONOSRC}/mono/mini/mono $@

${TARGET}/target/armle-v7/bin/mono: ${MONOSRC}/autogen.sh tmp/armle-v7/libstdc++.so
	cd ${MONOSRC} && source ${NDK}/bbndk-env.sh && env LDFLAGS="-L`pwd`/../tmp/armle-v7 -L${NDK}/target/qnx6/armle-v7/lib -L${NDK}/target/qnx6/armle-v7/usr/lib" ./autogen.sh --host=arm-unknown-nto-qnx8.0.0eabi --with-xen-opt=no --with-large-heap=no --disable-mcs-build --enable-small-config=yes && make clean && make
	mkdir -p `dirname $@`
	install ${MONOSRC}/mono/mini/mono $@

${TARGET}/lib/mono/2.0/mscorlib.dll: ${MONOSRC}/autogen.sh
	cd ${MONOSRC} && ./autogen.sh && make
	mkdir -p `dirname $@`
	install ${MONOSRC}/mcs/class/lib/net_2_0/mscorlib.dll $@

${TARGET}/lib/mono/4.0/mscorlib.dll: ${TARGET}/lib/mono/2.0/mscorlib.dll
	mkdir -p `dirname $@`
	install ${MONOSRC}/mcs/class/lib/net_4_0/mscorlib.dll $@

${MONOSRC}/autogen.sh: .gitmodules
	git submodule init
	git submodule update

.PHONY: clean all install
