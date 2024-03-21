using System;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ChatApplicationServerHttp
{
	public class WriteResponse
	{
        public static async Task WriteJsonResponse(HttpListenerContext context, object responseObject)
        {
            string jsonResponse = JsonSerializer.Serialize(responseObject);
            byte[] responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentLength64 = responseBytes.Length;

            Cookie cookie = new("coo", "cook");
            context.Response.Cookies.Add(cookie);

            await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            context.Response.Close();
        }
    }
}

