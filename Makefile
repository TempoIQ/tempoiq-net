.PHONY: compile compile-test update update-build update-test clean clean-build clean-test clean-packages

compile:
		xbuild TempoIQ/TempoIQ.csproj

compile-test: compile
		xbuild TempoIQ.Tests/TempoIQ.Tests.csproj

compile-snippets: compile
		xbuild TempoIQ.Snippets/TempoIQ.Snippets.csproj

compile-all: compile-snippets compile-test

update-build:
		xbuild TempoIQ/TempoIQ.csproj /t:RestorePackages

update-test:
		xbuild TempoIQ.Tests/TempoIQ.Tests.csproj /t:RestorePackages

update-snippets:
		xbuild TempoIQ.Snippets/TempoIQ.Snippets.csproj /t:RestorePackages

update: update-build update-test update-snippets

test: compile-test
		mono packages/NUnit.Runners.2.6.4/tools/nunit-console.exe TempoIQ.Tests/bin/Debug/TempoIQTests.dll

snippets: compile-snippets
		mono packages/NUnit.Runners.2.6.4/tools/nunit-console.exe TempoIQ.Snippets/bin/Debug/TempoIQSnippets.dll

check: test

clean-build:
		rm -rf TempoIQ/bin
			rm -rf TempoIQ/obj

clean-test:
		rm -rf TempoIQ.Tests/bin
			rm -rf TempoIQ.Tests/obj

clean-snippets:
		rm -rf TempoIQ.Snippets/bin
			rm -rf TempoIQ.Snippets/obj

clean-packages:
		rm -rf packages

clean: clean-build clean-test clean-snippets

clean-all: clean clean-packages
