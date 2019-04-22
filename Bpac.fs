[<RequireQualifiedAccess>]
module Bpac

open System
open System.Reflection

let [<Literal>] private MemberAccess = BindingFlags.Public ||| BindingFlags.Instance
let [<Literal>] private SetProperty = MemberAccess ||| BindingFlags.SetProperty
let [<Literal>] private InvokeMethod = MemberAccess ||| BindingFlags.InvokeMethod

let private invokeMethod inst method param =
    let t = inst.GetType()
    t.InvokeMember(method, InvokeMethod, null, inst, param)

let private setProperty inst prop value =
    inst.GetType().InvokeMember(prop, SetProperty, null, inst, [| value |]) |> ignore

let openTemplate lbx =
    let doc = Activator.CreateInstance(Type.GetTypeFromProgID("bpac.Document", true))
    doc, invokeMethod doc "Open" [| lbx |]

let private getObject doc object = invokeMethod doc "GetObject" [| object |]

let setText doc object text =
    setProperty (getObject doc object) "Text" text

let setImage doc object image =
    invokeMethod (getObject doc object) "SetData" [| 0; image; 4 |] |> ignore

let export doc path =
    invokeMethod doc "Export" [| 4; path; 600 |] |> ignore

let print doc media title =
    let invoke = fun meth param -> invokeMethod doc meth param |> ignore
    invoke "SetMediaByName" [| media; false |]
    invoke "StartPrint" [| title; 0 |]
    invoke "PrintOut" [| 1; 0x00010000 |]
    invoke "EndPrint" [||]
