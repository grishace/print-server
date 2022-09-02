module Printer

open System.IO
open Types
open Configuration

let print (attendee: Attendee) =

    let doc, _ = Bpac.openTemplate (Path.Combine(cfg.Templates.TemplatesFolder, cfg.Templates.Default))
    let setText = Bpac.setText doc
    setText "FirstName" attendee.FirstName
    setText "LastName" attendee.LastName
    setText "TicketType" (attendee.Ticket |> Option.defaultValue "")
    setText "EventDate" cfg.EventDate

    let job = sprintf "%s_%s" attendee.FirstName attendee.LastName
    if cfg.Print
        then 
            Bpac.print doc cfg.MediaName job
        else
            let folder = Path.GetDirectoryName(Path.GetFullPath(cfg.Templates.TemplatesFolder))
            Bpac.export doc (sprintf "%s\\%s.bmp" folder job)
