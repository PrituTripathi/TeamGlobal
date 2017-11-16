using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace TeamGlobal.Infrastructure.Services
//{
//    /// <summary>
//    /// This class exposes RESTful CRUD functionality in a generic way, abstracting
//    /// the implementation and useage details of HttpClient, HttpRequestMessage,
//    /// HttpResponseMessage, ObjectContent, Formatters etc.
//    /// </summary>
//    /// <typeparam name="T">This is the Type of Resource you want to work with, such as Customer, Order etc.</typeparam>
//    /// <typeparam name="TResourceIdentifier">This is the type of the identifier that uniquely identifies a specific resource such as Id or Username etc.</typeparam>
//    public class GenericRestfulCrudHttpClient<T, TResourceIdentifier> : IDisposable where T : class
//    {
//        private bool disposed = false;
//        private HttpClient httpClient;
//        protected readonly string serviceBaseAddress;
//        private readonly string addressSuffix;
//        private readonly string jsonMediaType = "application/json";

//        /// <summary>
//        /// The constructor requires two parameters that essentially initialize the underlying HttpClient.
//        /// In a RESTful service, you might have URLs of the following nature (for a given resource - Member in this example):<para />
//        /// 1. http://www.somedomain/api/members/<para />
//        /// 2. http://www.somedomain/api/members/jdoe<para />
//        /// Where the first URL will GET you all members, and allow you to POST new members.<para />
//        /// While the second URL supports PUT and DELETE operations on a specifc member.
//        /// </summary>
//        /// <param name="serviceBaseAddress">As per the example, this would be "http://www.somedomain"</param>
//        /// <param name="addressSuffix">As per the example, this would be "api/members/"</param>

//        public GenericRestfulCrudHttpClient(string serviceBaseAddress, string addressSuffix)
//        {
//            this.serviceBaseAddress = serviceBaseAddress;
//            this.addressSuffix = addressSuffix;
//            httpClient = MakeHttpClient(serviceBaseAddress);
//        }

//        protected virtual HttpClient MakeHttpClient(string serviceBaseAddress)
//        {
//            httpClient = new HttpClient();
//            httpClient.BaseAddress = new Uri(serviceBaseAddress);
//            httpClient.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(jsonMediaType));
//            //httpClient.DefaultRequestHeaders.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("gzip"));
//            //httpClient.DefaultRequestHeaders.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("defalte"));
//            //httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("Matlus_HttpClient", "1.0")));
//            return httpClient;
//        }

//        public async Task<IEnumerable<T>> GetManyAsync()
//        {
//            var responseMessage = await httpClient.GetAsync(addressSuffix);
//            responseMessage.EnsureSuccessStatusCode();
//            return await responseMessage.Content.ReadAsStringAsync<IEnumerable<T>>();
//        }

//        public async Task<string> GetAsync(TResourceIdentifier identifier)
//        {
//            var responseMessage = await httpClient.GetAsync(addressSuffix + identifier.ToString());
//            responseMessage.EnsureSuccessStatusCode();
//            return await responseMessage.Content.ReadAsStringAsync();
//        }

//        public async Task<T> PostAsync(T model)
//        {
//            var requestMessage = new HttpRequestMessage();
//            var objectContent = CreateJsonObjectContent(model);
//            var responseMessage = await httpClient.PostAsync(addressSuffix, objectContent);
//            return await responseMessage.Content.ReadAsAsync<T>();
//        }

//        public async Task PutAsync(TResourceIdentifier identifier, T model)
//        {
//            var requestMessage = new HttpRequestMessage();
//            var objectContent = CreateJsonObjectContent(model);
//            var responseMessage = await httpClient.PutAsync(addressSuffix + identifier.ToString(), objectContent);
//        }

//        public async Task DeleteAsync(TResourceIdentifier identifier)
//        {
//            var r = await httpClient.DeleteAsync(addressSuffix + identifier.ToString());
//        }

//        private ObjectContent CreateJsonObjectContent(T model)
//        {
//            var requestMessage = new HttpRequestMessage();
//            return requestMessage.CreateContent<T>(
//                model,
//                MediaTypeHeaderValue.Parse(jsonMediaType),
//                new MediaTypeFormatter[] { new JsonMediaTypeFormatter() },
//                new FormatterSelector());
//        }

//        #region IDisposable Members

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        private void Dispose(bool disposing)
//        {
//            if (!disposed && disposing)
//            {
//                if (httpClient != null)
//                {
//                    var hc = httpClient;
//                    httpClient = null;
//                    hc.Dispose();
//                }
//                disposed = true;
//            }
//        }

//        #endregion IDisposable Members
//    }

{
    public class RestHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string FormDate { get; set; }
        public string ToDate { get; set; }

        public string FormSource { get; set; }
        public string ToDestination { get; set; }

        public RestHelper(string from, string to, string source, string destination)
        {
            FormDate = from;
            ToDate = to;
            FormSource = source;
            ToDestination = destination;
        }

        public async Task<T> GetObjects<T>() where T : class, new()
        {
            log.Debug("start GetObjects");
            if (string.IsNullOrEmpty(ToDate))
            {
                new NullReferenceException("To Date can not be null. ");
            }

            if (string.IsNullOrEmpty(FormDate))
            {
                new NullReferenceException("Form Date can not be null. ");
            }

            var client = new HttpClient();
            Uri uri = new Uri(string.Format(WebConfigurationManager.AppSettings["WebApi"], FormDate, ToDate, FormSource, ToDestination));
            client.MaxResponseContentBufferSize = int.MaxValue;
            var returnobj = new T();
            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    returnobj = JsonConvert.DeserializeObject<T>(content);
                }
            }
            catch(Exception exception)
            {
                log.Debug(exception.Message);
                log.Debug(exception.StackTrace);
                return null;
            }
            log.Debug("complete GetObjects");
            return returnobj;
        }
    }
}