using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Chaos.Mcm;
using Chaos.Mcm.Data.Dto;
using Chaos.Portal.Core.Cache.Couchbase;
using Chaos.Portal.Core.Data;
using Chaos.Portal.Core.Indexing.View;
using Chaos.Portal.Core.Logging.Database;
using Chaos.Portal.Module.Larmfm;
using Object = Chaos.Mcm.Data.Dto.Object;

namespace Chaos.Portal.Larmfm.Indexer
{
  public class Program
  {
    public  static void Main(string[] args)
    {
        try
        {
            Index();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Message: "+ex.Message);
            Console.WriteLine("Stacktrace: " + ex.StackTrace);
            Console.ReadLine();
        }
        Index();
    }

      private static void Index()
      {
          Console.Clear();
          var cache = new Cache(new Couchbase.CouchbaseClient());
          var portalRepository =
            new PortalRepository().WithConfiguration(
              System.Configuration.ConfigurationManager.ConnectionStrings["Portal"].ConnectionString);
          var portal = new PortalApplication(cache, new ViewManager(new Dictionary<string, IView>(), cache),
                                             portalRepository, new DatabaseLoggerFactory(portalRepository));
          var mcm = new McmModule();
          var larm = new LarmModule();

          portal.AddModule(mcm);
          portal.AddModule(larm);

          Thread.Sleep(10000);

          const uint PageSize = 100;
          var indexedCount = 0;
          var retrievedCount = 0;

          uint folderId = 0;

          Console.WriteLine("Enter folderId (leave blank for all):");
          uint.TryParse(Console.ReadLine(), out folderId);

          var t1 = new Stopwatch();

          for (uint i = 0; ; i++)
          {
              t1.Reset();
              t1.Start();
              IList<Chaos.Mcm.Data.Dto.Object> objects = new List<Object>();
               
              if(folderId == 0)  
                objects = mcm.McmRepository.ObjectGet(null, i, PageSize, true, true, true, true, true).ToList();

              if(folderId > 0)
                  objects = mcm.McmRepository.ObjectGet(folderId, i, PageSize, true, true, true, true, true).ToList();
             
              Write(0, 1, String.Format("ObjectGet time: {0}", t1.Elapsed));
              retrievedCount += objects.Count;

              new Thread(() =>
              {

                  var objs = objects;
                  var t2 = new Stopwatch();
                  t2.Start();
                  
                  try
                  {
                      portal.ViewManager.Index(objs);
                  }
                  catch (Exception)
                  {
                      Thread.Sleep(30000);
                      portal.ViewManager.Index(objs);
                  }
                  
                  Write(0, 2, String.Format("Index time: {0}", t2.Elapsed));
                  t2.Restart();

                  indexedCount += 100;
                  Write(0, 5, String.Format("Objects indexed: {0}", indexedCount));
              }).Start();

              Write(0, 4, String.Format("Objects retrieved: {0}", retrievedCount));
              if (objects.Count != PageSize) break;
          }
      }

    private static readonly object LockObject = new object();
    private static void Write(int left, int top, string message)
    {
      lock (LockObject)
      {
        Console.SetCursorPosition(left, top);
        Console.Write(message);
      }
    }
  }
}
