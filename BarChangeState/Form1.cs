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

        public Form1()
        {
            RemotingConfiguration.Configure("BarChangeState.exe.config", false);
            listServer = (IListSingleton)RemoteNew.New(typeof(IListSingleton));
            InitializeComponent();

            repeater = new AlterEventRepeater();
            repeater.alterEvent += ListReqAddedHandler;
            listServer.alterEvent += new AlterDelegate(repeater.Repeater);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        // Handler
        public void ListReqAddedHandler(Request req) // Handler
        {
            Console.WriteLine("Listing.......");
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
