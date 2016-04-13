﻿using System;
using System.Collections;


[Serializable]
public class Request
{
    private int id;
    private string description;
    private int quantity;
    private int table;
    private Destination dest;
    private State state;
    private double price;

    public int Id
    {
        get
        {
            return id;
        }
        set
        {
            id = value;
        }
    }
    public string Description
    {
        get
        {
            return description;
        }
        set
        {
            description = value;
        }
    }
    public int Quantity
    {
        get
        {
            return quantity;
        }
        set
        {
            quantity = value;
        }
    }
    public int Table
    {
        get
        {
            return table;
        }
        set
        {
            table = value;
        }
    }
    public Destination Destination
    {
        get
        {
            return dest;
        }
        set
        {
            dest = value;
        }
    }
    public State State
    {
        get
        {
            return state;
        }
        set
        {
            state = value;
        }
    }
    public double Price
    {
        get
        {
            return price;
        }
        set
        {
            price = value;
        }
    }

    public Request(int id, string desc, int quant, int table, Destination dest, double price)
    {
        this.Id = id;
        this.Description = desc;
        this.Quantity = quant;
        this.Table = table;
        this.Destination = dest;
        this.State = State.Unattended;
        this.Price = price;
    }
}

public enum Destination { Kitchen, Bar };
public enum State { Unattended, Preparing, Ready };

public delegate void AddedRequestDelegate(Request req);
public delegate void ChangedStateDelegate(Request req);

public interface IListSingleton
{
    event AddedRequestDelegate addedRequestEvent;

    ArrayList GetList();
    int GetNewId();
    void AddRequest(Request req);
}

/*public class AlterEventRepeater : MarshalByRefObject {
  public event AddedRequestDelegate addedRequestEvent;

  public override object InitializeLifetimeService() {
    return null;
  }

  public void Repeater(Request req) {
    if (addedRequestEvent != null)
            addedRequestEvent(req);
  }
}*/
