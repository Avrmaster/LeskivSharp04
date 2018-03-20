using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using LeskivSharp04.Annotations;

namespace LeskivSharp04
{
    public class PersonCreationException : Exception
    {
        public PersonCreationException(string message)
            : base(message)
        {
        }
    }

    public class WrongNameException : PersonCreationException
    {
        public WrongNameException(string message)
            : base(message)
        {
        }
    }

    public class WrongSurnameException : PersonCreationException
    {
        public WrongSurnameException(string message)
            : base(message)
        {
        }
    }

    public class WrongEmailException : PersonCreationException
    {
        public WrongEmailException(string givenEmail)
            : base($"Email {givenEmail} is not valid!")
        {
        }
    }

    public class WrongBirthdayException : PersonCreationException
    {
        public WrongBirthdayException(DateTime birthday)
            : base($"Birthday {birthday.ToShortDateString()} is not valid!")
        {
        }
    }

    [Serializable]
    public class Person
    {
        private const string DataFilepath = "database";
        private const string PersonFileTemplate = "p{0}.bin";

        public string Name { get; private set; }

        public string Surname { get; private set; }

        public string Email { get; private set; }

        public DateTime Birthday { get; private set; }

        public Person(string name, string surname, string email, DateTime birthday)
        {
            if (name.Length < 2)
            {
                throw new WrongNameException($"_name {name} is too small!");
            }

            if (surname.Length < 2)
            {
                throw new WrongSurnameException($"_surname {surname} is too small!");
            }

            if (email.Length < 3 || email.Count(f => f == '@') != 1 ||
                (email.IndexOf("@", StringComparison.Ordinal) == email.Length - 1) ||
                (email.IndexOf("@", StringComparison.Ordinal) == 0))
            {
                throw new WrongEmailException(email);
            }

            var yearsDif = DateTime.Today.YearsPassedCnt(birthday);
            if (yearsDif < 0 || yearsDif > 135)
            {
                throw new WrongBirthdayException(birthday);
            }

            Name = name;
            Surname = surname;
            Email = email;
            Birthday = birthday;
        }

        public Person(string name, string surname, string email) : this(name, surname, email, DateTime.Today)
        {
        }

        public Person(string name, string surname, DateTime birthday) : this(name, surname, "not specified", birthday)
        {
        }

        public bool IsAdult => DateTime.Today.YearsPassedCnt(Birthday) >= 18;
        public bool IsBirthday => DateTime.Today.DayOfYear == Birthday.DayOfYear;

        public string ChineseSign => ChineseZodiaсs[(Birthday.Year + 8) % 12];

        public string SunSign
        {
            get
            {
                var day = Birthday.Day;
                int westZodiacNum;
                switch (Birthday.Month)
                {
                    case 1: //January
                        westZodiacNum = day <= 20 ? 9 : 10;
                        break;
                    case 2: //February
                        westZodiacNum = day <= 19 ? 10 : 11;
                        break;
                    case 3: //March
                        westZodiacNum = day <= 20 ? 11 : 0;
                        break;
                    case 4: //April
                        westZodiacNum = day <= 20 ? 0 : 1;
                        break;
                    case 5: //May
                        westZodiacNum = day <= 20 ? 1 : 2;
                        break;
                    case 6: //June
                        westZodiacNum = day <= 20 ? 2 : 3;
                        break;
                    case 7: //Jule
                        westZodiacNum = day <= 21 ? 3 : 4;
                        break;
                    case 8: //August
                        westZodiacNum = day <= 22 ? 4 : 5;
                        break;
                    case 9: //September
                        westZodiacNum = day <= 21 ? 5 : 6;
                        break;
                    case 10: //October
                        westZodiacNum = day <= 21 ? 6 : 7;
                        break;
                    case 11: //November
                        westZodiacNum = day <= 21 ? 7 : 8;
                        break;
                    case 12: //December
                        westZodiacNum = day <= 21 ? 8 : 9;
                        break;
                    default:
                        westZodiacNum = 0;
                        break;
                }

                return WesternZodiaсs[westZodiacNum];
            }
        }

        public void CopyFrom(Person person)
        {
            Name = person.Name;
            Surname = person.Surname;
            Email = person.Email;
            Birthday = person.Birthday;
        }

