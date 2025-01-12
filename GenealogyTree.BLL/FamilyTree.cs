using System.Text;

namespace BLL
{
    public class FamilyTree
    {
        public List<Person> People { get; set; } = new();

        public void AddPerson(Person person) => People.Add(person);

        public Person? FindPerson(Guid id) => People.FirstOrDefault(p => p.Id == id);
        public static bool HasRelation(Person person1, Person person2, string relation)
        {
            if (person2.Relations.ContainsKey(relation))
            {
                return person2.Relations[relation].Contains(person1.Id);
            }
            return false;
        }

        public void SetRelation(Guid fromId, Guid toId, string relation)
        {
            var from = FindPerson(fromId);
            var to = FindPerson(toId);
            if (from == null || to == null || !(relation == "parent" || relation == "child" || relation == "spouse"))
            {
                throw new Exception("Incorrect data");
            }

            if (!to.Relations.ContainsKey(relation))
            {
                to.Relations[relation] = new List<Guid>();
            }
            else if (HasRelation(from, to, relation))
            {
                return;
            }

            to.Relations[relation].Add(fromId);

            if (relation == "spouse")
            {
                if (!HasRelation(to, from, "spouse"))
                {
                    SetRelation(toId, fromId, "spouse");
                }
                var childs = GetRelatives(fromId, "child");
                foreach (var ch in childs)
                {
                    SetRelation(toId, ch.Id, "parent");
                }
            }
            else if (relation == "parent")
            {
                if (!HasRelation(to, from, "child"))
                {
                    SetRelation(toId, fromId, "child");
                }
                var spouses = GetRelatives(fromId, "spouse");
                foreach (var sp in spouses)
                {
                    SetRelation(toId, sp.Id, "child");
                }
            }
            else if (relation == "child")
            {
                if (!HasRelation(to, from, "parent"))
                {
                    SetRelation(toId, fromId, "parent");
                }
                var spouses = GetRelatives(toId, "spouse");
                foreach (var sp in spouses)
                {
                    SetRelation(fromId, sp.Id, "child");
                }
            }
        }

        public List<Person> GetRelatives(Guid id, string relation)
        {
            var person = FindPerson(id);
            if (person == null || !person.Relations.ContainsKey(relation)) return new List<Person>();

            return person.Relations[relation].Select(FindPerson).Where(p => p != null).Cast<Person>().ToList();
        }

        public string PrintTree()
        {
            StringBuilder data = new StringBuilder();
            foreach (var person in People)
            {
                data.AppendLine();
                data.AppendLine(person.ToString());
                foreach (var relation in person.Relations)
                {
                    data.AppendLine($"  {relation.Key}: {string.Join(", ", relation.Value.Select(id => FindPerson(id)?.FullName ?? "Unknown"))}");
                }
            }
            return data.ToString();
        }

        public (int Years, int Months, int Days) DetermineAgeAtBirth(Guid fromId, Guid toId)
        {
            var from = FindPerson(fromId);
            var to = FindPerson(toId);
            if (from == null || to == null)
            {
                throw new Exception("Incorrect data");
            }

            if (from.BirthDate > to.BirthDate)
            {
                (from, to) = (to, from);
            }
            int years = to.BirthDate.Year - from.BirthDate.Year;
            int months = to.BirthDate.Month - from.BirthDate.Month;
            int days = to.BirthDate.Day - from.BirthDate.Day;

            if (days < 0)
            {
                months--;
                days += DateTime.DaysInMonth(to.BirthDate.Year, to.BirthDate.Month == 1 ? 12 : to.BirthDate.Month - 1);
            }

            if (months < 0)
            {
                years--;
                months += 12;
            }

            return (years, months, days);

        }

