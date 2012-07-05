all:	tool mono libs

tool:	target/tool/monoberry.exe
	@rm -rf tooling/bin/Release
	@echo Building MonoBerry CLI tool ...
	@xbuild tooling/tool.csproj /p:Configuration=Release > /dev/null
	@mkdir -p target/tool
	@cp tooling/bin/Release/*.exe tooling/bin/Release/*.dll tooling/monoberry.sh target/tool

clean:
	@rm -rf tooling/bin/ target/

#mono:
