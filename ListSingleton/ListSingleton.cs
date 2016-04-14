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
        NotifyClients(req);
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

    public ArrayList GetListByStateAndDest(State state, Destination dest)
    {
        ArrayList res = new ArrayList();
        foreach (Request req in requestsList)
            if (req.State == state && req.Destination == dest)
                res.Add(req);
        return res;
    }

    public void ChangeState(int idReq)
    {
        foreach (Request req in requestsList)
        {
            if (req.Id == idReq)
            {
                req.State++;
                NotifyClients(req);
                return;
            }
        }
    }

    public void DeleteAllReqsTable(int table)
    {
        ArrayList arrReq = new ArrayList();
        foreach (Request req in requestsList)
            if (req.Table == table)
            {
                arrReq.Add(req);
            }

        foreach(Request req in arrReq)
        {
            requestsList.Remove(req);
        }
        NotifyClients((Request)arrReq[arrReq.Count-1]);

        Console.WriteLine("Table " + ((Request)arrReq[0]).Table + " :");
        double totalPrice = 0.0;
        foreach (Request req in arrReq)
        {
            Console.WriteLine(req.Description + " - Price : " + req.Price);
            totalPrice += req.Price;
        }

        Console.WriteLine(totalPrice);
    }

    void NotifyClients(Request req)
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