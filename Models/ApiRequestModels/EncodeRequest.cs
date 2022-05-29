using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ApiRequestModels
{
    public class EncodeRequest
    {
        public string Picture { get; set; }
        public string Text { get; set; }
        public EncodeRequest(string picture, string text)
        {
            this.Picture = picture;
            this.Text = text;
        }
    }
}
