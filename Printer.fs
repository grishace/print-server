module Printer

open System.IO
open Configuration
open QR
open Types

let print (attendee: Attendee) =

    let template, hasTemplate =
        (match attendee.LinkedIn, attendee.Twitter with
            | Some _, _ | _, Some _ -> cfg.Templates.QR
            | _ -> cfg.Templates.Default)
        |> Option.ofObj
        |> Option.map (fun t -> t, true)
        |> Option.defaultValue (cfg.Templates.Default, false)

    let doc, _ = Bpac.openTemplate (Path.Combine(cfg.Templates.TemplatesFolder, template))
    let setText = Bpac.setText doc
    setText "FirstName" attendee.FirstName
    setText "LastName" attendee.LastName
    setText "TicketType" (attendee.Ticket |> Option.defaultValue "")

    if hasTemplate then
        QR.generate attendee |> Option.iter (Bpac.setImage doc "QR")

    let job = sprintf "%s_%s" attendee.FirstName attendee.LastName
    if cfg.Print
        then Bpac.print doc cfg.MediaName job
        else
            let folder = Path.GetDirectoryName(Path.GetFullPath(cfg.Templates.TemplatesFolder))
            Bpac.export doc (sprintf "%s\\%s.bmp" folder job)
