using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ApiRequestModels
{
    public class DecodeRequest
    {
        public string Picture { get; set; }
        public DecodeRequest(string picture)
        {
            this.Picture = picture;
        }
    }
}
