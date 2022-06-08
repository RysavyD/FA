using _3F.Model;
using _3F.Model.Extensions;
using _3F.Model.Model;
using _3F.Model.Utils;
using _3F.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.Globalization;
using System.Drawing;
using System.Net;

namespace _3F.Web.Utils
{
    public interface IChat
    {
        IEnumerable<ChatMessage> GetMessages(string roomName);
        void AddMessage(string message, string roomName);
        IEnumerable<User> GetOnlineUsers(string roomName);
        IEnumerable<string> GetRooms();
        void DeleteRoom(string roomName);
        void CreateRoom(string roomName);
    }

    public class ChatMessage
    {
        public string UserName { get; set; }
        public DateTime DateTime { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
    }

    public class MemoryChat : IChat
    {
        private static List<string> rooms;
        private static List<MemoryChatMessage> messages;
        private static List<ChatUser> users;

        public MemoryChat()
        {
            if (rooms == null)
            {
                rooms = new List<string>();
                rooms.Add("index");
                rooms.Add("admini");
                rooms.Add("tajná");
                rooms.Add("SA");
                rooms.Add("co dál");
                rooms.Add("Test1");
            }

            if (users == null)
            {
                users = new List<ChatUser>();
                users.Add(new ChatUser() { Room = "index", DateTime = new DateTime(2015, 3, 21), User = new User() { id = "1", name = "du-šan" } });
                users.Add(new ChatUser() { Room = "index", DateTime = new DateTime(2015, 3, 22), User = new User() { id = "1", name = "Manik" } });
                users.Add(new ChatUser() { Room = "index", DateTime = new DateTime(2015, 3, 23), User = new User() { id = "1", name = "Lenka" } });
                users.Add(new ChatUser() { Room = "index", DateTime = new DateTime(2015, 3, 24), User = new User() { id = "1", name = "Bobo" } });
                users.Add(new ChatUser() { Room = "admini", DateTime = new DateTime(2015, 1, 11), User = new User() { id = "1", name = "du-šan" } });
                users.Add(new ChatUser() { Room = "admini", DateTime = new DateTime(2015, 1, 12), User = new User() { id = "1", name = "Manik" } });
                users.Add(new ChatUser() { Room = "SA", DateTime = new DateTime(2013, 12, 31), User = new User() { id = "1", name = "du-šan" } });
                users.Add(new ChatUser() { Room = "SA", DateTime = new DateTime(2013, 12, 31), User = new User() { id = "1", name = "Nerothar" } });
            }

            if (messages == null)
            {
                messages = new List<MemoryChatMessage>();
                messages.Add(new MemoryChatMessage() { Room = "index", DateTime = new DateTime(2015, 3, 21), User = new User() { id = "1", name = "du-šan" }, Text = "abcd" });
                messages.Add(new MemoryChatMessage() { Room = "index", DateTime = new DateTime(2015, 3, 23), User = new User() { id = "1", name = "Manik" }, Text = "ble ble ble" });
                messages.Add(new MemoryChatMessage() { Room = "index", DateTime = new DateTime(2015, 3, 22), User = new User() { id = "1", name = "Lenka" }, Text = "kuželky" });
                messages.Add(new MemoryChatMessage() { Room = "index", DateTime = new DateTime(2015, 3, 24), User = new User() { id = "1", name = "Bobo" }, Text = "chlapeček" });
                messages.Add(new MemoryChatMessage() { Room = "admini", DateTime = new DateTime(2015, 1, 11), User = new User() { id = "1", name = "du-šan" }, Text = "já jsem admin" });
                messages.Add(new MemoryChatMessage() { Room = "admini", DateTime = new DateTime(2015, 1, 12), User = new User() { id = "1", name = "Manik" }, Text = "já jsem admin" });
                messages.Add(new MemoryChatMessage() { Room = "index", DateTime = new DateTime(2013, 12, 31), User = new User() { id = "1", name = "du-šan" }, Text = "beďas" });
                messages.Add(new MemoryChatMessage() { Room = "index", DateTime = new DateTime(2013, 12, 31), User = new User() { id = "1", name = "Nerothar" }, Text = "bude" });
            }
        }


