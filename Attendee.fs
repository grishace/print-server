module Attendee

open System.Net.Http
open System.Text
open System.Text.Json

open Types
open Configuration

let http = new HttpClient()
http.DefaultRequestHeaders.Add("Authorization", (sprintf "Bearer %s" cfg.EventBriteAccessToken))

let getTicketType className =
    let pattern (patt: string) (str: string) = if str.Contains patt then Some patt else None
    let (|SPONSOR|_|) = pattern "SPONSOR"
    let (|VOLUNTEER|_|) = pattern "VOLUNTEER"
    let (|SPEAKER|_|) = pattern "SPEAKER"
    match className with
    | SPONSOR p | VOLUNTEER p | SPEAKER p -> Some p
    | _ -> None

let getAttendee (api: string) =
    task {
        let! res' = http.GetAsync(api)
        let! res'' = res'.Content.ReadAsByteArrayAsync()
        let json = Encoding.UTF8.GetString(res'')
        let data = JsonSerializer.Deserialize<EventBriteApiResponse> json

        return {
            FirstName = data.Profile.FirstName
            LastName = data.Profile.LastName
            Ticket = getTicketType data.TicketClass
        }
    }
