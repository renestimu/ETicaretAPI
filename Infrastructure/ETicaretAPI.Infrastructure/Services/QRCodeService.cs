using ETicaretAPI.Application.Abstractions.Services;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services
{
    public class QRCodeService : IQRCodeService
    {
        public QRCodeService()
        {
           
        }

        public byte[] GenerateQRCode(string content)
        {
            QRCodeGenerator qRCode = new();
            QRCodeData data = qRCode.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode pngByteQRCode = new(data);
            byte[] bytes = pngByteQRCode.GetGraphic(10, System.Drawing.Color.Black, System.Drawing.Color.White, true);
            return bytes;
        }
    }
}
