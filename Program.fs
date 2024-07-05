open System
open System.Text
open System.Threading.Tasks
open Azure.Messaging.ServiceBus
open Argu

open Types
open Attendee
open Configuration

let receiveHandler (args: ProcessMessageEventArgs) =
    task {
        let api = Encoding.UTF8.GetString(args.Message.Body)
        let! attendee = getAttendee api
        printfn "%A %b" attendee (Printer.print attendee)

        do! args.CompleteMessageAsync(args.Message);
    } :> Task

let exceptionReceivedhandler (args: ProcessErrorEventArgs) =
    task {
        printfn "*** %A" args.Exception
    } :> Task

let server () = 
    task {
        use client = new ServiceBusClient(cfg.ServiceBusConnection);
        use processor = client.CreateProcessor(cfg.ServiceBusQueue, ServiceBusProcessorOptions(AutoCompleteMessages = false, MaxConcurrentCalls = 1))

        processor.add_ProcessMessageAsync receiveHandler
        processor.add_ProcessErrorAsync exceptionReceivedhandler

        do! processor.StartProcessingAsync()

        printfn "Waiting for icoming events. Press <Enter> to quit..."
        Console.ReadLine() |> ignore
    }

[<CliPrefix(CliPrefix.Dash)>]
type PrintArgs =
     | [<AltCommandLine("-f")>] FirstName of string
     | [<AltCommandLine("-l")>] LastName of string
     | [<AltCommandLine("-t")>] TicketType of string option

     interface IArgParserTemplate with
        member this.Usage =
            match this with
            | FirstName _ -> "First Name"
            | LastName _ -> "Last Name"
            | TicketType _ -> "Ticket Type"

and  PrintServerArgs =
    | [<CliPrefix(CliPrefix.None)>] Server
    | [<CliPrefix(CliPrefix.None)>] Print of ParseResults<PrintArgs>

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Server -> "Listener mode"
            | Print _ -> "Print mode"

let parser = ArgumentParser.Create<PrintServerArgs>(programName = "print-server.exe")

[<EntryPoint>]
let main argv =
    try
        let res = parser.ParseCommandLine(inputs = argv, raiseOnUsage = true)
        let arg = res.GetAllResults()

        match arg with
        |  [Server] -> server().Wait()
        |  [Print printArgs] ->
            let attendee = {
                FirstName = printArgs.GetResult(FirstName)
                LastName = printArgs.GetResult(LastName)
                Ticket = printArgs.GetResult(TicketType)
            }
            printfn "%A %b" attendee (Printer.print attendee)
        | _ -> printfn "%s" (parser.PrintUsage())
        0
    with :? ArguParseException as e -> eprintfn "%s" e.Message; 1