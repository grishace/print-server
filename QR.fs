module QR

open System.Drawing
open System.Drawing.Imaging
open System.Drawing.Drawing2D
open System.IO
open QRCoder
open Types

let private vCard = sprintf """
BEGIN:VCARD
VERSION:4.0
N:%s;%s
FN:%s
URL;TWITTER:%s
URL;LINKEDIN:%s
END:VCARD
"""
let [<Literal>] private QrSize = 330

let generate (payload: Attendee) =
    let qr, ico =
        match payload with
        | { Twitter = Some t; LinkedIn = None } -> string (PayloadGenerator.Url(t)), @"assets\twitter.bmp"
        | { Twitter = None; LinkedIn = Some l } -> string (PayloadGenerator.Url(l)), @"assets\linkedin.bmp"
        | { FirstName = f; LastName = n; Twitter = Some t; LinkedIn = Some l } -> vCard n f (f + " " + n) t l, @"assets\twitter-linkedin.bmp"
        | _ -> failwith "One or both accounts must be passed as a payload"

    use qrGenerator = new QRCodeGenerator()
    let qrCodeData = qrGenerator.CreateQrCode(qr, QRCodeGenerator.ECCLevel.H)
    use qrCode = new QRCode(qrCodeData)
    use ico = new Bitmap(ico)
    let qrCodeAsBitmap = qrCode.GetGraphic(10, Color.Black, Color.White, ico, 25, 30, false)
    let fileName = Path.GetTempFileName()

    let scale = min (float QrSize / float qrCodeAsBitmap.Width) (float QrSize / float qrCodeAsBitmap.Height)
    use bmp = new Bitmap(QrSize, QrSize);
    let graph = Graphics.FromImage(bmp);
    graph.InterpolationMode <- InterpolationMode.NearestNeighbor
    let scaleWidth = int (float qrCodeAsBitmap.Width * scale)
    let scaleHeight = int (float qrCodeAsBitmap.Height * scale)
    graph.DrawImage(qrCodeAsBitmap, (QrSize - scaleWidth)/2, (QrSize - scaleHeight)/2, scaleWidth, scaleHeight)

    bmp.Save(fileName, ImageFormat.Bmp)
    Some fileName
