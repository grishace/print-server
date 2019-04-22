open System
open System.Text
open System.Threading.Tasks
open Microsoft.Azure.ServiceBus

open Attendee
open Configuration

[<EntryPoint>]
let main _ =

    let receiveHandler (m: Message) _ =
        async {
            let api = Encoding.UTF8.GetString(m.Body)
            let! attendee = getAttendee api
            printfn "%A\n" attendee
            Printer.print {
                attendee with
                    LinkedIn = QR.generate @"assets\linkedin.bmp" attendee.LinkedIn
                    Twitter = QR.generate @"assets\twitter.bmp" attendee.Twitter
            }
        } |> Async.StartAsTask :> Task

    let exceptionReceivedhandler (m: ExceptionReceivedEventArgs) =
        async {
            printfn "*** %A" m.Exception
        } |> Async.StartAsTask :> Task

    async {
        let client = QueueClient(ServiceBusConnectionStringBuilder(cfg.ServiceBusQueue))
        let options = MessageHandlerOptions(Func<_,_>(exceptionReceivedhandler), AutoComplete = true)
        client.RegisterMessageHandler(receiveHandler, options)
        printfn "Waiting for icoming events. Press <Enter> to quit..."

        Console.ReadLine() |> ignore

        do! client.CloseAsync() |> Async.AwaitTask
    } |> Async.RunSynchronously

    0

