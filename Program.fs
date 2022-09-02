open System
open System.Text
open System.Threading.Tasks
open Azure.Messaging.ServiceBus

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

let main = 
    task {
        use client = new ServiceBusClient(cfg.ServiceBusConnection);
        use processor = client.CreateProcessor(cfg.ServiceBusQueue, ServiceBusProcessorOptions(AutoCompleteMessages = false, MaxConcurrentCalls = 1))

        processor.add_ProcessMessageAsync receiveHandler
        processor.add_ProcessErrorAsync exceptionReceivedhandler

        do! processor.StartProcessingAsync()

        printfn "Waiting for icoming events. Press <Enter> to quit..."
        Console.ReadLine() |> ignore
    }

main.Wait()
