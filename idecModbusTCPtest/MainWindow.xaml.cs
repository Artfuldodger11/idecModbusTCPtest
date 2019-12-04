using EasyModbus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace idecModbusTCPtest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ModbusClient modbusClient;
        string textIP;
        int[] plcHoldingRegisterStatus;
        bool[] plcInputsRegisterStatus;
        bool[] plcOutputRegisterStatus;

        

        public MainWindow()
        {
            InitializeComponent();
           
        }

   
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {

            {
                try
                {

                    ModbusClient mc = new ModbusClient("192.168.0.1", 502);    //Ip-Address and Port of Modbus-TCP-Server ( Port Normaly is 502)
                    this.modbusClient = mc;
                    modbusClient.Connect();
                    connectionStatus.Content = "Connected";
                    connectionStatus.Foreground = Brushes.Green;
                    btnConnect.IsEnabled = false;


                    DispatcherTimer dispatcherTimer = new DispatcherTimer();     // Dispatcher Timer for periodic start method -> dispatcherTimer_Tick 
                    dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick); // represents method whitch will handle periodic event --> in our case it is  dispatcherTimer_Tick
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 1); // TimeSpan(int day, int hours,int seconds)
                    dispatcherTimer.Start();
                }
                catch (Exception ex)
                {
                    connectionStatus.Content = ex.ToString();
                    connectionStatus.Foreground = Brushes.Red;
                }
            }
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                modbusClient.Disconnect();
                connectionStatus.Content = "Disconnected";
                connectionStatus.Foreground = Brushes.Gray;
                btnConnect.IsEnabled = true;

            }
            catch(Exception ex) {
                connectionStatus.Content = ex.ToString();
                connectionStatus.Foreground = Brushes.Red;
            }
        }

        private void OnTick()
        {
           
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (modbusClient.Connected) {


                try
                {
                    /* 
                     
                       @ modbusClient.ReadCoils(0, 1); - Reads Coils(Q)  from PLC as array bool[] - method ReadCoils(int startAddress, array size )
                       @ modbusClient.ReadHoldingRegisters( int StartingAdress, int[] ValuesToRead ) - reads from PLC "D" area (Holding Registers)
                    
                    

                       - All methods http://easymodbustcp.net/en/modbusclient-

                       - P.S. example for adressing for IDEC - Q0-Q7 ->  modbusClient.ReadCoils(0, 8)
                                                               Q10-Q17 -> modbusClient.ReadCoils(8, 8)
                             In IDEC software -> NetworkSetting -> Connection Settings -> Communication Mode - Modbus TCP Server
                                            
                                        !!! In Modbus TCP Server Configuration "Allow access bu IP Adress" MUST be UNCHECKED!!!
                                        Port NR : 502
                     */



                    modbusClient.WriteMultipleCoils(4, new bool[] { true, true, });
                    modbusClient.ReadCoils(0, 1);
                    plcInputsRegisterStatus = modbusClient.ReadCoils(0, 1);
                    data1.Content = plcInputsRegisterStatus[0];

                    


                    modbusClient.WriteSingleCoil(2 ,true);
                }
                catch (Exception ex)
                {
                    connectionStatus.Content = "No connection to PLC "; // for case when cable is disconnected 
                    connectionStatus.Foreground = Brushes.Red;
                    btnConnect.IsEnabled = true;
                }
            

            }
            else {
                modbusClient.Disconnect();
                connectionStatus.Content = "Disconnected";
                connectionStatus.Foreground = Brushes.Gray;
                btnConnect.IsEnabled = true;
            }
        }

        private void CheckBox1_Checked(object sender, RoutedEventArgs e)
        {
            //@ modbusClient.WriteMultipleCoils(0, boolArrayVariable); -Writes Coils(Q) to PLC as array bool[] -method WriteMultipleCoils(int startAddress, bool[])
            //@ modbusClient.WriteMultipleRegisters(int StartingAdress, int[] ValuesToWrite) - writes to PLC "D" area(Holding Registers)
            // @modbusClient.WriteSingleCoil
            // @modbusClient.WriteSingleRegister

            if (modbusClient.Connected)
            {
                modbusClient.WriteSingleCoil(7, true);
            }
            }

        private void CheckBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            if (modbusClient.Connected)
            { modbusClient.WriteSingleCoil(7, false); }
        }


    }
 }

