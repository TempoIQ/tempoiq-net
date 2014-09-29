using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace TempoIQTest
{
    public class TempoIQTestClass
    {
        public JsonDeserializer In { get; set; }
        public JsonSerializer Out { get; set; }

        public RestResponse fromString(string content)
        {
            var resp = new RestResponse();
            resp.Content = content;
            return resp;
        }

        public void Init()
        {
            this.In = new JsonDeserializer();
            this.Out = new JsonSerializer();
        }
    }
}
