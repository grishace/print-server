module Types

open System.Text.Json.Serialization

[<CLIMutable>]
type Profile =
    {
        [<JsonPropertyName("first_name")>]
        FirstName: string
        [<JsonPropertyName("last_name")>]
        LastName: string
        [<JsonPropertyName("email")>]
        Email: string
    }

[<CLIMutable>]
type EventBriteApiResponse =
    {
        [<JsonPropertyName("profile")>]
        Profile: Profile
        [<JsonPropertyName("ticket_class_name")>]
        TicketClass: string
    }

type Attendee =
    {
        FirstName: string
        LastName: string
        Ticket: string option
    }
