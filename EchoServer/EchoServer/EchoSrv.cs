using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace EchoServer
{
    class EchoSrv
    {
        public EchoSrv()
        {
            int port = 5000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            var server = new TcpListener(localAddr, port);

            server.Start();

            UtilityHelper.categories = new List<Category>
            {
                new Category(1, "Beverages"),
                new Category(2, "Condiments"),
                new Category(3, "Confections")
            };

            Console.WriteLine("Started");

            while (true)
            {
                var client = server.AcceptTcpClient();

                Console.WriteLine("Client connected");

                var thread = new Thread(HandleClient);

                thread.Start(client);
            }
        }

        static void HandleClient(object clientObj)
        {
            var client = clientObj as TcpClient;

            if (client == null) return;

            var strm = client.GetStream();
            var response = new Response();

            // set response status to 6 error to enable throws to still send response
            response.Status = "6";
            try
            {
                var request = UtilityHelper.Read(strm, client.ReceiveBufferSize);

                if (request == null)
                {
                    throw new Exception("request was null or unreadable");
                }

                // check for code 4 errors
                bool codeFourError = false;
                if (request.Method == null)
                {
                    response.Status = "4 missing method";
                    codeFourError = true;
                }

                if (request.Date == null)
                {
                    if (codeFourError) { response.Status = response.Status + ", 4 missing date"; }
                    else { response.Status = "4 missing date"; }
                }

                // write out info
                Console.WriteLine("request: " + request);
                Console.WriteLine("request-method: " + request.Method);
                Console.WriteLine("request-path: " + request.Path);

                switch (request.Method)
                {
                    case "read":
                        {
                            Console.WriteLine("Method is read");

                            if (request.Path.Contains("/api/categories/"))
                            {
                                Console.WriteLine("searching for index after path: /api/categories/");
                                int requestIndex = UtilityHelper.GetIndex(request, ref response);


                                Console.WriteLine("Path is categories/" + requestIndex);

                                response.Body = JsonConvert.SerializeObject(UtilityHelper.categories[requestIndex]);
                                response.Status = "1 Ok";
                            }

                            else if (request.Path.Equals("/api/categories"))
                            {
                                Console.WriteLine("Path is categories");

                                response.Body = JsonConvert.SerializeObject(UtilityHelper.categories);
                                response.Status = "1 Ok";
                            }
                            else
                            {
                                Console.WriteLine("4 bad request, path is invalid");
                                response.Status = "4 bad request";
                                throw new Exception("4 bad request, path is invalid");
                            }
                            break;
                        }

                    case "create":
                        {
                            Console.WriteLine("Method is create");

                            // create object from body
                            Category reqBodyObject;
                            if (string.IsNullOrEmpty(request.Body))
                            {
                                if (codeFourError)
                                {
                                    response.Status = response.Status + ", 4 missing body";
                                }
                                else
                                {
                                    response.Status = "4 missing body";
                                }
                                throw new Exception("4 missing body");
                            }
                            else
                            {
                                try
                                {
                                    reqBodyObject = JsonConvert.DeserializeObject<Category>(request.Body);
                                }
                                catch (Exception)
                                {

                                    if (codeFourError)
                                    {
                                        response.Status = response.Status + ", 4 illegal body";
                                    }
                                    else
                                    {
                                        response.Status = "4 illegal body";
                                    }
                                    throw new Exception("4 illegal body. Body was not json");

                                }
                            }

                            if (request.Path != "/api/categories")
                            {
                                Console.WriteLine("4 bad request, path is invalid");
                                response.Status = "4 bad request";
                                throw new Exception("4 bad request, path is invalid");
                            }

                            Console.WriteLine("wanting to add body" + request.Body);



                            Console.WriteLine("wanting to add:" + reqBodyObject.name);


                            Console.WriteLine("wanting to give id:" + (UtilityHelper.categories.Count + 1));

                            UtilityHelper.categories.Add(new Category((UtilityHelper.categories.Count + 1), reqBodyObject.name));

                            Console.WriteLine("categories after creation:" + JsonConvert.SerializeObject(UtilityHelper.categories));
                            response.Body = JsonConvert.SerializeObject(UtilityHelper.categories[(UtilityHelper.categories.Count - 1)]);
                            response.Status = "2 Created";
                            break;
                        }

                    case "delete":
                        {
                            int reqRemoveIndex = UtilityHelper.GetIndex(request, ref response);

                            Console.WriteLine("wanting to delete: {0}, at position {1}", request.Body, reqRemoveIndex);

                            UtilityHelper.categories.RemoveAt(reqRemoveIndex);

                            response.Status = "1 Ok";
                            break;
                        }

                    case "update":
                        {
                            Console.WriteLine("Method is update");

                            // create objects from body
                            Category reqBodyObject;
                            if (string.IsNullOrEmpty(request.Body))
                            {
                                if (codeFourError)
                                {
                                    response.Status = response.Status + ", 4 missing body";
                                }
                                else
                                {
                                    response.Status = "4 missing body";
                                }
                                throw new Exception("4 missing body");
                            }
                            else
                            {
                                try
                                {
                                    reqBodyObject = JsonConvert.DeserializeObject<Category>(request.Body);
                                }
                                catch (Exception)
                                {

                                    if (codeFourError)
                                    {
                                        response.Status = response.Status + ", 4 illegal body";
                                    }
                                    else
                                    {
                                        response.Status = "4 illegal body";
                                    }
                                    throw new Exception("4 illegal body. Body was not json");

                                }
                            }

                            // find final index through substring method
                            int requestIndex = UtilityHelper.GetIndex(request, ref response);


                            // write out info
                            Console.WriteLine("this is the new obj category: {0}, this is the cid: {1}, and the name: {2}",
                                reqBodyObject, reqBodyObject.cid, reqBodyObject.name);

                            Category catToChange = UtilityHelper.categories[requestIndex];

                            Console.WriteLine("wanting to update {0} to {1} and {2} to {3}", catToChange.cid,
                                reqBodyObject.cid,
                                catToChange.name, reqBodyObject.name);

                            // update
                            catToChange.cid = reqBodyObject.cid;
                            catToChange.name = reqBodyObject.name;

                            response.Status = "3 Updated";
                            response.Body = JsonConvert.SerializeObject(UtilityHelper.categories[requestIndex]);

                            break;
                        }

                    case "echo":
                        {
                            if (request.Body != null)
                            {
                                response.Body = request.Body;
                                response.Status = "1 ok";
                            }
                            else
                            {
                                if (codeFourError)
                                {
                                    response.Status = response.Status + ", missing body";
                                }
                                else
                                {
                                    response.Status = "4 missing body";
                                }
                                throw new Exception("missing body");
                            }
                            break;
                        }


                    default:
                        if (codeFourError)
                        {
                            response.Status = response.Status + ", 4 illegal method";
                        }
                        else
                        {
                            response.Status = "4 illegal method";
                        }
                        
                        throw new Exception("4 illegal method");
                        break;
                }


            }
            catch (Exception e)
            {
                if (e is IOException)
                {
                    Console.WriteLine("No request!!!");
                }
                else
                {
                    Console.WriteLine(e);
                }

            }
            UtilityHelper.Send(strm, JsonConvert.SerializeObject(response));

            strm.Close();
            client.Dispose();
        }
    }
}
