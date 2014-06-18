using System;
using System.Text;
using System.Collections.Generic;

namespace Nethack_Online_GUI
{
    public class TelnetController
    {
        TCPConnection telnetClient;
        ASCIIEncoding encoder;

        public TelnetController()
        {
            telnetClient = new TCPConnection();
            encoder = new ASCIIEncoding();
        }

        // Connect to server and send initial responses
        public void Connect(string username, string password, string host, int port)
        {
            if (telnetClient.Connected)
                throw new Exception("Already Connected");

            telnetClient.Connect(host, port);

            // Things our client is requesting
            SendCommand(TelnetHelper.DO, 0x03); // DO Suppress Go Ahead

            Console.WriteLine("Connected!");
        }

        public void Disconnect()
        {
            telnetClient.Disconnect();
        }

        public void PollServer()
        {
            if (!telnetClient.Connected)
                return;

            int dataLength;
            byte[] dataBytes;

            dataLength = telnetClient.ReceiveData();
            dataBytes = telnetClient.GetData();

            for (int i = 0; i < dataLength; ++i)
            {
                switch (dataBytes[i])
                {
                    // Interpret As Command
                    case TelnetHelper.IAC:
                        i = ProcessIAC(dataBytes, dataLength, i);
                        break;
                    // Terminal Command
                    case TelnetHelper.ESC:
                        i = ProcessTerminalData(dataBytes, dataLength, i);
                        break;
                }
            }
        }

        // If you want to add additional IAC responses add it in this method
        private int ProcessIAC(byte[] dataBytes, int dataLength, int i)
        {
            // IAC Negotiation Option
            byte negotiation = dataBytes[i + 1];
            byte option = dataBytes[i + 2];
            
            if (negotiation == TelnetHelper.DO)
            {
                Console.WriteLine("DO= " + dataBytes[i + 3].ToString("X"));

                if (TelnetHelper.GetOptionDescription(option) == "Echo")
                    SendCommand(TelnetHelper.WILL, 0x01);
                else if (TelnetHelper.GetOptionDescription(option) == "Suppress Go Ahead")
                    SendCommand(TelnetHelper.WILL, 0x03);
                else if (TelnetHelper.GetOptionDescription(option) == "Terminal Type")
                    SendCommand(TelnetHelper.WILL, 0x18);
                else if (TelnetHelper.GetOptionDescription(option) == "Terminal Speed")
                    SendCommand(TelnetHelper.WILL, 0x20);
                else if (TelnetHelper.GetOptionDescription(option) == "X Display Location")
                    SendCommand(TelnetHelper.WONT, 0x23); // Won't display X Location
                else if (TelnetHelper.GetOptionDescription(option) == "New Environment Option")
                    SendCommand(TelnetHelper.WILL, 0x27);
                else if (TelnetHelper.GetOptionDescription(option) == "Negotiate About Window Size")
                    SendCommand(TelnetHelper.WILL, 0x1F);

                i += 2;
            }

            else if (negotiation == TelnetHelper.SB)
            {
                Console.WriteLine("SB= " + dataBytes[i + 3].ToString("X"));

                if (TelnetHelper.GetOptionDescription(option) == "Terminal Type")
                    SendSubNegotiation(option, encoder.GetBytes("\0XTERM"));
                else if (TelnetHelper.GetOptionDescription(option) == "Terminal Speed")
                    SendSubNegotiation(option, encoder.GetBytes("\038400,38400"));
                else if (TelnetHelper.GetOptionDescription(option) == "New Environment Option")
                    SendSubNegotiation(option, encoder.GetBytes("\0"));

                i += 3;
            }

            return i;
        }

        // If you want to add additional Terminal responses add it in this method
        private int ProcessTerminalData(byte[] dataBytes, int dataLength, int i)
        {
            // Find end of the command
            int endOfLine;
            int endOfCommand = i;

            for (endOfLine = i; endOfLine < dataLength; ++endOfLine)
            {
                if (dataBytes[endOfCommand] != 0x20 && dataBytes[endOfLine] == 0x20) // space
                    endOfCommand = endOfLine;

                if (dataBytes[endOfLine] == 0)
                    break;
            }

            // Copy the data over
            byte[] terminalLine = new byte[endOfLine - i - (endOfCommand - i)];
            byte[] terminalCommand = new byte[endOfCommand - i > 0 ? endOfCommand - i - 1 : 0];

            // Terminal line
            for (int j = endOfCommand + 1; j < endOfLine; ++j)
                terminalLine[j - endOfCommand] = dataBytes[j];

            // Terminal command
            for (int j = i + 1; j < endOfCommand; ++j)
                terminalCommand[j - (i + 1)] = dataBytes[j];

            Console.WriteLine("[" + encoder.GetString(terminalCommand) + "]" + encoder.GetString(terminalLine));

            //ProcessTerminalCommand( terminalCommand, terminalLine);

            i += endOfLine - i - 1;

            return i;
        }

