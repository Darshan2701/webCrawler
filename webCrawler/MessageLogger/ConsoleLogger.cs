using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webCrawler
{
    class ConsoleLogger : IMessageService
    {
        private static object lockObject = new object();

        private static IMessageService instance;
        public static IMessageService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                            instance = new ConsoleLogger();
                    }
                }
                return instance;
            }
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }



    }
}
