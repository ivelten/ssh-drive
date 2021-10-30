module SshDrive.Domain.Processes

open System
open System.Diagnostics
open System.IO

let addOutput processName (args : DataReceivedEventArgs) =
    if not (String.IsNullOrWhiteSpace args.Data) then
        sprintf "%s: %s" processName args.Data
        |> Console.WriteLine
    match Boolean.TryParse(args.Data) with
    | (true, data) -> Commands.setLastBoolResponse (Some data)
    | _ -> ()

let startPlinkProcess parameters =
    let p = new Process()
    let processName = "plink.exe"
    p.StartInfo.FileName <- Path.Combine(parameters.PuttyPath, processName)
    p.StartInfo.UseShellExecute <- false
    p.StartInfo.RedirectStandardInput <- true
    p.StartInfo.RedirectStandardOutput <- true
    p.StartInfo.RedirectStandardError <- true
    p.StartInfo.CreateNoWindow <- true
    p.StartInfo.Arguments <- sprintf "-ssh %s@%s -P %i -pw %s" parameters.UserName parameters.HostName parameters.Port parameters.Password
    p.OutputDataReceived.Add <| addOutput processName
    p.Start() |> ignore
    p.BeginOutputReadLine()
    p

let startPsftpProcess parameters =
    let p = new Process()
    let processName = "psftp.exe"
    p.StartInfo.FileName <- Path.Combine(parameters.PuttyPath, processName)
    p.StartInfo.UseShellExecute <- false
    p.StartInfo.RedirectStandardInput <- true
    p.StartInfo.RedirectStandardOutput <- true
    p.StartInfo.RedirectStandardError <- true
    p.StartInfo.CreateNoWindow <- true
    p.StartInfo.Arguments <- sprintf "-P %i -pw %s %s@%s" parameters.Port parameters.Password parameters.UserName parameters.HostName
    p.OutputDataReceived.Add <| addOutput processName
    p.Start() |> ignore
    p.BeginOutputReadLine()
    p