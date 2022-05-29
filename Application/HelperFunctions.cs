using Models.ApiRequestModels;
using System.Drawing;
using System.Text;

namespace Application
{
    public class HelperFunctions : IHelperFunctions
    {
        public HelperFunctions() { 
            
        }
        public Bitmap Base64StringToBitmap(string base64String)
        {
            Bitmap bmpReturn;
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(byteBuffer);
            ms.Position = 0;
            bmpReturn = (Bitmap)Bitmap.FromStream(ms);
            ms.Close();
            return bmpReturn;
        }
        public void ChangeColor(ref string bitString, ref string color) // son renk pixellerini çıkarma ve istenen yazıyı yazmak
        {
            color = color.Remove(color.Length - 1);
            color += bitString[0];
            bitString = bitString.Remove(0, 1);
        }
        public string GetStringOfBinaryCode(EncodeRequest textToCode) // sonraki kodlama için bir ikili kod dizisi alma
        {
            byte[] bytes = Encoding.GetEncoding(65001).GetBytes(textToCode.Text);
            string bitString = "";
            for (int i = 0; i < 16; i++)
            {
                bitString += '0';
            }
            for (int i = 0; i < bytes.Length; i++)
            {
                bitString += Convert.ToString(bytes[i], 2).PadLeft(16, '0');
            }
            for (int i = 0; i < 16; i++)
            {
                bitString += '0';
            }

            return bitString;
        }
    }
}