.PHONY: compile compile-test update update-build update-test clean clean-build clean-test clean-packages

compile:
		xbuild TempoIQ/TempoIQ.csproj

compile-test: compile
		xbuild TempoIQ.Tests/TempoIQ.Tests.csproj

update-build:
		xbuild TempoIQ/TempoIQ.csproj /t:RestorePackages

update-test:
		xbuild TempoIQ.Tests/TempoIQ.Tests.csproj /t:RestorePackages

update: update-build update-test

test: compile-test
		mono packages/NUnit.Runners.2.6.4/tools/nunit-console.exe --exclude=Snippet TempoIQ.Tests/bin/Debug/TempoIQTests.dll

snippet: compile-test
		mono packages/NUnit.Runners.2.6.4/tools/nunit-console.exe --include=Snippet TempoIQ.Tests/bin/Debug/TempoIQTests.dll

check: test

clean-build:
		rm -rf TempoIQ/bin
			rm -rf TempoIQ/obj

clean-test:
		rm -rf TempoIQ.Tests/bin
			rm -rf TempoIQ.Tests/obj

clean-packages:
		rm -rf packages

clean: clean-build clean-test

clean-all: clean clean-packages
