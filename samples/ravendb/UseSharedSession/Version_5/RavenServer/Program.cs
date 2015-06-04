using System;
using Raven.Client.Embedded;
using System.Diagnostics;
using System.Globalization;

class Program
{
    static void Main()
    {
        Console.Title = "Raven Server";
        int port = 8080;

        string url = string.Format(CultureInfo.InvariantCulture, "http://localhost:{0}/", port);

        using (EmbeddableDocumentStore documentStore = new EmbeddableDocumentStore
                                   {
                                       DataDirectory = "Data",
                                       UseEmbeddedHttpServer = true,
                                       Configuration =
                                       {
                                           PluginsDirectory = Environment.CurrentDirectory,
                                           Port = port,
                                           HostName = "localhost"
                                       },
                                       DefaultDatabase = "Samples.UseSharedSession"
                                   })
        {
            documentStore.Initialize();
            Console.WriteLine("Raven server started on {0}. Press enter to stop.", url);
            Process.Start(url);
            Console.ReadLine();
        }
    }
}