using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace ServerForAntivirus
{
    class Program
    {
        static Socket socketForUpdate = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Socket socketForGetVirus = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //static List<Socket> alreadyСonnected = new List<Socket>();
        static List<Thread> serverUpdateThreads = new List<Thread>();
        static List<Thread> serverGetVirusThreads = new List<Thread>();
        static Thread transferThread;
        static int port = 5555;

        static void Main(string[] args)
        {
            AddInMainVirusFile();

            transferThread = new Thread(new ThreadStart(TransferHash))
            {
                IsBackground = true
            };
            transferThread.Start();

            socketForUpdate.Bind(new IPEndPoint(IPAddress.Any, port));
            socketForGetVirus.Bind(new IPEndPoint(IPAddress.Any, port + 1));
            socketForUpdate.Listen(50);
            socketForGetVirus.Listen(50);
            Console.WriteLine("Сервер запущен");
            while (true)
            {
                //try
                //{
                    Socket socketForUndateThread = socketForUpdate.Accept();
                    Socket socketForGetVirusThread = socketForGetVirus.Accept();
                    Console.WriteLine($"К серверу подключен пользователь {socketForUndateThread.RemoteEndPoint}");
                    if (!serverUpdateThreads.All(x => x.IsAlive))
                    {
                        for (int i = 0; i < serverUpdateThreads.Count; i++)
                        {
                            if (!serverUpdateThreads[i].IsAlive)
                            {
                                serverUpdateThreads.RemoveAt(i);
                            }
                        }
                    }
                    if (!serverGetVirusThreads.All(x=>x.IsAlive))
                    {
                        for (int i = 0; i < serverGetVirusThreads.Count; i++)
                        {
                            if (!serverGetVirusThreads[i].IsAlive)
                            {
                                serverGetVirusThreads.RemoveAt(i);
                            }
                        }
                    }

                    serverUpdateThreads.Add(new Thread(ServerWorkUpdate) { IsBackground = true });
                    serverGetVirusThreads.Add(new Thread(ServerWorkGetVirus) { IsBackground = true });
                    serverUpdateThreads[serverUpdateThreads.Count - 1].Start(new UserOnServer(socketForUndateThread,socketForGetVirusThread, serverUpdateThreads.Count - 1));
                    serverGetVirusThreads[serverGetVirusThreads.Count - 1].Start(new UserOnServer(socketForUndateThread, socketForGetVirusThread, serverGetVirusThreads.Count - 1));
                //}
                //catch (Exception)
                //{

                //}
            }
        }

        private static void AddInMainVirusFile()
        {
            List<string> arrayOfStringInFile = null;
            if (File.Exists(Directory.GetCurrentDirectory() + @"\HashForCheck.vih"))
            {
                string stringInFile = string.Empty;
                using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + @"\HashForCheck.vih", Encoding.UTF8))
                {
                    stringInFile = sr.ReadToEnd();
                }

                if (stringInFile != string.Empty)
                {
                    stringInFile = stringInFile.Replace("\r", "");
                    arrayOfStringInFile = stringInFile.Split('\n').ToList();
                    stringInFile = string.Empty;
                    for (int i = 0; i < arrayOfStringInFile.Count; i++)
                    {
                        if (arrayOfStringInFile.Where(x => x == arrayOfStringInFile[i]).ToArray().Length >= 3 && arrayOfStringInFile[i] != "")
                        {
                            if (File.Exists(Directory.GetCurrentDirectory() + @"\HashWhichTheChecked.vih"))
                            {
                                using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + @"\HashWhichTheChecked.vih", Encoding.UTF8))
                                {
                                    stringInFile = sr.ReadToEnd();
                                }
                                if (stringInFile != string.Empty && !stringInFile.Contains(arrayOfStringInFile[i]))
                                {
                                    using (StreamWriter stream = new StreamWriter(Directory.GetCurrentDirectory() + @"\HashWhichTheChecked.vih",
                                    true,
                                    Encoding.UTF8))
                                    {
                                        stream.WriteLine(arrayOfStringInFile[i]);
                                    }
                                    string stringForRemove = arrayOfStringInFile[i];
                                    i = arrayOfStringInFile.Where(x => x == arrayOfStringInFile[i]).Select((hash, index) => new { hash, index }).First().index - 1;
                                    arrayOfStringInFile.RemoveAll(x => x == stringForRemove);
                                }
                            }
                            else
                            {
                                using (StreamWriter stream = new StreamWriter(Directory.GetCurrentDirectory() + @"\HashWhichTheChecked.vih",
                                true,
                                Encoding.UTF8))
                                {
                                    stream.WriteLine(arrayOfStringInFile[i]);
                                }
                                string stringForRemove = arrayOfStringInFile[i];
                                i = arrayOfStringInFile.Where(x => x == arrayOfStringInFile[i]).Select((hash, index) => new { hash, index }).First().index - 1;
                                arrayOfStringInFile.RemoveAll(x => x == stringForRemove);
                            }
                        }
                    }
                }
            }

            if (arrayOfStringInFile.Count > 0 && !arrayOfStringInFile.All(x => x == ""))
            {
                using (StreamWriter stream = new StreamWriter(Directory.GetCurrentDirectory() + @"\HashForCheck.vih",
                false,
                Encoding.UTF8))
                {
                    for (int i = 0; i < arrayOfStringInFile.Count; i++)
                    {
                        stream.WriteLine(arrayOfStringInFile[i]);
                    }
                }
            }
            else
            {
                using (StreamWriter stream = new StreamWriter(Directory.GetCurrentDirectory() + @"\HashForCheck.vih",
                false,
                Encoding.UTF8))
                {
                }
            }
        }

        private static void TransferHash()
        {
            while (true)
            {
                if (File.Exists(Directory.GetCurrentDirectory() + @"\HashForCheck.vih"))
                {
                    string stringInFile = string.Empty;
                    using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + @"\HashForCheck.vih", Encoding.UTF8))
                    {
                        stringInFile = sr.ReadToEnd();
                    }

                    if (stringInFile !=string.Empty)
                    {
                        stringInFile = stringInFile.Replace("\r", "");
                        string[] arrayOfStringInFile = stringInFile.Split('\n');
                        stringInFile = string.Empty;
                        if (File.Exists(Directory.GetCurrentDirectory() + @"\HashWhichTheChecked.vih"))
                        {
                            using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + @"\HashWhichTheChecked.vih", Encoding.UTF8))
                            {
                                stringInFile = sr.ReadToEnd();
                            }
                        }
                        for (int i = 0; i < arrayOfStringInFile.Length; i++)
                        {
                            if (arrayOfStringInFile.Where(x=>x==arrayOfStringInFile[i]).ToArray().Length>=3)
                            {

                                if (stringInFile != string.Empty)
                                {
                                    if (!stringInFile.Contains(arrayOfStringInFile[i]))
                                    {
                                        using (StreamWriter stream = new StreamWriter(Directory.GetCurrentDirectory() + @"\HashWhichTheChecked.vih",
                                        true,
                                        Encoding.UTF8))
                                        {
                                            stream.WriteLine(arrayOfStringInFile[i]);
                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamWriter stream = new StreamWriter(Directory.GetCurrentDirectory() + @"\HashWhichTheChecked.vih",
                                    true,
                                    Encoding.UTF8))
                                    {
                                        stream.WriteLine(arrayOfStringInFile[i]);
                                    }
                                }                                
                            }
                        }
                    }
                }
                Thread.Sleep(30000);
            }
        }

        private static void ServerGetVirus(object obj)
        {
            try
            {
                string message = (string)obj;
                Console.WriteLine($"Запрос на сохранение хеша - {message}");
                string hashInEndFile = string.Empty;
                if (File.Exists(Directory.GetCurrentDirectory() + @"\HashWhichTheChecked.vih"))
                {
                    using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + @"\HashWhichTheChecked.vih", Encoding.UTF8))
                    {
                        hashInEndFile = sr.ReadToEnd();
                    }
                }

                if (!hashInEndFile.Contains(message) || hashInEndFile == string.Empty)
                {
                    using (StreamWriter stream = new StreamWriter(Directory.GetCurrentDirectory() + @"\HashForCheck.vih",
                    true,
                    Encoding.UTF8))
                    {
                        stream.WriteLine(message);
                        Console.WriteLine("Сохранение прошло успешно");
                    }
                }
                else
                    Console.WriteLine("Произошла ошибка");
            }
            catch (Exception)
            {
            }
        }

        private static void CallBase(object obj)
        {
            try
            {
                Socket userSoket = (Socket)obj;
                Console.WriteLine("Получен запрос на базу hash");
                if (File.Exists(Directory.GetCurrentDirectory() + @"\HashWhichTheChecked.vih"))
                {

                    string hashInEndFile = string.Empty;
                    using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + @"\HashWhichTheChecked.vih", Encoding.UTF8))
                    {
                        hashInEndFile += sr.ReadToEnd();
                    }
                    if (hashInEndFile != string.Empty)
                    {
                        byte[] buffer1 = Encoding.UTF8.GetBytes(hashInEndFile);
                        byte[] buffer = Encoding.UTF8.GetBytes(Convert.ToString(buffer1.Length));
                        userSoket.Send(buffer);
                        buffer = new byte[3];
                        userSoket.Receive(buffer);
                        if (Encoding.UTF8.GetString(buffer) == "#NE")
                        {
                            userSoket.Send(buffer1);
                        }
                    }
                    Console.WriteLine("Запрос Выполнен");
                }
            }
            catch (Exception)
            {
            }
        }

        private static void ServerWorkGetVirus(object obj)
        {
            UserOnServer user = (UserOnServer)obj;
            while (true)
            {
                Socket userSoket = user.UserSocketGetVirus;
                try
                {
                    byte[] buffer = new byte[1024];
                    userSoket.Receive(buffer);

                    string message = Encoding.UTF8.GetString(buffer).Replace("\0", "");
                    string HashCode = message.Substring(0, 2);
                    message = message.Remove(0, 2);
                    if (HashCode == "#V")
                    {
                        Console.WriteLine($"Запрос на сохранение хеша - {message}");
                        string hashInEndFile = string.Empty;
                        if (File.Exists(Directory.GetCurrentDirectory() + @"\HashWhichTheChecked.vih"))
                        {
                            using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + @"\HashWhichTheChecked.vih", Encoding.UTF8))
                            {
                                hashInEndFile = sr.ReadToEnd();
                            }
                        }

                        if (!hashInEndFile.Contains(message) || hashInEndFile == string.Empty)
                        {
                            using (StreamWriter stream = new StreamWriter(Directory.GetCurrentDirectory() + @"\HashForCheck.vih",
                            true,
                            Encoding.UTF8))
                            {
                                stream.WriteLine(message);
                                Console.WriteLine("Сохранение прошло успешно");
                            }
                        }
                        else
                            Console.WriteLine("Произошла ошибка");
                        //Thread threadForGetVirus = new Thread(ServerGetVirus)
                        //{
                        //    IsBackground = true
                        //};
                        //threadForGetVirus.Start(message);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Был закрыт порт");
                    userSoket.Disconnect(false);
                    //serverGetVirusThreads[user.UserThreadIndex].Abort();
                    break;
                }
            }
        }

        private static void ServerWorkUpdate(object obj)
        {
            UserOnServer user = (UserOnServer)obj;
            while (true)
            {
                Socket userSoket = user.UserSocketUpdate;
                try
                {
                    byte[] buffer = new byte[1024];
                    userSoket.Receive(buffer);

                    string message = Encoding.UTF8.GetString(buffer).Replace("\0","");
                    string HashCode = message.Substring(0,2);
                    message = message.Remove(0,2);
                    if (HashCode == "#U")
                    {
                        Console.WriteLine("Получен запрос на базу hash");
                        if (File.Exists(Directory.GetCurrentDirectory() + @"\HashWhichTheChecked.vih"))
                        {

                            string hashInEndFile = string.Empty;
                            using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + @"\HashWhichTheChecked.vih", Encoding.UTF8))
                            {
                                hashInEndFile += sr.ReadToEnd();
                            }
                            if (hashInEndFile != string.Empty)
                            {
                                byte[] buffer1 = Encoding.UTF8.GetBytes(hashInEndFile);
                                buffer = Encoding.UTF8.GetBytes(Convert.ToString(buffer1.Length));
                                userSoket.Send(buffer);
                                buffer = new byte[3];
                                userSoket.Receive(buffer);
                                if (Encoding.UTF8.GetString(buffer) == "#NE")
                                {
                                    userSoket.Send(buffer1);
                                }
                            }
                            Console.WriteLine("Запрос Выполнен");
                        }
                        //Thread threadForUpdate = new Thread(CallBase)
                        //{
                        //    IsBackground = true
                        //};
                        //threadForUpdate.Start(userSoket);
                    }
                    
                }
                catch (Exception)
                {
                    Console.WriteLine("Был закрыт порт");
                    userSoket.Disconnect(false);
                    //serverUpdateThreads[user.UserThreadIndex].Abort();
                    break;
                }
            }
        }
    }
}
