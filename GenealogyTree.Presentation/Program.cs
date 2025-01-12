using BLL;
using DAL;

namespace ConsoleApp6
{
    internal class Program
    {
        static FamilyTree familyTree = DataAccess.LoadTree();
        static Person person = null;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Создать сущность “Человек”");
                Console.WriteLine("2. Добавить сущность в древо");
                Console.WriteLine("3. Установить отношение");
                Console.WriteLine("4. Вывести ближайших родственников");
                Console.WriteLine("5. Показать получившееся древо");
                Console.WriteLine("6. Вычислить возраст предка при рождении потомка");
                Console.WriteLine("7. Открыть существующее или создать новое древо");
                Console.WriteLine("8. Визуализация генеалогического древа в графическом виде");
                Console.WriteLine("9. Сохранить древо");
                Console.WriteLine("*. Сохранить древо");
                Console.WriteLine("0. Выход\n");

                DisplayTree();

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": CreatePerson(); break;
                    case "2": AddPerson(); break;
                    case "3": SetRelation(); break;
                    case "4": ViewRelatives(); break;
                    case "5": ViewTree(); break;
                    case "6": DetermineAgeAtBirth(); break;
                    case "7": OpenOrCreateNewTree(); break;
                    case "8": DisplayTree(); break;
                    case "9": DisplayTree(); break;
                    case "*": DataAccess.SaveTree(familyTree); break;
                    case "0": return;
                    default: Console.WriteLine("Неверный выбор!"); break;
                }

                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        static void PrintPersons(List<Person> people = null)
        {
            if (people == null)
            {
                people = familyTree.People;
            }
            Console.WriteLine("\n№\tID\t\t\t\t\t Name\t\t\t (Gender, BirthDate)");
            var i = 1;
            foreach (var p in people)
            {
                Console.WriteLine(i + ".\t" + p.ToString());
                i++;
            }
            Console.WriteLine();
        }
        static void CreatePerson()
        {
            try
            {
                Console.Write("Введите полное имя: ");
                var name = Console.ReadLine();
                Console.Write("Введите дату рождения (yyyy-MM-dd): ");
                var birthDate = DateTime.Parse(Console.ReadLine() ?? string.Empty);
                Console.Write("Введите пол (W или M): ");
                var gender = Console.ReadLine().ToUpper();
                if (gender == "W" || gender == "WOMAN" || gender == "Ж" || gender == "ЖЕНЩИНА")
                {
                    gender = "Woman";
                }
                else if (gender == "M" || gender == "MAN" || gender == "М" || gender == "МУЖЧИНА")
                {
                    gender = "Man";
                }
                else
                {
                    Console.WriteLine("Неверные данные");
                    return;
                }
                person = new Person(name, birthDate, gender);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                person = null;
            }
        }
        static void AddPerson()
        {
            if (person == null)
            {
                Console.WriteLine("Сначала создайте человека!");
                return;
            }
            familyTree.AddPerson(person);
            Console.WriteLine($"Человек {person.FullName} добавлен с ID {person.Id}");
            person = null;
        }
        static void SetRelation()
        {
            try
            {
                PrintPersons();
                Console.Write("Выберите номер человека 1: ");
                int.TryParse(Console.ReadLine(), out int fromInt);
                Console.Write("Выберите номер человека 2: ");
                int.TryParse(Console.ReadLine(), out int toInt);
                Console.Write("Выберите связь parent(p), child(c), spouse(s): ");
                var relation = Console.ReadLine().ToLower();
                if (relation == "p" || relation == "parent" || relation == "р" || relation == "родитель")
                {
                    relation = "parent";
                }
                else if (relation == "c" || relation == "child" || relation == "д" || relation == "дитя")
                {
                    relation = "child";
                }
                else if (relation == "s" || relation == "spouse" || relation == "с" || relation == "супруг")
                {
                    relation = "spouse";
                }
                else
                {
                    Console.WriteLine("Неверные данные");
                    return;
                }
                var count = familyTree.People.Count + 1;
                if (fromInt > 0 && fromInt < count && toInt > 0 && toInt < count)
                {
                    Console.WriteLine("\nВсё верно?");
                    var fromId = familyTree.People[fromInt - 1].Id;
                    var toId = familyTree.People[toInt - 1].Id;
                    Console.WriteLine($"{familyTree.People[fromInt - 1].FullName} {relation} {familyTree.People[toInt - 1].FullName}");
                    Console.WriteLine("yes(y), no(n), reverse(r)");
                    var confirmation = Console.ReadLine().ToLower();
                    if (confirmation == "n" || confirmation == "no" || confirmation == "н" || confirmation == "нет")
                    {
                        Console.WriteLine("Отмена сохранения");
                        return;
                    }
                    else if (confirmation == "r" || confirmation == "reverse" || confirmation == "р")
                    {
                        (fromId, toId) = (toId, fromId);
                    }
                    familyTree.SetRelation(fromId, toId, relation);
                    Console.WriteLine("Отношение установлено!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static void ViewRelatives()
        {
            try
            {
                PrintPersons();
                Console.Write("Выберите человека: ");
                int.TryParse(Console.ReadLine(), out int idInt);
                Console.Write("Выберите связь parent(p), child(c), spouse(s): ");
                var relation = Console.ReadLine().ToLower();
                if (relation == "p" || relation == "parent" || relation == "р" || relation == "родитель")
                {
                    relation = "parent";
                }
                else if (relation == "c" || relation == "child" || relation == "д" || relation == "дитя")
                {
                    relation = "child";
                }
                else if (relation == "s" || relation == "spouse" || relation == "с" || relation == "супруг")
                {
                    relation = "spouse";
                }
                else
                {
                    Console.WriteLine("Неверные данные");
                    return;
                }
                if (idInt > 0 && idInt < familyTree.People.Count + 1)
                {
                    var id = familyTree.People[idInt - 1].Id;
                    var relatives = familyTree.GetRelatives(id, relation);
                    PrintPersons(relatives);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static void ViewTree()
        {
            Console.WriteLine(familyTree.PrintTree());
        }
        static void DetermineAgeAtBirth()
        {
            try
            {
                PrintPersons();
                Console.Write("Выберите номер человека 1: ");
                int.TryParse(Console.ReadLine(), out int fromInt);
                Console.Write("Выберите номер человека 2: ");
                int.TryParse(Console.ReadLine(), out int toInt);
                var count = familyTree.People.Count + 1;

                if (fromInt > 0 && fromInt < count && toInt > 0 && toInt < count)
                {
                    var fromId = familyTree.People[fromInt - 1].Id;
                    var toId = familyTree.People[toInt - 1].Id;
                    var res = familyTree.DetermineAgeAtBirth(fromId, toId);
                    Console.WriteLine($"Возраст предка при рождении потомка составляет {res.Years} лет, {res.Months} месяцев, {res.Days} дней");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static void OpenOrCreateNewTree()
        {
            Console.WriteLine("Введите существующее имя для открытия файла или несуществующее для создания нового: ");
            var nameFile = Console.ReadLine();
            DataAccess.SetEndFilePath(nameFile);
            familyTree = DataAccess.LoadTree();
        }
        static void DisplayTree()
        {
            var people = familyTree.People;
            var rootPerson = people.OrderBy(p => p.BirthDate).FirstOrDefault();
            if (rootPerson == null)
            {
                Console.WriteLine("Неверные входные данные!");
                return;
            }
            Console.WriteLine($"\nФамильное древо (Корень: {rootPerson.FullName})");
            Console.WriteLine("Legend:\n\t└── : Spouse\n\t├── : Subling\n");
            DisplayGenerations(rootPerson, "", new HashSet<Guid>());
        }
        private static void DisplayGenerations(Person rootPerson, string indent, HashSet<Guid> visited)
        {
            if (visited.Contains(rootPerson.Id))
            {
                return;
            }
            visited.Add(rootPerson.Id);

            Console.WriteLine($"{indent}├── {rootPerson.FullName} ({rootPerson.BirthDate:yyyy-MM-dd})");

            if (rootPerson.Relations.TryGetValue("spouse", out var spouseIds) && spouseIds.Any())
            {
                var spouses = spouseIds.Select(familyTree.FindPerson).Where(s => s != null).ToList();
                if (spouses.Any())
                {
                    foreach (var spouse in spouses)
                    {
                        Console.WriteLine($"{indent}└── {spouse.FullName} ({spouse.BirthDate:yyyy-MM-dd})");
                    }
                }
            }

            if (rootPerson.Relations.TryGetValue("child", out var childIds) && childIds.Any())
            {
                var children = childIds.Select(familyTree.FindPerson).Where(c => c != null).ToList();
                if (children.Any())
                {
                    foreach (var child in children)
                    {
                        DisplayGenerations(child, indent + "│    ", visited);
                    }
                }
            }
        }
    }
}
