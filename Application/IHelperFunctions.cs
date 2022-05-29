using Models;
using Models.ApiRequestModels;
using System.Drawing;

namespace Application
{
    public interface IHelperFunctions
    {
        public Bitmap Base64StringToBitmap(string base64String);
        public void ChangeColor(ref string bitString, ref string color);
        public string GetStringOfBinaryCode(EncodeRequest textToCode);
    }
}
