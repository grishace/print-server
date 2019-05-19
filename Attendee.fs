module Attendee

open System.Net.Http
open System.Text
open System.Text.RegularExpressions
open Utf8Json

open Types
open Configuration

let getTicketType className =
    let pattern (patt: string) (str: string) = if str.Contains patt then Some patt else None
    let (|SPONSOR|_|) = pattern "SPONSOR"
    let (|VOLUNTEER|_|) = pattern "VOLUNTEER"
    let (|SPEAKER|_|) = pattern "SPEAKER"
    match className with
    | SPONSOR p | VOLUNTEER p | SPEAKER p -> Some p
    | _ -> None

let getSocialNetwork (sn: string) fn answers =
    let sl = sn.ToLower()
    let byType t a =
        let answerType t a = a.Type = t
        let andQuestion a = a.Question.ToLower().Contains(sl)
        answerType t a && andQuestion a
    let yes =
        answers
        |> Array.filter (byType "multiple_choice")
        |> Array.tryHead
        |> Option.map (fun a -> a.Answer.ToLower() = "yes" )
        |> Option.defaultValue false
    if yes
        then
            answers
            |> Array.filter (byType "text")
            |> Array.tryHead
            |> Option.map (fun a -> fn a.Answer)
            |> Option.flatten
        else None

let private commonRx =  sprintf "^(?:https?://)?(?:(?:www\.)?%s)?%s(?<account>[^/]+)(?:.*)$"
let private rxOptions = RegexOptions.Compiled ||| RegexOptions.IgnoreCase
let private rxTwitter = Regex(commonRx "twitter\.com/" "@?", rxOptions)
let private rxLinkedIn = Regex(commonRx "linkedin\.com/in/" "", rxOptions)

let private validate (rx: Regex) a =
    let match'= rx.Match(a)
    match'.Groups.["account"] |> Option.ofObj |> Option.map (fun v -> v.Value)

let getAttendee (api: string) =
    async {
        use http = new HttpClient()
        http.DefaultRequestHeaders.Add("Authorization", (sprintf "Bearer %s" cfg.EventBriteAccessToken))
        let! res' = http.GetAsync(api) |> Async.AwaitTask
        let! res'' = res'.Content.ReadAsByteArrayAsync() |> Async.AwaitTask
        let json = Encoding.UTF8.GetString(res'')
        let data = JsonSerializer.Deserialize<EventBriteApiResponse> json

        let liProfile = getSocialNetwork "LinkedIn" (validate rxLinkedIn) data.Answers
        let twHandle = getSocialNetwork "Twitter" (validate rxTwitter) data.Answers

        return {
            FirstName = data.Profile.FirstName
            LastName = data.Profile.LastName
            Ticket = getTicketType data.TicketClass
            LinkedIn = liProfile |> Option.map (sprintf "https://www.linkedin.com/in/%s/")
            Twitter = twHandle |> Option.map (sprintf "https://twitter.com/intent/follow?screen_name=%s")
        }
    }
