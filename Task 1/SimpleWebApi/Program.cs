using System;
using System.IO;
using System.Net;
using System.Text;

namespace SimpleWebApi
{
    class Program
    {
        static HttpListener _server = new HttpListener();
        
        static void Main(string[] args)
        {
            string uri = @"http://*:8080/health/";

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            try
            {
                StartServer(uri);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            
            Console.WriteLine("Application stopped.");
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _server.Stop();
            _server.Abort();
            _server.Close();
            Console.WriteLine("Сервер остановлен!");
            e.Cancel = true;
        }

        private static void StartServer(string prefix)
        {
            if (!HttpListener.IsSupported)
                return;

            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentException("prefix");
            
            Console.WriteLine("Starting for prefix = {0}", prefix);
            _server.Prefixes.Add(prefix);
           
            _server.Start();
            Console.WriteLine("Service started!");

            while (_server.IsListening)
            {
                var context = _server.BeginGetContext(ListenerCallback, _server);
                
                bool success = context.AsyncWaitHandle.WaitOne(5000, true);

                //получаем входящий запрос
                //HttpListenerRequest request = context.Request;
                //обрабатываем POST запрос
                //запрос получен методом POST (пришли данные формы)
//                 if (request.HttpMethod == "GET")
//                 {
// //показать, что пришло от клиента
//                     ShowRequestData(request);
// //завершаем работу сервера
//                     if (!flag) return;
//                 }
            }
        }
        
        public static void ListenerCallback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener) result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            
            string responseString = @"{""status"": ""OK""}";
             
            response.ContentType = "application/json; charset=UTF-8";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            
            // Write to response stream.
            Console.WriteLine("Processing request..");
            using (Stream output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
            }
            Console.WriteLine("Request is processed.");
        }
    }
}