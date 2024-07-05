[<RequireQualifiedAccess>]
module Bpac

open bpac

let openTemplate lbx =
    let doc = DocumentClass()
    doc, doc.Open(lbx)

let private getObject (doc: DocumentClass) object = doc.GetObject(object)

let setText doc object text =
    (getObject doc object).Text <- text

let export (doc: DocumentClass) path = doc.Export(ExportType.bexBmp, path, 600)

let print (doc: DocumentClass) media title =
    doc.SetMediaByName(media, false) &&
    doc.StartPrint(title, PrintOptionConstants.bpoHighSpeed + PrintOptionConstants.bpoHighResolution + PrintOptionConstants.bpoAutoCut) &&
    doc.PrintOut(1, PrintOptionConstants.bpoDefault) &&
    doc.EndPrint()
