using System;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Threading.Tasks;

namespace THConsoleAppWcfKlient
{
    class Program
    {
        static void Main(string[] args)
        {

            var client = new Sales.SaleServiceClient();


            client.Endpoint.Address = new 
                System.ServiceModel.EndpointAddress("http://localhost:61685/SaleService.svc");
            client.Endpoint.EndpointBehaviors.Add(new HttpMessageHandlerBehavior());

            foreach (var v in client.GetAllCustomer())
            {
                Console.WriteLine(v.CustomerName);
            }

            Console.WriteLine("Hello World!");
        }


        public class HttpMessageHandlerBehavior : IEndpointBehavior
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
                bindingParameters.Add(new Func<HttpClientHandler, HttpMessageHandler>(GetHttpMessageHandler));
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }
            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }
            public void Validate(ServiceEndpoint endpoint) { }
            public HttpMessageHandler GetHttpMessageHandler(HttpClientHandler httpClientHandler)
            {
                return new InterceptingHttpMessageHandler(httpClientHandler);
            }
        }
        public class InterceptingHttpMessageHandler : DelegatingHandler
        {
            public InterceptingHttpMessageHandler(HttpMessageHandler innerHandler)
            {
                InnerHandler = innerHandler;
            }
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                //Here we could inpect and change things
                //LOG the exact outgoing message
                //request.Headers.ExpectContinue = false;
                var response = await base.SendAsync(request, cancellationToken);
                return response;
            }
        }



    }
}
