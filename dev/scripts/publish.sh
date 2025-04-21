dotnet publish /t:PublishContainer -p:ContainerImageTag=0.0.2 -p ContainerRegistry=docker.io
dotnet publish /t:PublishContainer -p:ContainerImageTag=latest -p ContainerRegistry=docker.io