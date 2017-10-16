using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace EchoServer
{
    /// <summary>
    /// Helper class with useful methods used numerous times.
    /// </summary>
    static class UtilityHelper
    {
        /// <summary>
        /// The list conataining categories
        /// </summary>
        public static List<Category> categories;


        public static void Send(NetworkStream strm, string data)
        {
            var response = Encoding.UTF8.GetBytes(data);
            Console.WriteLine($"Response: {data}");
            try
            {
                strm.Write(response, 0, response.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static Request Read(NetworkStream strm, int size)
        {
            byte[] buffer = new byte[size];
            var bytesRead = strm.Read(buffer, 0, buffer.Length);
            var request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Request: {JsonConvert.SerializeObject(request)}");
            try
            {
                return JsonConvert.DeserializeObject<Request>(request);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        /// <summary>
        /// Method for getting the index from request paths. Also checks and throws errors on invalid api calls
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static int GetIndex(Request request, ref Response response)
        {
            int lengthOfSubPath = "/api/categories/".Length;

            if (string.IsNullOrEmpty(request.Path))
            {
                response.Status = "4 missing resource";
                throw new Exception("4 missing resource");
            }

            if (lengthOfSubPath > request.Path.Length)
            {
                Console.WriteLine("4 bad request, path is invalid");
                response.Status = "4 bad request";
                throw new Exception("4 bad request, path is invalid");
            }

            int requestIndex;
            string requestIndexStr = request.Path.Substring(lengthOfSubPath);
            Console.WriteLine("trying to find index: " + requestIndexStr);

            // check if index is int and not out of index bounds
            if (!int.TryParse(requestIndexStr, out requestIndex))
            {
                Console.WriteLine("4 bad request, index of category was text not integer");
                response.Status = "4 bad request";
                throw new Exception("response.Status = 5 not found, index of category was text not integer");
            }
            if (categories.Count <= requestIndex - 1)
            {
                response.Status = "5 not found";
                throw new Exception("response.Status = 5 not found");
            }

            // convert to zero based index
            return requestIndex - 1;

        }
    }
}
