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

namespace Sala
{
    public partial class Form1 : Form
    {
        IListSingleton listServer; // data struct
        AlterEventRepeater repeater;

        delegate void Dispatcher();

        public Form1()
        {
            RemotingConfiguration.Configure("Sala.exe.config", false);
            listServer = (IListSingleton)RemoteNew.New(typeof(IListSingleton));
            InitializeComponent();

            repeater = new AlterEventRepeater();
            repeater.alterEvent += AlterHandler;
            listServer.alterEvent += new AlterDelegate(repeater.Repeater);

        }


        private void label1_Click(object sender, EventArgs e)
        {

        }
        

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Request fields init
            int table = Int32.Parse(comboBox4.SelectedItem.ToString());
            int quant = Int32.Parse(comboBox3.SelectedItem.ToString());
            string desc = comboBox1.SelectedItem.ToString();
            string dest = comboBox2.SelectedItem.ToString();
            Destination destEnum = (Destination)Enum.Parse(typeof(Destination), dest);
            double price = comboBox2.SelectedIndex + 1;
            int id = listServer.GetNewId();

            Request req = new Request(id, desc, quant, table, destEnum, price);
            listServer.AddRequest(req);
        }

        // Handler
        public void AlterHandler(Request req) // Handler
        {
            if(this.listView1.InvokeRequired)
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

            foreach(Request req in listReadyKitchen)
            {
                ListViewItem reqItem = new ListViewItem(new string[] { req.Id.ToString(), req.Description, req.Quantity.ToString(), req.Table.ToString(), req.Destination.ToString()});
                listView1.Items.Add(reqItem);
            }

            foreach (Request req in listReadyBar)
            {
                ListViewItem reqItem = new ListViewItem(new string[] { req.Id.ToString(), req.Description, req.Quantity.ToString(), req.Table.ToString(), req.Destination.ToString() });
                listView1.Items.Add(reqItem);
            }
        }

        private void label4_Click(object sender, EventArgs e)
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
