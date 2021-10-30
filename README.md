# SSH Drive

Serviço de backup de arquivos montado com servidor SSH e Putty.

## Pré-requisitos

1. Docker instalado e ativo
2. .NET Core 5.0

## Como utilizar

1. Entre [no diretório de scripts](scripts) e execute o arquivo `start-server.cmd`. Este arquivo irá iniciar um container Docker com um servidor SSH na máquina.
2. Compile o projeto indo para a pasta raiz executando `dotnet build`.
3. Entre [no diretório da aplicação](src/SshDrive.App) e edite o arquivo `appsettings.json`, configurando os valores corretos para a conexão com o servidor SSH. Os valores já colocados são os valores padrão para o servidor levantado via Docker. O valor da propriedade `PuttyPath` assume o local de instalação padrão do Putty.
4. Ainda no arquivo `appsettings.json`, defina o diretório que será sincronizado com o servidor na propriedade `Path`. O padrão é `C:\Temp`.
5. Faça alterações no diretório e verifique o servidor sincronizar os arquivos e pastas.
