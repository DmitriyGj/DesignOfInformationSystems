using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgressCheck1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                Console.ResetColor();
                Console.Write("Введите имя пользователя: ");
                try
                {
                    UserService.AddUserByLogin(Console.ReadLine());
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Пользователь успешно добавлен");
                }
                catch(Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                }
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nСписок пользователей:");
                Console.ForegroundColor = ConsoleColor.White;
                foreach (var user in UserService.Users)
                    Console.WriteLine(user.ToString());
                Console.WriteLine();
            }
        }
    }

    public class User
    {
        public readonly string Login;
        
        public User(string login)
        {
            Login = login == string.Empty ? throw new ArgumentException("#ERROR Имя пользователя не может быть пустым"): login;
        }

        public override string ToString() => Login;
    }

   public class UserUnicLoginComparer : IEqualityComparer<User>
    {
        public bool Equals(User user1, User user2)
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals(user1.Login, user2.Login);
        }

        public int GetHashCode(User obj)
        {
            return obj.Login.GetHashCode();
        }
    }


    public static class UserService
    {
        public  static readonly HashSet<User> Users = new HashSet<User>(new UserUnicLoginComparer());

        public static void AddUser(User user)
        {
            var prevUsersCOunt = Users.Count;
            Users.Add(user);
            if (prevUsersCOunt == Users.Count)
                throw new Exception("#ERROR Пользователь с таким логином существует");
        }

        public static void AddUserByLogin(string login) => AddUser(new User(login));
    }
}
