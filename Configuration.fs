module Configuration

open System
open System.IO
open Microsoft.Extensions.Configuration

[<CLIMutable>]
type TemplatesConfiguration =
    {   TemplatesFolder: string
        Default: string
    }
    static member DefaultConfig =
        {   TemplatesFolder = "./assets/templates/"
            Default = "template.lbx"
        }

[<CLIMutable>]
type PrintConfiguration =
    {   Templates: TemplatesConfiguration
        MediaName: string
        Print: bool
        EventDate: string
        EventBriteAccessToken: string
        ServiceBusConnection: string
        ServiceBusQueue: string
    }
    static member DefaultConfig =
        {
            Templates = TemplatesConfiguration.DefaultConfig
            MediaName = "2.4\" x 3.9\""
            Print = false
            EventDate = String.Empty
            EventBriteAccessToken = String.Empty
            ServiceBusConnection = String.Empty
            ServiceBusQueue = String.Empty
        }

let private cfgBuilder (file: string) =
    ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile(file)
        .Build()

let cfg = PrintConfiguration.DefaultConfig
cfgBuilder("print-server.json").Bind(cfg)