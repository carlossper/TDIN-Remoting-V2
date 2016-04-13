using System;
using System.Collections;
using System.Threading;


    public class ListSingleton : MarshalByRefObject, IListSingleton
    {
        ArrayList requestsList;
        public event AddedRequestDelegate addedRequestEvent;
        public event ChangedStateDelegate changedStateEvent;
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

        public void ChangeState(int id)
        {
            foreach (Request req in requestsList)
            {
                if (req.Id == id)
                    if (req.State != State.Ready)
                    {
                        req.State++;
                        NotifyClientsChange(req);
                        return;
                    }
                    else Console.WriteLine("Invalid ChangeState request");
            }
        }

        public ArrayList GetListByTable(int table)
        {
            ArrayList res = new ArrayList();
            foreach (Request req in requestsList)
                if (req.Table == table)
                    res.Add(req);
            return res;
            /*foreach(Request req in requestsList)
            {
                if(req.Table == table)
                {
                    Console.WriteLine(req.Description + "\n");
                    Console.WriteLine("Quantity: " + req.Quantity + "\n");
                    Console.WriteLine("Price:" + req.Price + "\n");

                    totalPrice += req.Price;
                }
            }
            Console.WriteLine("Total table price: " + totalPrice + "\n");*/
        }

        public ArrayList GetListByState(State state)
        {
            ArrayList res = new ArrayList();
            foreach (Request req in requestsList)
                if (req.State == state)
                    res.Add(req);
            return res;
        }

        void NotifyClientsAdd(Request req)
        {
            if (addedRequestEvent != null)
            {
                Delegate[] invkList = addedRequestEvent.GetInvocationList();

                foreach (AddedRequestDelegate handler in invkList)
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
                            addedRequestEvent -= handler;
                            Console.WriteLine("Exception: Removed an event handler");
                        }
                    }).Start();
                }
            }
        }

        void NotifyClientsChange(Request req)
        {
            if (changedStateEvent != null)
            {
                Delegate[] invkList = changedStateEvent.GetInvocationList();

                foreach (ChangedStateDelegate handler in invkList)
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
                            changedStateEvent -= handler;
                            Console.WriteLine("Exception: Removed an event handler");
                        }
                    }).Start();
                }
            }
        }
    }