.PHONY: compile compile-test update clean clean-build clean-test clean-packages

install-deps:
	EnableNuGetPackageRestore=true nuget install TempoIQ/packages.config -o packages & nuget install TempoIQ.Tests/packages.config -o packages

compile:
	xbuild TempoIQ/TempoIQ.csproj

compile-snippets: compile
	xbuild TempoIQ.Snippets/TempoIQ.Snippets.csproj

compile-test: compile
	xbuild TempoIQ.Tests/TempoIQ.Tests.csproj

compile-all: update compile compile-test compile-snippets

update:
	nuget restore TempoIQ.sln

test: compile-test
	mono packages/NUnit.Runners.2.6.4/tools/nunit-console.exe TempoIQ.Tests/bin/Debug/TempoIQTests.dll

check: test

check-snippets:
	mono packages/NUnit.Runners.2.6.4/tools/nunit-console.exe TempoIQ.Snippets/bin/Debug/TempoIQSnippets.dll

clean-build:
	rm -rf TempoIQ/bin
	rm -rf TempoIQ/obj

clean-test:
	rm -rf TempoIQ.Tests/bin
	rm -rf TempoIQ.Tests/obj

clean-packages:
	rm -rf packages

clean: clean-build clean-test
