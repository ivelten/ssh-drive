namespace SshDrive.Domain

[<CLIMutable>]
type Parameters =
    { Path : string
      PuttyPath : string
      HostName : string
      Port : int
      UserName : string
      Password : string }