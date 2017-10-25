FROM microsoft/dotnet

RUN apt-get -qq update && apt-get -y install netcat

COPY . app/
WORKDIR app/

RUN ["dotnet", "restore"]

WORKDIR PlayGen.SUGAR.Server.WebAPI/
RUN ["dotnet", "publish", "-c", "Release", "-o", "out"]

ENTRYPOINT ["./delay-startup.sh", "dotnet", "out/PlayGen.SUGAR.Server.WebAPI.dll"]
