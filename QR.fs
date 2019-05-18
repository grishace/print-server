module QR

open System.Drawing
open System.Drawing.Imaging
open System.Drawing.Drawing2D
open System.IO
open QRCoder

let [<Literal>] private QrSize = 330

let generate (ico: string) (link: string option) =
    let generate' link' =
        let generator = PayloadGenerator.Url(link')
        let payload = string generator
        use qrGenerator = new QRCodeGenerator()
        let qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.H)
        use qrCode = new QRCode(qrCodeData)
        use ico = new Bitmap(ico)
        let qrCodeAsBitmap = qrCode.GetGraphic(10, Color.Black, Color.White, ico, 20, 25, false)
        let fileName = Path.GetTempFileName()

        let scale = float QrSize / float qrCodeAsBitmap.Width
        use bmp = new Bitmap(QrSize, QrSize);
        let graph = Graphics.FromImage(bmp);
        graph.InterpolationMode <- InterpolationMode.HighQualityBilinear
        let scaledSize = int (float qrCodeAsBitmap.Width * scale)
        let offset = (QrSize - scaledSize) / 2
        graph.DrawImage(qrCodeAsBitmap, offset, offset, scaledSize, scaledSize)

        bmp.Save(fileName, ImageFormat.Bmp)
        fileName

    link |> Option.map generate'