        public IEnumerable<ChatMessage> GetMessages(string roomName)
        {
            return messages.Where(m => m.Room == roomName).Select(m => new ChatMessage() { DateTime = m.DateTime, Text = m.Text, UserName = m.User.name });
        }

        public void AddMessage(string message, string roomName)
        {
            using (var repository = new Repository())
                messages.Add(new MemoryChatMessage()
                    {
                        DateTime = Info.CentralEuropeNow,
                        Room = roomName,
                        Text = message.AtFilter(),
                        User = new User(repository.One<AspNetUsers>(u => u.UserName == HttpContext.Current.User.Identity.Name))
                    });
        }

        public IEnumerable<User> GetOnlineUsers(string roomName)
        {
            return users.Where(u => u.Room == roomName).Select(u => u.User);
        }

        public IEnumerable<string> GetRooms()
        {
            return rooms;
        }

        public void DeleteRoom(string roomName)
        {
            users.RemoveAll(u => u.Room == roomName);
            messages.RemoveAll(m => m.Room == roomName);
            if (rooms.Contains(roomName))
                rooms.Remove(roomName);
        }

        public void CreateRoom(string roomName)
        {
            if (!rooms.Contains(roomName))
                rooms.Add(roomName);
        }
    }

    public class FileChat : IChat
    {
        private string ChatFolder = Path.Combine(Values.Instance.AppDataPath, "Chat");
        private static object lockObj = new object();
        private static List<ChatUser> chatUsers = new List<ChatUser>();
        private string xmlExtension = ".xml";
        private string robotName = "Duch ve stroji";

        public IEnumerable<ChatMessage> GetMessages(string roomName)
        {
            var filePath = Path.Combine(ChatFolder, roomName + xmlExtension);
            var serializer = new XmlSerializer(typeof(List<ChatMessage>));
            lock (lockObj)
            {
                if (File.Exists(filePath))
                    using (TextReader reader = new StreamReader(filePath))
                    {
                        return (List<ChatMessage>)serializer.Deserialize(reader);
                    }
                else
                    return new List<ChatMessage>();
            }
        }

