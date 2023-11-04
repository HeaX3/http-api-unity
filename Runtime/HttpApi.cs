using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using RSG;
using UnityEngine;
using UnityEngine.Networking;

namespace HttpApis
{
    /// <summary>
    /// To create a new API client, inherit this class, create a DontDestroyOnLoad(gameObject) instance carrying a
    /// component of your class, and then start interacting with it from your code
    /// </summary>
    public abstract class HttpApi : MonoBehaviour
    {
        public string endpoint { get; set; }
        public readonly Dictionary<string, string> defaultHeaders = new();

        #region Utility Methods

        /// <summary>
        /// Shorthand to write the AuthenticationHeader header value in the <see cref="defaultHeaders"/> dictionary
        /// </summary>
        public void SetAuthenticationHeader(string token)
        {
            defaultHeaders["AuthenticationHeader"] = token;
        }

        /// <summary>
        /// Shorthand to write the Authorization header value in the <see cref="defaultHeaders"/> dictionary
        /// </summary>
        public void SetAuthorization(string token)
        {
            defaultHeaders["Authorization"] = token;
        }

        #endregion

        #region HTTP Methods

        /// <summary>
        /// Perform a HTTP GET request
        /// </summary>
        protected IPromise<HttpResponse> Get(string url)
        {
            return new Promise<HttpResponse>((resolve, reject) =>
            {
                var www = UnityWebRequest.Get(BuildUrl(url));
                AddHeaders(www);
                Perform(www).Then(result => resolve(HttpResponse.Response(result))).Catch(reject);
            });
        }

        /// <summary>
        /// Perform a HTTP POST request
        /// </summary>
        protected IPromise<HttpResponse> Post(string url, JToken body) => Post(url, body?.ToString());

        /// <summary>
        /// Perform a HTTP POST request
        /// </summary>
        protected IPromise<HttpResponse> Post(string url, string body = null)
        {
            return new Promise<HttpResponse>((resolve, reject) =>
            {
                var www = new UnityWebRequest(BuildUrl(url), "POST");
                AddHeaders(www);
                www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body ?? "{}"));
                www.uploadHandler.contentType = "application/json";

                www.downloadHandler = new DownloadHandlerBuffer();
                Perform(www).Then(result => resolve(HttpResponse.Response(result))).Catch(reject);
            });
        }

        /// <summary>
        /// Perform a HTTP PUT request
        /// </summary>
        protected IPromise<HttpResponse> Put(string url, JToken body) => Put(url, body?.ToString());

        /// <summary>
        /// Perform a HTTP PUT request
        /// </summary>
        protected IPromise<HttpResponse> Put(string url, string body = null)
        {
            return new Promise<HttpResponse>((resolve, reject) =>
            {
                var www = new UnityWebRequest(BuildUrl(url), "PUT");
                AddHeaders(www);
                www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body ?? "{}"));
                www.uploadHandler.contentType = "application/json";

                www.downloadHandler = new DownloadHandlerBuffer();
                Perform(www).Then(result => resolve(HttpResponse.Response(result))).Catch(reject);
            });
        }

        /// <summary>
        /// Perform a HTTP PATCH request
        /// </summary>
        protected IPromise<HttpResponse> Patch(string url, JToken body) => Patch(url, body?.ToString());

        /// <summary>
        /// Perform a HTTP PATCH request
        /// </summary>
        protected IPromise<HttpResponse> Patch(string url, string body = null)
        {
            return new Promise<HttpResponse>((resolve, reject) =>
            {
                var www = new UnityWebRequest(BuildUrl(url), "PATCH");
                AddHeaders(www);
                www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body ?? "{}"));
                www.uploadHandler.contentType = "application/json";

                www.downloadHandler = new DownloadHandlerBuffer();
                Perform(www).Then(result => resolve(HttpResponse.Response(result))).Catch(reject);
            });
        }

        /// <summary>
        /// Perform a HTTP DELETE request
        /// </summary>
        protected IPromise Delete(string url)
        {
            return new Promise((resolve, reject) =>
            {
                var www = UnityWebRequest.Delete(BuildUrl(url));
                AddHeaders(www);
                Perform(www).Then(_ => resolve()).Catch(reject);
            });
        }

        #endregion

        #region Helpers

        private IPromise<UnityWebRequest> Perform(UnityWebRequest request)
        {
            return new Promise<UnityWebRequest>((resolve, reject) =>
            {
                StartCoroutine(PerformRoutine(request, () =>
                {
                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        resolve(request);
                    }
                    else
                    {
                        reject(new Exception(request.responseCode + ": " + request.error + "\n" +
                                             request.method + " " + request.url + "\n" +
                                             (request.uploadHandler?.data != null
                                                 ? Encoding.UTF8.GetString(request.uploadHandler.data)
                                                 : "no uploaded data") + "\n" +
                                             (request.downloadHandler?.data != null
                                                 ? Encoding.UTF8.GetString(request.downloadHandler.data)
                                                 : "no downloaded data")
                        ));
                    }
                }));
            });
        }

        private static IEnumerator PerformRoutine(UnityWebRequest request, Action callback)
        {
            using (request)
            {
                yield return request.SendWebRequest();
                try
                {
                    callback();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                finally
                {
                    request.Dispose();
                }
            }
        }

        private void AddHeaders(UnityWebRequest request)
        {
            request.SetRequestHeader("Accept", "application/json");

            foreach (var entry in defaultHeaders)
            {
                request.SetRequestHeader(entry.Key, entry.Value);
            }
        }

        public static JObject Parse(string text)
        {
            try
            {
                return JObject.Parse(text);
            }
            catch
            {
                return new JObject();
            }
        }

        public static JArray ParseArray(string text)
        {
            try
            {
                return JArray.Parse(text);
            }
            catch
            {
                return new JArray();
            }
        }

        private string BuildUrl(string path)
        {
            if (path.StartsWith("http")) return path;
            var result = endpoint + path;
            return result;
        }

        #endregion
    }
}