        /*
                private void DisplaySubTree(Person person, string indent, HashSet<Guid> visited)
                {
                    if (visited.Contains(person.Id)) return; // Предотвращаем зацикливание
                    visited.Add(person.Id);

                    // Вывод текущего человека
                    Console.WriteLine($"{indent}├── {person.FullName} ({person.BirthDate:yyyy-MM-dd})");

                    // Супруги
                    if (person.Relations.TryGetValue("spouse", out var spouseIds) && spouseIds.Any())
                    {
                        Console.WriteLine($"{indent}│   Spouses:");
                        foreach (var spouseId in spouseIds)
                        {
                            var spouse = FindPerson(spouseId);
                            if (spouse != null)
                                Console.WriteLine($"{indent}│   ├── {spouse.FullName} ({spouse.BirthDate:yyyy-MM-dd})");
                        }
                    }

                    // Дети
                    if (person.Relations.TryGetValue("child", out var childIds) && childIds.Any())
                    {
                        Console.WriteLine($"{indent}│   Children:");
                        foreach (var childId in childIds)
                        {
                            var child = FindPerson(childId);
                            if (child != null)
                            {
                                DisplaySubTree(child, indent + "│   ", visited);
                            }
                        }
                    }

                    // Родители
                    if (person.Relations.TryGetValue("parent", out var parentIds) && parentIds.Any())
                    {
                        Console.WriteLine($"{indent}│   Parents:");
                        foreach (var parentId in parentIds)
                        {
                            var parent = FindPerson(parentId);
                            if (parent != null)
                                Console.WriteLine($"{indent}│   ├── {parent.FullName} ({parent.BirthDate:yyyy-MM-dd})");
                        }
                    }

                    // Братья/сёстры
                    if (person.Relations.TryGetValue("sibling", out var siblingIds) && siblingIds.Any())
                    {
                        Console.WriteLine($"{indent}│   Siblings:");
                        foreach (var siblingId in siblingIds)
                        {
                            var sibling = FindPerson(siblingId);
                            if (sibling != null)
                                Console.WriteLine($"{indent}│   ├── {sibling.FullName} ({sibling.BirthDate:yyyy-MM-dd})");
                        }
                    }

                    // Дяди/тёти
                    if (person.Relations.TryGetValue("parent", out var parentIdsForUnclesAunts))
                    {
                        foreach (var parentId in parentIdsForUnclesAunts)
                        {
                            var parent = FindPerson(parentId);
                            if (parent == null || !parent.Relations.TryGetValue("sibling", out var uncleAuntIds)) continue;

                            Console.WriteLine($"{indent}│   Uncles/Aunts:");
                            foreach (var uncleAuntId in uncleAuntIds)
                            {
                                var uncleAunt = FindPerson(uncleAuntId);
                                if (uncleAunt != null)
                                    Console.WriteLine($"{indent}│   ├── {uncleAunt.FullName} ({uncleAunt.BirthDate:yyyy-MM-dd})");
                            }
                        }
                    }

                    // Двоюродные братья/сёстры
                    if (person.Relations.TryGetValue("parent", out var parentIdsForCousins))
                    {
                        foreach (var parentId in parentIdsForCousins)
                        {
                            var parent = FindPerson(parentId);
                            if (parent == null || !parent.Relations.TryGetValue("sibling", out var cousinParentIds)) continue;

                            Console.WriteLine($"{indent}│   Cousins:");
                            foreach (var cousinParentId in cousinParentIds)
                            {
                                var cousinParent = FindPerson(cousinParentId);
                                if (cousinParent == null || !cousinParent.Relations.TryGetValue("child", out var cousinIds)) continue;

                                foreach (var cousinId in cousinIds)
                                {
                                    var cousin = FindPerson(cousinId);
                                    if (cousin != null)
                                        Console.WriteLine($"{indent}│   ├── {cousin.FullName} ({cousin.BirthDate:yyyy-MM-dd})");
                                }
                            }
                        }
                    }
                }*/

