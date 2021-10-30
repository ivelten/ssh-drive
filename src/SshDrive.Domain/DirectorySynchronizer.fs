module SshDrive.Domain.DirectorySynchronizer

open System
open System.Diagnostics
open System.IO

let startSync path userName (plink : Process) (psftp : Process) =
    let directoryExists = Commands.directoryExists plink
    let fileExists = Commands.fileExists plink
    let moveFile = Commands.moveFile plink
    let createDirectory = Commands.createDirectory plink
    let removeFile = Commands.removeFile plink
    let uploadFile = Commands.uploadFile psftp

    let getRemoteDirectory localDirectory =
        let dirPath =
            if not (Seq.last localDirectory = Path.DirectorySeparatorChar)
            then sprintf "%s%c" localDirectory Path.DirectorySeparatorChar
            else localDirectory
        let dirUri = Uri(dirPath)
        let workDirPath =
            if not (Seq.last path = Path.DirectorySeparatorChar)
            then sprintf "%s%c" path Path.DirectorySeparatorChar
            else path
        let workDirUri = Uri(workDirPath)
        let relativeUri = workDirUri.MakeRelativeUri(dirUri).ToString() |> Uri.UnescapeDataString
        let result = sprintf "/%s/%s" userName relativeUri
        result.TrimEnd('/')

    let onFileChanged (args : FileSystemEventArgs) =
        let remoteFile = getRemoteDirectory args.FullPath
        let remoteDirectory = getRemoteDirectory (Path.GetDirectoryName(args.FullPath))
        if fileExists remoteFile then removeFile remoteFile
        uploadFile args.FullPath remoteDirectory

    let onFileCreated (args : FileSystemEventArgs) =
        let isDirectory = File.GetAttributes(args.FullPath).HasFlag(FileAttributes.Directory)
        let localDirectory = if isDirectory then args.FullPath else Path.GetDirectoryName(args.FullPath)
        let remoteDirectory = getRemoteDirectory localDirectory
        if isDirectory then createDirectory remoteDirectory
        else
            if not (directoryExists remoteDirectory) then createDirectory remoteDirectory
            uploadFile args.FullPath remoteDirectory

    let onFileDeleted (args : FileSystemEventArgs) =
        let remotePath = getRemoteDirectory args.FullPath
        if fileExists remotePath then removeFile remotePath

    let onFileRenamed (args : RenamedEventArgs) =
        let isDirectory = File.GetAttributes(args.FullPath).HasFlag(FileAttributes.Directory)
        let exists path =
            if isDirectory
            then directoryExists path
            else fileExists path
        let oldRemotePath = getRemoteDirectory args.OldFullPath
        let newRemotePath = getRemoteDirectory args.FullPath
        if exists oldRemotePath then moveFile oldRemotePath newRemotePath

    let onError (args : ErrorEventArgs) =
        args.GetException() |> Console.WriteLine

    let watcher = new FileSystemWatcher(path)

    watcher.IncludeSubdirectories <- true
    watcher.EnableRaisingEvents <- true
    watcher.Changed.Add onFileChanged
    watcher.Created.Add onFileCreated
    watcher.Deleted.Add onFileDeleted
    watcher.Renamed.Add onFileRenamed
    watcher.Error.Add onError

    watcher