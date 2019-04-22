module Configuration

open System
open System.IO
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Configuration.Binder
open Microsoft.Extensions.Configuration.Json

[<CLIMutable>]
type TemplatesConfiguration =
    {   TemplatesFolder: string
        Default: string
        QR1: string
        QR2: string
    }
    static member DefaultConfig =
        {   TemplatesFolder = "./assets/templates/"
            Default = "template.lbx"
            QR1 = String.Empty
            QR2 = String.Empty
        }

[<CLIMutable>]
type PrintConfiguration =
    {   Templates: TemplatesConfiguration
        MediaName: string
        Print: bool
        EventBriteAccessToken: string
        ServiceBusQueue: string
    }
    static member DefaultConfig =
        {
            Templates = TemplatesConfiguration.DefaultConfig
            MediaName = "2.4\" x 3.9\""
            Print = false
            EventBriteAccessToken = String.Empty
            ServiceBusQueue = String.Empty
        }

let private cfgBuilder (file: string) =
    ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(file)
        .Build()

let cfg = PrintConfiguration.DefaultConfig
cfgBuilder("print-server.json").Bind(cfg)