        public void DisplayTree()
        {
            // Найти корня древа (самого старшего человека)
            var rootPerson = People.OrderBy(p => p.BirthDate).FirstOrDefault();
            if (rootPerson == null)
            {
                Console.WriteLine("The family tree is empty.");
                return;
            }
            Console.WriteLine($"Family Tree (root: {rootPerson.FullName})");
            Console.WriteLine("Legend:\n\t└── : Spouse\n\t├── : Subling\n");
            DisplayGenerations(rootPerson, "", new HashSet<Guid>());
        }
        private void DisplayGenerations(Person rootPerson, string indent, HashSet<Guid> visited)
        {
            if (visited.Contains(rootPerson.Id)) return; // Избегаем зацикливания
            visited.Add(rootPerson.Id);

            // Печать текущего человека
            Console.WriteLine($"{indent}├── {rootPerson.FullName} ({rootPerson.BirthDate:yyyy-MM-dd})");


            // Получение супругов
            if (rootPerson.Relations.TryGetValue("spouse", out var spouseIds) && spouseIds.Any())
            {
                var spouses = spouseIds.Select(FindPerson).Where(s => s != null).ToList();
                if (spouses.Any())
                {
                    //Console.WriteLine($"{indent}    ├── Spouses:");
                    //Console.WriteLine($"{indent}├── Spouses:");
                    foreach (var spouse in spouses)
                    {
                        Console.WriteLine($"{indent}└── {spouse.FullName} ({spouse.BirthDate:yyyy-MM-dd})");
                    }
                }
            }

            // Получение детей текущего человека
            if (rootPerson.Relations.TryGetValue("child", out var childIds) && childIds.Any())
            {
                var children = childIds.Select(FindPerson).Where(c => c != null).ToList();
                if (children.Any())
                {
                    //Console.WriteLine($"{indent}    ├── Children:");
                    foreach (var child in children)
                    {
                        DisplayGenerations(child, indent + "│    ", visited);
                    }
                }
            }
            //// Получение братьев/сестёр
            //if (rootPerson.Relations.TryGetValue("parent", out var parentIds))
            //{
            //    foreach (var parentId in parentIds)
            //    {
            //        var parent = FindPerson(parentId);
            //        if (parent != null && parent.Relations.TryGetValue("child", out var siblingIds))
            //        {
            //            var siblings = siblingIds
            //                .Select(FindPerson)
            //                .Where(s => s != null && s.Id != rootPerson.Id)
            //                .ToList();
            //            if (siblings.Any())
            //            {
            //                Console.WriteLine($"{indent}    ├── Siblings:");
            //                foreach (var sibling in siblings)
            //                {
            //                    Console.WriteLine($"{indent}    ├── {sibling.FullName} ({sibling.BirthDate:yyyy-MM-dd})");
            //                }
            //            }
            //        }
            //    }
            //}
        }

        public void BuildGenealogicalTree()
        {

            if (People == null || People.Count == 0)
            {
                Console.WriteLine("Данные отсутствуют.");
                return;
            }

            // Найти корень дерева (человека с самой ранней датой рождения)
            var root = People.OrderBy(p => p.BirthDate).First();

            // Построить древо
            Console.WriteLine($"{root.FullName} ({root.Gender}, {root.BirthDate:yyyy-MM-dd})");
            BuildTree(root, "", true);
        }

        private void BuildTree(Person person, string indent, bool last)
        {
            string childIndent = indent + (last ? "    " : "│   ");

            foreach (var relation in person.Relations)
            {
                foreach (var relatedId in relation.Value)
                {
                    var relatedPerson = People.FirstOrDefault(p => p.Id == relatedId);
                    if (relatedPerson != null)
                    {
                        string prefix = last ? "└── " : "├── ";
                        Console.WriteLine($"{indent}{prefix}{relation.Key}: {relatedPerson.FullName} ({relatedPerson.Gender}, {relatedPerson.BirthDate:yyyy-MM-dd})");
                        BuildTree(relatedPerson, childIndent, relation.Value.Last() == relatedId);
                    }
                }
            }
        }
        public void BuildGenealogicalTree(List<Person> people)
        {
            if (people == null || people.Count == 0)
            {
                Console.WriteLine("Данные отсутствуют.");
                return;
            }

            // Найти корень дерева (человека с самой ранней датой рождения)
            var root = people.OrderBy(p => p.BirthDate).First();

            // Построить древо
            Console.WriteLine($"{root.FullName} ({root.Gender}, {root.BirthDate:yyyy-MM-dd})");

            var visited = new HashSet<Guid>();
            BuildTree(root, people, "", true, visited);
        }

        private void BuildTree(Person person, List<Person> people, string indent, bool last, HashSet<Guid> visited)
        {
            if (visited.Contains(person.Id))
            {
                //Console.WriteLine($"{indent}└── [Зацикливание: {person.FullName}]");
                return;
            }

            visited.Add(person.Id);

            string childIndent = indent + (last ? "    " : "│   ");

            foreach (var relation in person.Relations)
            {
                foreach (var relatedId in relation.Value)
                {
                    var relatedPerson = people.FirstOrDefault(p => p.Id == relatedId);
                    if (relatedPerson != null)
                    {
                        string prefix = last ? "└── " : "├── ";
                        Console.WriteLine($"{indent}{prefix}{relation.Key}: {relatedPerson.FullName} ({relatedPerson.Gender}, {relatedPerson.BirthDate:yyyy-MM-dd})");
                        BuildTree(relatedPerson, people, childIndent, relation.Value.Last() == relatedId, visited);
                    }
                }
            }
        }


    }
}
