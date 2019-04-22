module Printer

open System.IO
open Types
open Configuration

let print (attendee: Attendee) =
    let cnt opt = opt |> Option.map (fun _ -> 1) |> Option.defaultValue 0
    let qrs = (cnt attendee.LinkedIn) + (cnt attendee.Twitter)

    let template, hasTemplate =
        (match qrs with
            | 1 -> cfg.Templates.QR1
            | 2 -> cfg.Templates.QR2
            | _ -> cfg.Templates.Default)
        |> Option.ofObj
        |> Option.map (fun t -> t, true)
        |> Option.defaultValue (cfg.Templates.Default, false)

    let doc, _ = Bpac.openTemplate (Path.Combine(cfg.Templates.TemplatesFolder, template))
    let setText = Bpac.setText doc
    setText "FirstName" attendee.FirstName
    setText "LastName" attendee.LastName
    setText "TicketType" (attendee.Ticket |> Option.defaultValue "")

    if qrs > 0 && hasTemplate then
        let qrt n = if qrs = 1 then "QR" else n
        attendee.LinkedIn |> Option.iter (Bpac.setImage doc (qrt "QR1"))
        attendee.Twitter |> Option.iter (Bpac.setImage doc (qrt "QR2"))

    let job = sprintf "%s_%s" attendee.FirstName attendee.LastName
    if cfg.Print
        then Bpac.print doc cfg.MediaName job
        else
            let folder = Path.GetDirectoryName(Path.GetFullPath(cfg.Templates.TemplatesFolder))
            Bpac.export doc (sprintf "%s\\%s.bmp" folder job)
