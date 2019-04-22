module QR

open System.Drawing
open System.Drawing.Imaging
open System.Drawing.Drawing2D
open System.IO
open QRCoder

let generate (ico: string) (link: string option) =
    let generate' link' =
        let generator = PayloadGenerator.Url(link')
        let payload = string generator
        use qrGenerator = new QRCodeGenerator()
        let qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.H)
        use qrCode = new QRCode(qrCodeData)
        use ico = new Bitmap(ico)
        let qrCodeAsBitmap = qrCode.GetGraphic(10, Color.Black, Color.White, ico, 25, 30, false)
        let fileName = Path.GetTempFileName()

        let scale = min (330.0 / float qrCodeAsBitmap.Width) (330.0 / float qrCodeAsBitmap.Height)
        use bmp = new Bitmap(330, 330);
        let graph = Graphics.FromImage(bmp);
        graph.InterpolationMode <- InterpolationMode.NearestNeighbor
        let scaleWidth = int (float qrCodeAsBitmap.Width * scale)
        let scaleHeight = int (float qrCodeAsBitmap.Height * scale)
        graph.DrawImage(qrCodeAsBitmap, (330 - scaleWidth)/2, (330 - scaleHeight)/2, scaleWidth, scaleHeight)

        bmp.Save(fileName, ImageFormat.Bmp)
        fileName

    link |> Option.map generate'
