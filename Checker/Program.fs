// Student Attendance Checker for CS220 @ KAIST
// Author: Sang Kil Cha <sangkilc@kaist.ac.kr>

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors
open Suave.ServerErrors
open System.Threading
open System.Net
open System.Net.Sockets
open System.Collections.Concurrent

let parseLine (line: string) =
  let arr = line.Split(',')
  arr.[0].Split(' ') |> Seq.last |> (fun s -> s.ToLower ()),
  arr.[1]

let students =
  System.IO.File.ReadLines "students.csv"
  |> Seq.fold (fun m line ->
               let lastname, sid = parseLine line
               Map.add sid lastname m) Map.empty

let attendanceQueue = ConcurrentQueue<string> ()

let index mainpath = "\
<html>
  <body>
    <h1>Student Attendance Checking System.</h1>
    <form method=\"post\" action=\"" + mainpath + "/submit\">
      Student ID: <input type=\"text\" name=\"sid\"/> <br/>
      Last Name: <input type=\"text\" name=\"lastname\"/> <br/>
      <input type=\"submit\"/>
    </form>
  </body>
</html>"

let submit (req: HttpRequest) =
  match req.formData "sid", req.formData "lastname" with
  | Choice1Of2 sid, Choice1Of2 lastname ->
    match Map.tryFind sid students with
    | Some n when n = (lastname.ToLower ()) ->
      attendanceQueue.Enqueue (sid) |> ignore
      OK "done"
    | _ -> INTERNAL_ERROR "Invalid request."
  | _ -> INTERNAL_ERROR "Invalid request."

let app mainpath =
  choose
    [ GET >=> choose [ path mainpath >=> OK (index mainpath) ]
      POST >=> choose [ path (mainpath + "/submit") >=> request submit ]
      NOT_FOUND "Stay away! Don't play with the system." ]

let getConfig ip port token =
  { defaultConfig with
      cancellationToken = token
      bindings = [ HttpBinding.createSimple HTTP ip port ]
      listenTimeout = System.TimeSpan.FromMilliseconds 3000. }

let getMyIP () =
  use socket =
    new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
  socket.Connect ("8.8.8.8", 65530) // Open an arbitrary connection.
  let endPoint = socket.LocalEndPoint :?> IPEndPoint
  endPoint.Address.ToString ()

let rec timer sec = async {
  do! Async.Sleep 1000
  if sec < 0 then return ()
  else
    System.Console.Write (sprintf "\r\t%d seconds left." sec)
    return! timer (sec - 1)
}

let finalize startTime =
  attendanceQueue
  |> Seq.fold (fun set sid -> Set.add sid set) Set.empty
  |> Set.fold (fun acc sid -> acc + sid + "\n") ""
  |> (fun s ->
      let filename = startTime + ".attendance"
      System.IO.File.WriteAllText (filename, s))

let randomStr n =
  let chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWUXYZ0123456789"
  let len = String.length chars
  let r = new System.Random ()
  System.String (Array.init n (fun _ -> chars.[r.Next(len)]))

[<EntryPoint>]
let main argv =
  use cts = new CancellationTokenSource ()
  let startTime = System.DateTime.Now.ToString ("yyyy.MM.dd-H.mm.ss")
  let sec = System.Convert.ToInt32 (argv.[0])
  let myip = getMyIP ()
  let myport = 8080
  let cfg = getConfig myip myport cts.Token
  let mainpath = "/" + randomStr 10
  let _, server = startWebServerAsync cfg (app mainpath)
  Async.Start (server, cts.Token)
  Thread.Sleep (1000)
  printfn "\n\nNow connect to http://%s:%d%s\n\n" myip myport mainpath
  Async.RunSynchronously (timer sec)
  cts.Cancel ()
  System.Console.WriteLine ()
  finalize startTime
  0