        private void SaveTo([NotNull] string filename)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Directory.CreateDirectory(Path.GetDirectoryName(filename) ?? throw new InvalidOperationException());
                Stream stream = new FileStream(path: filename,
                    mode: FileMode.Create,
                    access: FileAccess.Write,
                    share: FileShare.None);
                formatter.Serialize(serializationStream: stream, graph: this);
                stream.Close();
            }
            catch (IOException e)
            {

            }
        }

        private static Person LoadFrom(string filename)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(filename,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);
                var person = (Person)formatter.Deserialize(stream);
                stream.Close();
                return person;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /**
         * Loads the users from an existring 'database' (directory)
         * If it doesn't exist, it's created with 50 default users
         */
        public static async void LoadAllInto(List<Person> persons, Action action)
        {
            await Task.Run(() =>
            {
                if (!Directory.Exists(DataFilepath))
                {
                    Directory.CreateDirectory(DataFilepath);
                    persons.AddRange(PersonSpawner.SpawnPersons(50));
                    SaveAll(persons);
                }
                else
                {
                    persons.AddRange(Directory.EnumerateFiles(DataFilepath).Select(LoadFrom));
                }
                action();
            });
        }

        public static void SaveAll([NotNull] List<Person> persons)
        {
            var i = 0;
            persons.ForEach(delegate (Person p)
            {
                p.SaveTo(Path.Combine(DataFilepath, string.Format(PersonFileTemplate, i++)));
            });
            string extraFile;
            while (File.Exists(extraFile = Path.Combine(DataFilepath, string.Format(PersonFileTemplate, i++))))
                File.Delete(extraFile);
        }

        private static class PersonSpawner
        {
            public static List<Person> SpawnPersons(int count)
            {
                var persons = new List<Person>();
                var random = new Random();
                for (var i = 0; i < count; ++i)
                {
                    var name = Names[random.Next(Names.Length)];
                    var surname = Surnames[random.Next(Surnames.Length)];

                    persons.Add(new Person(name, surname,
                        $"{name}.{surname}@{EmailsEndings[random.Next(EmailsEndings.Length)]}",
                        DateTime.Now.AddYears(-random.Next(10, 80)).AddDays(-random.Next(31))
                            .AddMonths(-random.Next(12))));
                }

                return persons;
            }

            private static readonly string[] Names =
            {
                "Oliver",
                "Harry",
                "George",
                "Jack",
                "Jacob",
                "Noah",
                "Charlie",
                "Muhammed",
                "Thomans",
                "Oscar",
                "William",
                "James",
                "Leo",
                "Alfie",
                "Henry",
                "Joshua",
                "Freddie",
                "Archie",
                "Ethan",
                "Isaac",
                "Alexander",
                "Joseph",
                "Edward",
                "Samuel",
                "Max",
                "Logan",
                "Lucas",
                "Daniel",
                "Theo",
                "Arhur",
                "Mohammed",
                "Harrison",
                "Benjamin",
                "Mason",
                "Finley",
                "Sebastian",
                "Adam",
                "Dylan",
                "Zachary",
                "Riley",
                "Teddy",
                "Theodore",
                "David",
                "Elijah",
                "Jake",
                "Toby",
                "Louie",
                "Reuben",
                "Arlo",
                "Hugo"
            };

            private static readonly string[] Surnames =
            {
                "Smith",
                "Johnson",
                "Williams",
                "Jones",
                "Brown",
                "Davis",
                "Miller",
                "Wilson",
                "Moore",
                "Taylor",
                "Anderson",
                "Thomas",
                "Jackson",
                "White",
                "Harris",
                "Martin",
                "Thompson",
                "Garcia",
                "Martinez",
                "Robinson",
                "Clark",
                "Rodriguez",
                "Lewis",
                "Lee",
                "Walker",
                "Hall",
                "Allen",
                "Young",
                "Hernandez",
                "King",
                "Wright",
                "Lopez",
                "Hill",
                "Scott",
                "Green",
                "Adams",
                "Baker",
                "Gonzalez",
                "Nelson",
                "Carter",
                "Mitchell",
                "Perez",
                "Roberts",
                "Turner",
                "Phillips",
                "Campbell",
                "Parker",
                "Evans",
                "Edwards",
                "Collins"
            };

            private static readonly string[] EmailsEndings =
            {
                "gmail.com",
                "i.ua",
                "yandex.ru",
                "mail.ru"
            };
        }

        private static readonly string[] WesternZodiaсs =
        {
            "Ram",
            "Bull",
            "Twins",
            "Crab",
            "Lion",
            "Virgin",
            "Scales",
            "Scorpion",
            "Archer",
            "Mountain Sea-Goat",
            "Water Bearer",
            "Fish"
        };

        private static readonly string[] ChineseZodiaсs =
        {
            "Rat",
            "Ox",
            "Tiger",
            "Rabbit",
            "Dragon",
            "Snake",
            "Horse",
            "Goat",
            "Monkey",
            "Rooster",
            "Dog",
            "Pig"
        };
    }
}