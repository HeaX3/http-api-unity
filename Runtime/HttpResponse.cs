using System;
using System.Net;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace HttpApi
{
    public readonly struct HttpResponse
    {
        public readonly UnityWebRequest.Result result;
        public readonly HttpStatusCode status;
        [NotNull] private readonly byte[] data;

        public HttpResponse(UnityWebRequest.Result result, HttpStatusCode status, [NotNull] byte[] data)
        {
            this.result = result;
            this.status = status;
            this.data = data;
        }

        [NotNull]
        public byte[] bytes()
        {
            return data;
        }

        [NotNull]
        public string text()
        {
            return Encoding.UTF8.GetString(data);
        }

        [CanBeNull]
        public JObject json()
        {
            try
            {
                return JObject.Parse(text());
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [CanBeNull]
        public JArray jsonArray()
        {
            try
            {
                return JArray.Parse(text());
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static HttpResponse Response(UnityWebRequest request)
        {
            return new HttpResponse(
                request.result,
                (HttpStatusCode)request.responseCode,
                request.downloadHandler != null ? request.downloadHandler.data : Array.Empty<byte>()
            );
        }
    }
}