        public void AddMessage(string message, string roomName)
        {
            var filePath = Path.Combine(ChatFolder, roomName + xmlExtension);
            List<ChatMessage> messages = new List<ChatMessage>();
            var serializer = new XmlSerializer(typeof(List<ChatMessage>));
            message = message.ToHtml();
            
            if (string.IsNullOrWhiteSpace(message.Trim()))
                return; // prázdné zprávy zahazovat

            lock (lockObj)
            {
                if (File.Exists(filePath))
                {
                    using (TextReader reader = new StreamReader(filePath))
                    {
                        messages = (List<ChatMessage>)serializer.Deserialize(reader);
                    }
                    if (messages.Count > 90)
                        messages = messages.Skip(10).ToList();
                }

                if (message.StartsWith("!"))
                {
                    #region Special commands
                    if (message == "!vitr")
                    {
                        messages.Clear();
                        messages.Add(RobotMessage("Proběhl vítr"));
                    }
                    else if (message.StartsWith("!text"))
                    {
                        string robotMessage = message.Remove(0, 6);
                        messages.Add(RobotMessage(robotMessage));
                    }
                    else if (message.StartsWith("!datum"))
                    {
                        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("cs-CZ");
                        messages.Add(RobotMessage(string.Format("Dnes je {0}, svátek má {1}. Blahopřejeme", Info.CentralEuropeNow.ToString("dddd dd.MM.yyyy"), Svatek.DnesniSvatek())));
                    }
                    else if (message.StartsWith("!help"))
                    {
                        messages.Add(RobotMessage("Dostupné příkazy: !text, !datum, !help, !info, !vtip"));
                    }
                    else if (message.StartsWith("!info"))
                    {
                        messages.Add(RobotMessage(string.Format("Verze systému: {0}, uvolněna dne {1} {2}",
                            About.Version,
                            About.VersionDate,
                            About.VersionTime)));
                        messages.Add(RobotMessage(string.Format("Mé jméno je \"{0}\", jsem automatický robot sloužící pro vaše pohodlí a zábavu", robotName)));
                    }
                    else if (message.StartsWith("!vtip"))
                    {
                        var repository = DependencyResolver.Current.GetService<IRepository>();
                        var jokesDicsussion = repository.One<Discussion>(d => d.Name == "Vtipy");
                        if (jokesDicsussion == null) return;

                        var id = jokesDicsussion.Id;
                        var jokes = repository
                            .Where<DiscussionItem>(di => di.Id_Discussion == id && di.Text.Length <= 400)
                            .Select(di => di.Text)
                            .ToArray();

                        if (jokes.Any())
                        {
                            // zkusit najit cislo vtipu, kdyz neni, tak vyberu nahodne.. vtip muze byt zadan jako !vtip42 nebo !vtip 42
                            var words = message.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            int jokeNumber = 0;
                            if (words[0] != "!vtip")
                            {
                                string number = words[0].Remove(0, 5);
                                if (!int.TryParse(number, out jokeNumber))
                                    return;
                            }
                            else if (words.Length > 1)
                            {
                                string number = words[1];
                                if (!int.TryParse(number, out jokeNumber))
                                    return;
                            }

                            if (jokeNumber > jokes.Length || jokeNumber < 0)
                            {
                                messages.Add(RobotMessage(string.Format("Neznám vtip s číslem {0}", jokeNumber)));
                            }
                            else
                            {
                                if (jokeNumber == 0)
                                {
                                    Random rnd = new Random();
                                    jokeNumber = rnd.Next(0, jokes.Length) + 1; // +1 abych viděl číslo vtipy
                                }

                                string joke = jokes[jokeNumber - 1]; // -1 protože vtipy jsou číslovány od jedničky
                                joke = joke.Replace("<br />", " ");
                                joke = joke.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
                                joke = joke.Replace(Environment.NewLine, " ");
                                messages.Add(RobotMessage(string.Format("Vtip {0}/{1} - {2}", jokeNumber, jokes.Length, joke)));
                            }
                        }
                        else
                        {
                            messages.Add(RobotMessage("Neznám žádný vtip :-("));
                        }
                    }
                    else if (message.StartsWith("!rekniza"))
                    {
                        string[] words = message.Split(new [] { " " }, StringSplitOptions.None);
                        if (words.Length > 2)
                        {
                            var repository = DependencyResolver.Current.GetService<IRepository>();
                            var user = repository.One<AspNetUsers>(u => u.UserName == words[1]);

                            if (user != null)
                            {
                                messages.Add(new ChatMessage()
                                    {
                                        Color = GetColor(user.UserName),
                                        DateTime = Info.CentralEuropeNow,
                                        Text = string.Join(" ", words.Skip(2)),
                                        UserName = user.UserName,
                                    });
                            }
                        }
                    }
                    else if (message.StartsWith("!obr "))
                    {
                        int startIndex = message.IndexOf(">", StringComparison.Ordinal) + 1;
                        int endIndex = message.IndexOf("<", startIndex, StringComparison.Ordinal);
                        if (startIndex > -1 && endIndex > -1)
                        {
                            var link = message.Substring(startIndex, endIndex - startIndex);
                            using (var client = new WebClient())
                            {
                                var image = Image.FromStream(client.OpenRead(link));
                                if (image.Height <= 400 && image.Width <= 600)
                                {
                                    messages.Add(new ChatMessage()
                                    {
                                        UserName = HttpContext.Current.User.Identity.Name,
                                        Text = string.Format("<img src=\"{0}\" />",
                                            link),
                                        DateTime = Info.CentralEuropeNow,
                                        Color = GetColor(HttpContext.Current.User.Identity.Name),
                                    });
                                }
                            }

                        }
                    }
                    #endregion
                }
                else
                {
                    var lastMessage = messages.Last();
                    if (lastMessage.Text != message || lastMessage.UserName != HttpContext.Current.User.Identity.Name)
                        messages.Add(new ChatMessage()
                        {
                            UserName = HttpContext.Current.User.Identity.Name,
                            Text = message,
                            DateTime = Info.CentralEuropeNow,
                            Color = GetColor(HttpContext.Current.User.Identity.Name),
                        });
                }
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    serializer.Serialize(writer, messages);
                }
            }
        }

