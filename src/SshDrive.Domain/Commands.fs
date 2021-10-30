module SshDrive.Domain.Commands

open System.Diagnostics

let mutable private lastBoolResponse : bool option = None
let mutable private boolResponseLock = obj()

let setLastBoolResponse value =
    lock boolResponseLock (fun () -> lastBoolResponse <- value)

let getLastBoolResponse () =
    while lastBoolResponse = None do ()
    let result = lastBoolResponse.Value
    setLastBoolResponse None
    result

let executeCommand (p : Process) (command : string) =
    p.StandardInput.WriteLine(command)

let directoryExists (plink : Process) path =
    sprintf "[ -d '%s' ] && echo 'true' || echo 'false'" path
    |> executeCommand plink
    getLastBoolResponse ()

let fileExists (plink : Process) path =
    sprintf "[ -f '%s' ] && echo 'true' || echo 'false'" path
    |> executeCommand plink
    getLastBoolResponse ()

let createDirectory (plink : Process) path =
    sprintf "mkdir '%s'" path
    |> executeCommand plink

let uploadFile (psftp : Process) (localPath : string) remotePath =
    sprintf "cd \"%s\"" remotePath
    |> executeCommand psftp
    sprintf "put \"%s\"" localPath
    |> executeCommand psftp

let removeFile (plink : Process) path =
    sprintf "rm -r '%s'" path
    |> executeCommand plink

let moveFile (plink : Process) oldPath newPath =
    sprintf "mv '%s' '%s'" oldPath newPath
    |> executeCommand plink