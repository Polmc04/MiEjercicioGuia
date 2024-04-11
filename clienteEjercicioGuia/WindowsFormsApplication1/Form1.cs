using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Socket server;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, 9060);


            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Green;
                MessageBox.Show("Connected!");
            }
            catch (SocketException)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Longitud.Checked)
            {
                // Quiere saber la longitud
                string mensaje = "1/" + nombre.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                MessageBox.Show("La longitud de tu nombre es: " + mensaje);
            }
            else if (Bonito.Checked) 
            {
                // Quiere saber si el nombre es bonito
                string mensaje = "2/" + nombre.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];


                if (mensaje == "SI")
                    MessageBox.Show("Tu nombre ES bonito.");
                else
                    MessageBox.Show("Tu nombre NO es bonito. Lo siento.");

            }
            else
            {
                // Enviamos nombre y altura
                string mensaje = "3/" + nombre.Text + "/" + alturaBox.Text;
                // Convertimos a Byte para envair al server
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                // Recibimos respuesta del server
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0]; // Convertimos a ASCII
                MessageBox.Show(mensaje); // Mostramos el mensaje
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            // Nos desconectamos

            // Mensaje desconexion "0/"
            string mensaje = "0/";

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje); // Convertimos a Bytes
            server.Send(msg); // enviamos mensaje de desconexion

            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Enviamos nombre y altura
            string mensaje = "4/";
            // Convertimos a Byte para envair al server
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Recibimos respuesta del server
            byte[] msg2 = new byte[80];
            server.Receive(msg2);
            mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0]; // Convertimos a ASCII
            labelServicios.Text = mensaje; // Mostramos el mensaje
        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        private void Longitud_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

    }
}