        public IEnumerable<User> GetOnlineUsers(string roomName)
        {
            lock (lockObj)
            {
                var chatUser = chatUsers.SingleOrDefault(ch => ch.Room == roomName && ch.User.name == HttpContext.Current.User.Identity.Name);
                if (chatUser == null)
                {
                    // nový uživatel v místnosti
                    var repository = DependencyResolver.Current.GetService<IRepository>();
                    chatUsers.Add(new ChatUser()
                    {
                        DateTime = Info.CentralEuropeNow,
                        Room = roomName,
                        User = new User(repository.One<AspNetUsers>(u => u.UserName == HttpContext.Current.User.Identity.Name))
                    });
                }
                else
                {
                    chatUser.DateTime = Info.CentralEuropeNow;
                }

                chatUsers.RemoveAll(ch => ch.Room == roomName && ch.DateTime < Info.CentralEuropeNow.AddMinutes(-1));
                return chatUsers.Where(ch => ch.Room == roomName).Select(ch => ch.User);
            }
        }

        public IEnumerable<string> GetRooms()
        {
            return Directory.GetFiles(ChatFolder, "*" + xmlExtension);
        }

        public void DeleteRoom(string roomName)
        {
            var filePath = Path.Combine(ChatFolder, roomName + xmlExtension);
            if (File.Exists(filePath))
                File.Delete(filePath);
            lock (lockObj)
                chatUsers.RemoveAll(ch => ch.Room == roomName);
        }

        public void CreateRoom(string roomName)
        {
            var filePath = Path.Combine(ChatFolder, roomName + xmlExtension);
            List<ChatMessage> messages = new List<ChatMessage>();
            var serializer = new XmlSerializer(typeof(List<ChatMessage>));

            lock (lockObj)
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    serializer.Serialize(writer, messages);
                }
            }
        }

        private string GetColor(string username)
        {
            switch (username)
            {
                case "du-šan":
                    return "#0000FF";
                case "Manik":
                    return "#DF3A01";
                case "Lenka":
                    return "#467910";
                case "Bobo":
                    return "#FF4000";
                case "moncakm":
                    return "#FF00BF";
                case "shrek":
                    return "green";
                case "Jana J.":
                    return "#FF8000";
                case "Marťule":
                    return "#6A0888";
                case "Leni":
                    return "#01A9DB";
                case "mnovy":
                    return "#001959";
                case "pvymet":
                    return "#B40431";
                case "TerkaK":
                    return "#B40486";
                case "Nerothar":
                    return "#000080";
                case "Pavel.24":
                    return "#8A0808";
                case "EvikD":
                    return "#00797e";
                case "Majk":
                    return "#FF0000";
                case "Máňa":
                    return "#04B404";
                case "Jana":
                    return "#13E4E8";
                default:
                    return "black";
            }
        }

        private ChatMessage RobotMessage(string text)
        {
            return new ChatMessage()
            {
                Color = "black",
                DateTime = Info.CentralEuropeNow,
                UserName = robotName,
                Text = text,
            };
        }
    }

    class ChatUser
    {
        public string Room { get; set; }
        public User User { get; set; }
        public DateTime DateTime { get; set; }
    }

    class MemoryChatMessage
    {
        public string Room { get; set; }
        public User User { get; set; }
        public DateTime DateTime { get; set; }
        public string Text { get; set; }
    }
}