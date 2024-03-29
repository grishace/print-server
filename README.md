### EventBrite integration with F#

The [Denver Dev Day](https://denverdevday.github.io/) organization committee was using an event-hosting website for 3 years before the company behind it was sold and their community license was revoked. Because the event is organized by the community, there was no budget to pay for the commercial license, so organizers decided to move on to use free services for the event.

However, there was one feature missing. Printing the attendee's badge right away -automagically- after scanning the attendee's ticket adds a professional touch and streamlines the check-in process.

The purpose of this project is to fill that gap.

[![EventBrite integration demo](http://img.youtube.com/vi/YbJaJDEbzdk/0.jpg)](http://www.youtube.com/watch?v=YbJaJDEbzdk "EventBrite integration demo")

### F# Challenges

* COM-interop to use b-PAC SDK
* Generate QR code
* Invoke EventBrite API
* Subscribe to Azure message queue

### Prerequisites

* [Brother QL-700](https://www.brother-usa.com/products/QL700) Label Printer
* [Brother DK-1202](https://www.brother-usa.com/products/DK1202) Labels
* [Brother b-PAC SDK](https://www.brother.co.jp/eng/dev/bpac/download/index.aspx)
* [Brother P-touch](https://support.brother.com/g/b/downloadend.aspx?c=us&lang=en&prod=lpql700eus&os=10011&dlid=dlfp100377_000&flang=178&type3=296) Label Designer
* [EventBrite](https://www.eventbrite.com) account
* [Azure](https://portal.azure.com) account

### Build

* Unfortunately b-PAC SDK doesn't work with 64-bit .NET Core (or at least I didn't find how to make it work),
but 32-bit version works perfectly.
* ~~.NET Core 3.0 is supposed to bring COM interop to the .NET Core world, but the path described [here](https://github.com/dotnet/samples/tree/master/core/extensions/ExcelDemo) didn't work either, but b-PAC COM reference is
there in the project file (just commented out) for you to try.~~  COM interop assembly works after migrating project to the .NET 6.0
* `dotnet build`

### Configure

* Log into your Azure subscription.
* Create Service Bus Namespace and a message queue.
* Deploy Logic App using `eb-integration.logicapp.json`. Azure portal doesn't save connection information in the
action block, therefore create a Send Message action using the service bus and selection the message queue
from the drop-down list.
* Add `Content` parameter, set it to dynamic content, start typing `api_url` in the search box, but select
it from the found items below. Don't forget to save your logic app!
![LogicApp Designer](https://github.com/grishace/print-server/blob/master/images/logicapp-designer.png)
* Log into your EventBrite account and go to the Account Settings.
* Create WebHook for `barcode.checked_in` action. Use URL of the Logic App you've just deployed.
* Press `Test` button.
![EventBrite WebHook](https://github.com/grishace/print-server/blob/master/images/eventbrite-webhook.png)
* Create a new app registration under App Management.
* Copy your personal OAuth token into `print-server.json` `EventBriteAccessToken` parameter.
![EventBrite App Management](https://github.com/grishace/print-server/blob/master/images/eventbrite-app.png)
* Copy queue connection string into the `ServiceBusQueue` parameter.
* Design your badge with the P-touch editor and save it into the `./assets/templates/` folder.
![P-touch](https://github.com/grishace/print-server/blob/master/images/p-touch.png)

### Run

* `dotnet run --no-build`
* Or `dotnet publish` navigate to the publish folder and run `print-server.exe`
