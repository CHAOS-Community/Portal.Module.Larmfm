using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Chaos.Mcm;
using Chaos.Portal.Core.Cache.Couchbase;
using Chaos.Portal.Core.Data;
using Chaos.Portal.Core.Indexing.View;
using Chaos.Portal.Core.Logging.Database;
using Chaos.Portal.Module.Larmfm;

namespace Chaos.Portal.Larmfm.Indexer
{
  class Program
  {
    private static void Main(string[] args)
    {
      Console.Clear();
      var cache = new Cache(new Couchbase.CouchbaseClient());
      var portalRepository =
        new PortalRepository().WithConfiguration(
          "user id=larm-app;password=0n44Fx4f4m2jNtuLuA6ym88mr3h40D;server=mysql01.cpwvkgghf9fg.eu-west-1.rds.amazonaws.com;persist security info=True;database=larm-portal;Allow User Variables=True;CharSet=utf8;");
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
      var t1 = new Stopwatch();

      for (uint i = 0;; i++)
      {
        t1.Reset();
        t1.Start();
        var objects = mcm.McmRepository.ObjectGet(null, i, PageSize, true, true, true, true, true);
        Write(0, 1, String.Format("ObjectGet time: {0}", t1.Elapsed));
        retrievedCount += objects.Count;
        
        new Thread(() =>
          {
            var objs = objects;
            var t2 = new Stopwatch();
            t2.Start();
            portal.ViewManager.Index(objs);
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
