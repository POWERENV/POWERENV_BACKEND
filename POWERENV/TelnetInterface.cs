using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace XTELNET
{
    enum Verbs {
        WILL = 251,
        WONT = 252,
        DO = 253,
        DONT = 254,
        IAC = 255
    }

    enum Options
    {
        SGA = 3
    }

    class XTelnetConnection
    {
        TcpClient tcpSocket;

        int TimeOutMs = 100;

        public XTelnetConnection(string Hostname, int Port)
        {
            tcpSocket = new TcpClient(Hostname, Port);
        }

        public string Login(string Username, string Password, int LoginTimeOutMs, int MyRetryTimeOut, int MyRetryCount) 
        {
            int NumTentativasLigacao = MyRetryCount;
            int j;
            string s;
            string prompt;


            int oldTimeOutMs = TimeOutMs;
            TimeOutMs = LoginTimeOutMs;

            j = 1;
            s = Read();
            while (j <= NumTentativasLigacao && !s.TrimEnd().EndsWith(":"))
            {
                if (!s.TrimEnd().EndsWith(":"))
                {
                    //Console.Write("Falha de ligação ao servidor (login ko)...\n");
                    //Console.Write("Aguardando {0} milisegundos. Tentativa {1} de {2}\n", MyRetryTimeOut, j, NumTentativasLigacao);
                    Console.Write(".");
                    Thread.Sleep(MyRetryTimeOut);
                    s = Read();
                    j += 1;
                }
                else
                {
                    j = NumTentativasLigacao + 1;
                }
            }

            if (j > NumTentativasLigacao)
            {
                Console.Write("Falha na ligação: login prompt ko");
                throw new Exception("Falha na ligação: login prompt ko");
            }

            
            WriteLine(Username + Environment.NewLine);

            s+= Read();
            Console.WriteLine(s);

           

            Thread.Sleep(1000);
            
            // ENVIAR PALAVRA PASSE
            j = 1;
            s += Read();
            while (j <= NumTentativasLigacao && !s.TrimEnd().EndsWith(":"))
            {
                 Console.WriteLine(s);

                if (!s.TrimEnd().EndsWith(":"))
                {
                    j += 1;
                    Console.Write("Falha de ligação ao servidor (password ko)...\n");
                    Console.Write("Tentativa {0} de {1}\n", j, NumTentativasLigacao);
                    Thread.Sleep(MyRetryTimeOut);
                    s += Read();
                }
                else
                {
                    j = NumTentativasLigacao + 1;
                }
            }

            if (j > NumTentativasLigacao)
            {
                throw new Exception("Failed to connect : no password prompt");
            }
            WriteLine(Password + Environment.NewLine);


            // AGUARDAR PELO PROMPT
            j = 1;
            s += Read();
            prompt = s.TrimEnd();
            prompt = s.Substring(prompt.Length - 1, 1);
            while (j <= 10 && (prompt != "$" && prompt != ">" && prompt != "#"))
            {
                Console.WriteLine(s);
                if (prompt != "$" && prompt != ">" && prompt != "#")
                {
                    j += 1;
                    Thread.Sleep(1000);
                    s += Read();
                    prompt = s.TrimEnd();
                    prompt = s.Substring(prompt.Length - 1, 1);
                }
                else
                {
                    j = NumTentativasLigacao + 1;
                }
            }

            if (j > NumTentativasLigacao)
            {
                throw new Exception("\n\n\nServidor não devolveu prompt.\n\n");
            }


            s += Read();
            TimeOutMs = oldTimeOutMs;
            return s;
        }

        public void WriteLine(string cmd)
        {
            Write(cmd + "\n");
        }

        public void Write(string cmd)
        {
            if (!tcpSocket.Connected) return;
            byte[] buf = System.Text.ASCIIEncoding.ASCII.GetBytes(cmd.Replace("\0xFF","\0xFF\0xFF"));
            tcpSocket.GetStream().Write(buf,  0, buf.Length);
        }

        public string Read()
        {
            if (!tcpSocket.Connected) return null;
            StringBuilder sb=new StringBuilder();
            do
            {
                ParseTelnet(sb);
                System.Threading.Thread.Sleep(TimeOutMs);
            } while (tcpSocket.Available > 0);
            return sb.ToString();
        }

        public bool IsConnected
        {
            get { return tcpSocket.Connected; }
        }

        void ParseTelnet(StringBuilder sb)
        {
            while (tcpSocket.Available > 0)
            {
                int input = tcpSocket.GetStream().ReadByte();
                switch (input)
                {
                    case -1 :
                        break;
                    case (int)Verbs.IAC:
                        // interpret as command
                        int inputverb = tcpSocket.GetStream().ReadByte();
                        if (inputverb == -1) break;
                        switch (inputverb)
                        {
                            case (int)Verbs.IAC: 
                                //literal IAC = 255 escaped, so append char 255 to string
                                sb.Append(inputverb);
                                break;
                            case (int)Verbs.DO: 
                            case (int)Verbs.DONT:
                            case (int)Verbs.WILL:
                            case (int)Verbs.WONT:
                                // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                int inputoption = tcpSocket.GetStream().ReadByte();
                                if (inputoption == -1) break;
                                tcpSocket.GetStream().WriteByte((byte)Verbs.IAC);
                                if (inputoption == (int)Options.SGA )
                                    tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WILL:(byte)Verbs.DO); 
                                else
                                    tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WONT : (byte)Verbs.DONT); 
                                tcpSocket.GetStream().WriteByte((byte)inputoption);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        sb.Append( (char)input );
                        break;
                }
            }
        }
    }
}
