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

namespace BarChangeState
{
    public partial class Form1 : Form
    {
        IListSingleton listServer; // data struct
        AlterEventRepeater repeater;

        delegate void Dispatcher();

        public Form1()
        {
            RemotingConfiguration.Configure("BarChangeState.exe.config", false);
            listServer = (IListSingleton)RemoteNew.New(typeof(IListSingleton));
            InitializeComponent();

            repeater = new AlterEventRepeater();
            repeater.alterEvent += AlterHandler;
            listServer.alterEvent += new AlterDelegate(repeater.Repeater);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        // Handler
        public void AlterHandler(Request req) // Handler
        {
            if (this.listView1.InvokeRequired)
            {
                Dispatcher d = new Dispatcher(UpdateBarListViews);
                this.Invoke(d);
                Console.WriteLine("Thread");
            }
            else
            {
                UpdateBarListViews();
                Console.WriteLine("No THREAD");
            }
        }

        public void UpdateBarListViews()
        {
            var listReqsUn = listServer.GetListByStateAndDest(State.Unattended, Destination.Bar);
            var listReqsPrep = listServer.GetListByStateAndDest(State.Preparing, Destination.Bar);

            // flush all request on listviews
            listView1.Items.Clear();
            listView2.Items.Clear();

            foreach (Request reqUn in listReqsUn)
            {
                ListViewItem reqItem = new ListViewItem(new string[] { reqUn.Id.ToString(), reqUn.Description, reqUn.Quantity.ToString(), reqUn.Table.ToString() });
                listView1.Items.Add(reqItem);
            }

            foreach (Request reqPrep in listReqsPrep)
            {
                ListViewItem reqItem = new ListViewItem(new string[] { reqPrep.Id.ToString(), reqPrep.Description, reqPrep.Quantity.ToString(), reqPrep.Table.ToString() });
                listView2.Items.Add(reqItem);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
