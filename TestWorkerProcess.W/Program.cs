using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestWorkerProcess.W
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var _lock = new object();
            var index = 0;
            var sw = Stopwatch.StartNew();
            var tasks = new List<Task>();

            using (var client = new HttpClient())
            {
                for (var i = 1; i <= 11; i++)
                {
                    var url = $"http://localhost:83/api/values/get/{i}";
                    var task = client.GetAsync(url).ContinueWith(async (request) =>
                    {
                        try
                        {
                            var response = await request;
                            var responseString = await response.Content.ReadAsStringAsync();
                            lock (_lock)
                            {
                                index++;
                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    var timeDetails = string.Join("", response.Headers.GetValues("TimeDetails"));
                                    Console.WriteLine($"{index.ToString().PadRight(5)} {sw.ElapsedMilliseconds} ms code={response.StatusCode} response={responseString} timeDetails={timeDetails}");
                                }
                                else
                                {
                                    Console.WriteLine($"{index.ToString().PadRight(5)} Eroare {response.StatusCode}");
                                }
                                
                            }
                        }
                        catch (Exception exception)
                        {
                            //Console.WriteLine(exception);
                        }
                    });

                    tasks.Add(task);
                }

                Console.WriteLine($"All requests created in {sw.ElapsedMilliseconds} ms.");
                Task.WaitAll(tasks.ToArray());
            }

            Console.WriteLine($"All requests completed in {sw.ElapsedMilliseconds} ms.");
            Console.ReadKey();
        }
    }
}