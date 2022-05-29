using Models.ApiRequestModels;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Application
{
    public class WebSocketFunctions : IWebSocketFunctions
    {
        private static HelperFunctions hf;
        public static WebSocketServer wssv;
        public WebSocketFunctions(HelperFunctions helperFunctions) { 
            hf = helperFunctions;
            wssv = WebSocketServerCreate("192.168.43.11",7286);
        }
        public WebSocketServer WebSocketServerCreate(string url, int port)
        {
            if(wssv == null) { 
                wssv = new WebSocketServer(System.Net.IPAddress.Parse(url), port);
            }
            return wssv;
        }
        public WebSocketServer getWssv() { 
            return wssv;
        }
        public void WebSocketServicesCreate(WebSocketServer wssv)
        {
            wssv.AddWebSocketService<Encode>("/Encode");
            wssv.AddWebSocketService<Decode>("/Decode");
        }
        public class Encode : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                EncodeRequest request = JsonConvert.DeserializeObject<EncodeRequest>(e.Data);
                Bitmap Bitmap = new Bitmap(hf.Base64StringToBitmap(request.Picture));
                string bitString = hf.GetStringOfBinaryCode(request);
                bool is_brake = false;
                for (int x = 0; x < Bitmap.Width; x++)
                {
                    for (int y = 0; y < Bitmap.Height; y++)
                    {
                        Color pixelColor = Bitmap.GetPixel(x, y);
                        // son biti yazıyla değiştiren yöntem
                        var R = Convert.ToString(pixelColor.R, 2).PadLeft(8, '0');
                        var G = Convert.ToString(pixelColor.G, 2).PadLeft(8, '0');
                        var B = Convert.ToString(pixelColor.B, 2).PadLeft(8, '0');
                        if (bitString.Length == 0)
                        {
                            is_brake = true;
                            break;
                        }
                        hf.ChangeColor(ref bitString, ref R);

                        if (bitString.Length == 0)
                        {
                            is_brake = true;
                            break;
                        }
                        hf.ChangeColor(ref bitString, ref G);

                        if (bitString.Length == 0)
                        {
                            is_brake = true;
                            break;
                        }
                        hf.ChangeColor(ref bitString, ref B);

                        Bitmap.SetPixel(x, y, Color.FromArgb(Convert.ToInt32(R, 2), Convert.ToInt32(G, 2), Convert.ToInt32(B, 2)));
                    }
                    if (is_brake)
                        break;
                }
                Console.WriteLine(request);
                string str = "";
                using (MemoryStream ms = new MemoryStream())
                {
                    Bitmap.Save(ms, ImageFormat.Png);
                    str = Convert.ToBase64String(ms.ToArray());
                }
                wssv.WebSocketServices["/Encode"].Sessions.Broadcast(str);
                using (var fs = new FileStream("mert.png", FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        Bitmap.Save(fs, ImageFormat.Png);
                    }
                    catch
                    {
                    }
                }
            }
        }
        public class Decode : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                DecodeRequest request = JsonConvert.DeserializeObject<DecodeRequest>(e.Data);
                Bitmap Bitmap = new Bitmap(hf.Base64StringToBitmap(request.Picture));
                bool startReading = false;
                string decodeString = "";
                string decodePix = "";
                bool isBreak = false;
                for (int x = 0; x < Bitmap.Width; x++)
                {
                    for (int y = 0; y < Bitmap.Height; y++)
                    {
                        Color pixelColor = Bitmap.GetPixel(x, y);
                        var R = Convert.ToString(pixelColor.R, 2).PadLeft(8, '0');
                        var G = Convert.ToString(pixelColor.G, 2).PadLeft(8, '0');
                        var B = Convert.ToString(pixelColor.B, 2).PadLeft(8, '0');
                        /*-----------------------------------------------------------------------------------------------*/
                        if (decodePix.Length != 16) // burada kodu çözülen karakter dizisine son renk bitini ekliyoruz decodePix tek karakterdir
                        {
                            decodePix += R[7];
                        }
                        else if (decodePix == "0000000000000000") // eğer bu sonsa, yazı bitmiştir.
                        {
                            if (startReading == false)
                            {
                                startReading = true;
                                decodePix = "";
                                decodePix += R[7];
                            }
                            else
                            {
                                isBreak = true;
                                break;
                            }
                        }
                        else // dizi doluysa, dizinin son elemanına (decodeString) yazmak ve diziyi bir karakter için temizlemek için (decodePix)
                        {
                            if (startReading)
                            {
                                decodeString += decodePix;
                                decodePix = "";
                                decodePix += R[7];
                            }
                        }
                        /*-----------------------------------------------------------------------------------------------*/
                        if (decodePix.Length != 16)
                        {
                            decodePix += G[7];
                        }
                        else if (decodePix == "0000000000000000")
                        {
                            if (startReading == false)
                            {
                                startReading = true;
                                decodePix = "";
                                decodePix += G[7];
                            }
                            else
                            {
                                isBreak = true;
                                break;
                            }
                        }
                        else
                        {
                            if (startReading)
                            {
                                decodeString += decodePix;
                                decodePix = "";
                                decodePix += G[7];
                            }
                        }
                        /*-----------------------------------------------------------------------------------------------*/
                        if (decodePix.Length != 16)
                        {
                            decodePix += B[7];
                        }
                        else if (decodePix == "0000000000000000")
                        {
                            if (startReading == false)
                            {
                                startReading = true;
                                decodePix = "";
                                decodePix += B[7];
                            }
                            else
                            {
                                isBreak = true;
                                break;
                            }
                        }
                        else
                        {
                            if (startReading)
                            {
                                decodeString += decodePix;
                                decodePix = "";
                                decodePix += B[7];
                            }
                        }
                    }
                    if (isBreak)
                        break;
                }
                int deStr_Size = decodeString.Length / 16;
                byte[] bytes = new byte[deStr_Size];
                for (int i = 0; i < deStr_Size; i++) // 16 bit ikili kodu bayt gösterimine çevirmek için olan döngü
                {
                    var one_byte = decodeString.Substring(0, 16);
                    var integerByte = Convert.ToInt32(one_byte, 2);
                    bytes[i] = (byte)integerByte;
                    decodeString = decodeString.Substring(16);
                }
                decodeString = Encoding.GetEncoding(65001).GetString(bytes); // baytlardan metin karakterlerine dönüştürmek
                wssv.WebSocketServices["/Decode"].Sessions.Broadcast(decodeString);
            }
        }
    }
}