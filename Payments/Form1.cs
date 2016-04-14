using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Payments
{
    public partial class Form1 : Form
    {
        IListSingleton listServer; // data struct
        AlterEventRepeater repeater;

        delegate void Dispatcher();
        
        public Form1()
        {
            RemotingConfiguration.Configure("Payments.exe.config", false);
            listServer = (IListSingleton)RemoteNew.New(typeof(IListSingleton));
            InitializeComponent();

            repeater = new AlterEventRepeater();
            repeater.alterEvent += AlterHandler;
            listServer.alterEvent += new AlterDelegate(repeater.Repeater);

        }

        // Handler
        public void AlterHandler(Request req) // Handler
        {
            if (this.listView1.InvokeRequired)
            {
                Dispatcher d = new Dispatcher(DisplayReadyRequests);
                this.Invoke(d);
                Console.WriteLine("Thread");
            }
            else
            {
                DisplayReadyRequests();
                Console.WriteLine("No THREAD");
            }
        }

        private void DisplayReadyRequests()
        {
            // flush all request on listviews
            listView1.Items.Clear();

            var listReadyKitchen = listServer.GetListByStateAndDest(State.Ready, Destination.Kitchen);
            var listReadyBar = listServer.GetListByStateAndDest(State.Ready, Destination.Bar);

            foreach (Request req in listReadyKitchen)
            {
                double reqPrice = req.Price * req.Quantity;
                ListViewItem reqItem = new ListViewItem(new string[] { req.Id.ToString(), req.Description, req.Quantity.ToString(), req.Table.ToString(), req.Destination.ToString(), reqPrice.ToString() });
                listView1.Items.Add(reqItem);
            }

            foreach (Request req in listReadyBar)
            {
                double reqPrice = req.Price * req.Quantity;
                ListViewItem reqItem = new ListViewItem(new string[] { req.Id.ToString(), req.Description, req.Quantity.ToString(), req.Table.ToString(), req.Destination.ToString(), reqPrice.ToString() });
                listView1.Items.Add(reqItem);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            int table = Int32.Parse(comboBox4.SelectedItem.ToString());

            ArrayList listTable = listServer.GetListByTable(table);

            bool foundNotReady = false;
            foreach (Request req in listTable)
            {
                if (req.State != State.Ready)
                {
                    foundNotReady = true;
                }
            }

            if (!foundNotReady)
            {
                listServer.DeleteAllReqsTable(table);
            }

        }


    }

    /* Mechanism for instanciating a remote object through its interface, using the config file */

    class RemoteNew
    {
        private static Hashtable types = null;

        private static void InitTypeTable()
        {
            types = new Hashtable();
            foreach (WellKnownClientTypeEntry entry in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
                types.Add(entry.ObjectType, entry);
        }

        public static object New(Type type)
        {
            if (types == null)
                InitTypeTable();
            WellKnownClientTypeEntry entry = (WellKnownClientTypeEntry)types[type];
            if (entry == null)
                throw new RemotingException("Type not found!");
            return RemotingServices.Connect(type, entry.ObjectUrl);
        }
    }
}
