using Blazor.Bluetooth;
using Microsoft.AspNetCore.Components;
using Plotly.Blazor.Traces;
using Plotly.Blazor;
using Plotly.Blazor.Traces.ScatterLib;
using Plotly.Blazor.LayoutLib;
using OW18B.Enums;
using OW18B.Parse;
using System.Timers;
using System.Diagnostics.Metrics;
using OW18B.DataManagement;

namespace OW18B.Pages
{
    public partial class Ble
    {
        [Inject]
        public required IBluetoothNavigator BluetoothNavigator { get; set; }

        private IDevice? device;

        private IBluetoothRemoteGATTCharacteristic? characteristicRead;
        private IBluetoothRemoteGATTCharacteristic? characteristicWrite;

        private string formattedStringMode = "";
        private string formattedStringValue = "";


        PlotlyChart chart;
        Config config = new Config();
        Plotly.Blazor.LayoutLib.YAxisLib.Title yAxisTitle = new Plotly.Blazor.LayoutLib.YAxisLib.Title()
        {
            Text = ""
        };

        Layout layout = new Layout();

        private List<object> xValues = new List<object> ();
        private List<object> yValues = new List<object>();

        // Using of the interface IList is important for the event callback!
        IList<ITrace> data;

        private Mode _mode=Mode.Percent;

        private System.Timers.Timer timer;

        private uint MaxNumber=100;

        protected override void OnInitialized()
        {
            // initialize axis
            layout.XAxis = new List<XAxis>
            {
                new()
                {
                    Title = new Plotly.Blazor.LayoutLib.XAxisLib.Title()
                    {
                        Text = "Time"
                    },
                    TickFormat = "%H:%M:%S.%f",
                    

                }
            };
            layout.YAxis = new List<YAxis>
            {
                new()
                {
                    Title = yAxisTitle
                }
            };
            // initialize chart data 
            data = new List<ITrace>
            {
                new Scatter
                {
                    Name = "ScatterTrace",
                    Mode = ModeFlag.Lines | ModeFlag.Markers,
                    X = xValues,
                    Y = yValues
                }
            };

            // Initialize and start the timer
            timer = new System.Timers.Timer(500); // Set timer interval to 400 milliseconds
            timer.Elapsed += TimerElapsed;
            timer.AutoReset = true; 
            timer.Start();

        }

        private void TimerElapsed(object? sender, ElapsedEventArgs e)
        {

            chart.Update();

            StateHasChanged();
        }

        private async Task ConnectAsync()
        {
            var query = new RequestDeviceQuery { AcceptAllDevices = true, OptionalServices = new List<string> { "0000fff0-0000-1000-8000-00805f9b34fb" } };
            try
            {
                device = await BluetoothNavigator.RequestDevice(query);
                device.OnGattServerDisconnected += GattDisconnected;
                await device.Gatt.Connect();


                var service = await device.Gatt.GetPrimaryService("0000fff0-0000-1000-8000-00805f9b34fb");
                characteristicRead = await service.GetCharacteristic("0000fff4-0000-1000-8000-00805f9b34fb");
                characteristicWrite = await service.GetCharacteristic("0000fff3-0000-1000-8000-00805f9b34fb");
                characteristicRead!.OnRaiseCharacteristicValueChanged += CharacteristicCallback;
                await characteristicRead!.StartNotifications();
            }
            catch (Exception)
            {
                GattDisconnected();
            }


        }

        private void Light()
        {
            if (characteristicWrite != null)
            {
                characteristicWrite.WriteValueWithResponse(new byte[] { 03, 00 });
            }
        }
        private void Hold()
        {
            if (characteristicWrite != null)
            {
                characteristicWrite.WriteValueWithResponse(new byte[] { 03, 01 });
            }
        }

        private void Select()
        {
            if (characteristicWrite != null)
            {
                characteristicWrite.WriteValueWithResponse(new byte[] { 01, 01 });
            }
        }
        private void Select2nd()
        {
            if (characteristicWrite != null)
            {
                characteristicWrite.WriteValueWithResponse(new byte[] { 01, 00 });
            }
        }
        private void Ranger()
        {
            if (characteristicWrite != null)
            {
                characteristicWrite.WriteValueWithResponse(new byte[] { 02, 01 });
            }
        }
        private void Ranger2nd()
        {
            if (characteristicWrite != null)
            {
                characteristicWrite.WriteValueWithResponse(new byte[] { 02, 00 });
            }
        }

        private void HzDuty()
        {
            if (characteristicWrite != null)
            {
                characteristicWrite.WriteValueWithResponse(new byte[] { 04, 01 });
            }
        }
        private void Bluetooth()
        {
            if (characteristicWrite != null)
            {
                characteristicWrite.WriteValueWithResponse(new byte[] { 04, 00 });
            }
        }


        private void GattDisconnected()
        {
            if (characteristicRead != null)
            {
                characteristicRead!.OnRaiseCharacteristicValueChanged -= CharacteristicCallback;
            }
            characteristicWrite = null;
            characteristicRead = null;
            device = null;
        }

        private void CharacteristicCallback(object? sender, CharacteristicEventArgs e)
        {

            Parser.Parse(e.Value, out short value, out Divisor divisor, out Mode mode, out Prefixes prefixes);

            if(_mode!=mode)
            {
                _mode = mode;
                xValues.Clear();
                yValues.Clear();
            }

            formattedStringMode = Parser.BuildMeasureMode(mode);
            formattedStringValue = Parser.BuildMeasureValue(value, divisor, mode, prefixes);
            yAxisTitle.Text = formattedStringMode;
            var measurement = new Measurement()
            {
                DateTime = DateTime.Now,
                Divisor = divisor,
                Mode = mode,
                Prefixes = prefixes,
                Value = value
            };

            yValues.Add(measurement.GetMeasurementValue());
            xValues.Add(measurement.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            while (xValues.Count > MaxNumber)
                xValues.RemoveAt(0);
            while (yValues.Count > MaxNumber)
                yValues.RemoveAt(0);

        }

 

        private async Task DisconnectAsync()
        {
            if (characteristicRead != null)
            {
                await characteristicRead!.StopNotifications();
                characteristicRead!.OnRaiseCharacteristicValueChanged -= CharacteristicCallback;
            }
            if (device != null)
            {
                await device!.Gatt.Disonnect();
            }
            characteristicWrite = null;
            characteristicRead = null;
            device = null;
        }

    }
  
}