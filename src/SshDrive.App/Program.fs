open System
open SshDrive.Domain
open Microsoft.Extensions.Configuration

let [<Literal>] returnCode = 0

[<EntryPoint>]
let main _ =
    let config = ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json", optional = false).Build()
    let section = config.GetSection(nameof(Parameters))
    let parameters = section.Get<Parameters>()
    let plink = Processes.startPlinkProcess parameters
    let psftp = Processes.startPsftpProcess parameters
    let fileSystemWatcher = DirectorySynchronizer.startSync parameters.Path parameters.UserName plink psftp
    let mutable keepRunning = true

    let cancelKeyPress (e : ConsoleCancelEventArgs) =
        fileSystemWatcher.Dispose()
        plink.Dispose()
        psftp.Dispose()
        keepRunning <- false
        e.Cancel <- true

    Console.CancelKeyPress.Add(cancelKeyPress)

    while keepRunning do ()

    returnCode