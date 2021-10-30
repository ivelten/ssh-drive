# SSH Drive

Serviço de backup de arquivos montado com servidor SSH e Putty.

## Pré-requisitos

1. Docker instalado e ativo
2. .NET Core 5.0

## Como utilizar

1. Execute o script [start-server.cmd](scripts/start-server.cmd). Este arquivo irá iniciar um container Docker com um servidor SSH na máquina.
2. Compile o projeto indo para a pasta raiz executando `dotnet build`.
3. Edite o arquivo [appsettings.json](src/SshDrive.App/appsettings.json) da aplicação, configurando os valores corretos para a conexão com o servidor SSH. Os valores já colocados são os valores padrão para o servidor levantado via Docker. O valor da propriedade `PuttyPath` assume o local de instalação padrão do Putty.
4. Ainda no arquivo `appsettings.json`, defina o diretório que será sincronizado com o servidor na propriedade `Path`. O padrão é `C:\Temp`.
5. Faça alterações no diretório e verifique o servidor sincronizar os arquivos e pastas.
