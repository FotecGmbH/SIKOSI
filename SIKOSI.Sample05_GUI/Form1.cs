using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SIKOSI.Sample05_GUI
{
    public partial class Form1 : Form
    {
        private Task _udpHandlerTask;
        private Task _loraHandlerTask;
        private bool _running;
        private readonly HttpHandler _handler;
        private readonly CancellationTokenSource _cts;

        public Form1()
        {
            _cts = new CancellationTokenSource();
            _handler = new HttpHandler();
            InitializeComponent();
        }

        /// <summary>Raises the <see cref="E:System.Windows.Forms.Form.Load" /> event.</summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            base.OnLoad(e);
        }

        /// <summary>Raises the <see cref="E:System.Windows.Forms.Form.Activated" /> event.</summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnActivated(EventArgs e)
        {
            if (_running)
            {
                base.OnActivated(e);
                return;
            }

            _handler.Start(_cts.Token);
            _running = true;
            _udpHandlerTask = UdpHandler();
            _loraHandlerTask = LoraHandler();
            base.OnActivated(e);
        }

        /// <summary>Raises the <see cref="E:System.Windows.Forms.Form.Closed" /> event.</summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            _cts.Cancel();
            _running = false;
            base.OnClosed(e);
        }

        //temp label3
        //hum label5
        private async Task UdpHandler()
        {
            using var handler = new UdpClient(new IPEndPoint(IPAddress.Loopback, 6666));
            while (_running)
            {
                var message = await handler.ReceiveAsync();
                if (int.TryParse(Encoding.ASCII.GetString(message.Buffer), out int distance))
                {
                    label2.Text = $"{distance}mm";
                }

                await Task.Delay(1000);
            }
        }

        private async Task LoraHandler()
        {
            while (_running)
            {
                label3.Text = _handler.LatestTemp > 0.0 ? $"{_handler.LatestTemp}°C" : "";
                label5.Text = _handler.LatestHum > 0.0 ? $"{_handler.LatestHum}°C" : "";
                await Task.Delay(1000);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