        // will be private but for testing it will be public
        public List<TerminalCell> ProcessTerminalCommand(NetHackController nhControl, byte[] command, byte[] data)
        {
            List<TerminalCell> updateList = new List<TerminalCell>();
            TerminalCell[,] termCells = nhControl.getTermCells();
            TerminalCell termCell;
            
            // [##d
            // Move to row
            if (command[command.Length - 1] == 'd')
            {
                // (byte[]) [16d => (int) 16
                int row = int.Parse(encoder.GetString(command, 1, command.Length - 2));
                int col = 0;

                // Copy data over
                for (int i = col; i < data.Length; ++i)
                {
                    termCell = new TerminalCell(i, row, (char)data[i]);

                    updateList.Add(termCell);
                    termCells[i,row] = termCell;
                }
            }

            // [##;##H
            // Move to row, col
            else if (command[command.Length - 1] == 'H')
            {
                int col, row;

                if (command.Length != 2)
                {
                    string commandStr = encoder.GetString(command);
                    int locationOfSemicolon = commandStr.IndexOf(';') - 1;
                    int locationOfEnd = (commandStr.Length - 2) - (locationOfSemicolon + 1);

                    col = int.Parse(commandStr.Substring(1, locationOfSemicolon));
                    row = int.Parse(commandStr.Substring(locationOfSemicolon + 2, locationOfEnd));
                }

                else
                {
                    col = 0;
                    row = 0;
                }

                //Console.WriteLine("(" + row + "," + col + ")");

                // Copy data over
                for (int i = 0; i < data.Length; ++i)
                {
                    termCell = new TerminalCell(col+i, row, (char)data[i]);

                    updateList.Add(termCell);
                    termCells[col+i, row] = termCell;
                }
            }

            // THREAD UNSAFE
            nhControl.setTermCells(termCells);

            return updateList;
        }

        public bool Connected
        {
            get
            {
                return telnetClient.Connected;
            }
        }

        private bool SendCommand(int negotiation, int option)
        {
            byte[] sendData;

            sendData = TelnetHelper.GetCommand(negotiation, option);

            return telnetClient.SendData(sendData);
        }

        private bool SendSubNegotiation(int option, byte[] data)
        {
            byte[] SubNegotiationCommand;

            SubNegotiationCommand = TelnetHelper.GetSubNegotiationCommand(option, data);

            return telnetClient.SendData(SubNegotiationCommand);
        }

        // Possibly needs to be rewritten
        private void DisplayReceivedData(byte[] data, int dataLength)
        {
            for (int i = 0; i < dataLength; ++i)
            {
                byte dataByte = data[i];

                if (dataByte != 0)
                {
                    if (dataByte == TelnetHelper.SB)
                        Console.Write("SB:   ");
                    else if (dataByte == TelnetHelper.SE)
                        Console.Write("SE:    ");
                    else if (dataByte == TelnetHelper.WILL)
                        Console.Write("WILL:  ");
                    else if (dataByte == TelnetHelper.WONT)
                        Console.Write("WON'T: ");
                    else if (dataByte == TelnetHelper.DO)
                        Console.Write("DO:    ");
                    else if (dataByte == TelnetHelper.DONT)
                        Console.Write("DON'T: ");
                    else if (dataByte == TelnetHelper.IAC)
                        Console.Write("\nIAC  ");
                    else if (dataByte == TelnetHelper.ESC)
                    {
                        Console.Write("\nESC: ");
                    }
                    else // Get Option Description of Command
                        Console.Write(TelnetHelper.GetOptionDescription(dataByte) + " "); //dataByte.ToString("X")
                }
            }

            Console.WriteLine();
        }
    }
}