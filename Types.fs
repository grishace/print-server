module Types

open System.Runtime.Serialization

[<CLIMutable>]
type Answer =
    {   [<DataMember(Name = "question")>]
        Question: string
        [<DataMember(Name = "type")>]
        Type: string
        [<DataMember(Name = "answer")>]
        Answer: string
    }

[<CLIMutable>]
type Profile =
    {   [<DataMember(Name = "first_name")>]
        FirstName: string
        [<DataMember(Name = "last_name")>]
        LastName: string
        [<DataMember(Name = "email")>]
        Email: string
    }

[<CLIMutable>]
type EventBriteApiResponse =
    {   [<DataMember(Name = "profile")>]
        Profile: Profile
        [<DataMember(Name = "ticket_class_name")>]
        TicketClass: string
        [<DataMember(Name = "answers")>]
        Answers: Answer array
    }

type Attendee =
    {   FirstName: string
        LastName: string
        Ticket: string option
        LinkedIn: string option
        Twitter: string option
    }
