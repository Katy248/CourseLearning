EXECUTABLE=cl-l.exe
PROJECT=CourseLearning.Desktop
CONFIGURATION=Release
TARGET_FRAMEWORK=net7.0-windows

all: build

build:
	dotnet build $(PROJECT) --nologo --configuration $(CONFIGURATION)

run: build
	./$(PROJECT)/bin/$(CONFIGURATION)/$(TARGET_FRAMEWORK)/$(PROJECT).exe
