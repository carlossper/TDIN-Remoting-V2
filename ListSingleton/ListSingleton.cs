using System;
using System.Collections;
using System.Threading;


    public class ListSingleton : MarshalByRefObject, IListSingleton
    {
        ArrayList requestsList;
        public event AlterDelegate alterEvent;

        int id = 0;

        public ListSingleton()
        {
            Console.WriteLine("Constructor called.");
            requestsList = new ArrayList();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public ArrayList GetList()
        {
            Console.WriteLine("GetList() called.");
            return requestsList;
        }

        public int GetNewId()
        {
            return id++;
        }

        public void AddRequest(Request req)
        {
            requestsList.Add(req);
            NotifyClientsAdd(req);
        }

        public ArrayList GetListByTable(int table)
        {
            ArrayList res = new ArrayList();
            foreach (Request req in requestsList)
                if (req.Table == table)
                    res.Add(req);
            return res;
        }

        public ArrayList GetListByState(State state)
        {
            ArrayList res = new ArrayList();
            foreach (Request req in requestsList)
                if (req.State == state)
                    res.Add(req);
            return res;
        }

        public ArrayList GetListByStateAndDest(State state,Destination dest)
        {
            ArrayList res = new ArrayList();
            foreach (Request req in requestsList)
                if (req.State == state && req.Destination == dest)
                    res.Add(req);
            return res;
        }

        void NotifyClientsAdd(Request req)
        {
            if (alterEvent != null)
            {
                Delegate[] invkList = alterEvent.GetInvocationList();

                foreach (AlterDelegate handler in invkList)
                {
                    new Thread(() =>
                    {
                        try
                        {
                            handler(req);
                            Console.WriteLine("Invoking event handler");
                        }
                        catch (Exception)
                        {
                            alterEvent -= handler;
                            Console.WriteLine("Exception: Removed an event handler");
                        }
                    }).Start();
                }
            }
        }
    }