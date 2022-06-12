﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sat.Recruitment.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class UsersController : ControllerBase
    {

        private readonly List<User> _users = new List<User>();

        public UsersController()
        {
        }

        [HttpPost]
        [Route("/create-user")]
        public async Task<Result> CreateUser(string name, string email, string address, string phone, string userType, string money)
        {
            var errors = "";

            ValidateErrors(name, email, address, phone, ref errors);

            if (errors != null && errors != "")
            {
                return new Result()
                {
                    IsSuccess = false,
                    Errors = errors
                };
            }

            var newUser = new User
            {
                Name = name,
                Email = email,
                Address = address,
                Phone = phone,
                UserType = userType,
                Money = decimal.Parse(money)
            };

            newUser.Money += CalculateUserGif(newUser.UserType, decimal.Parse(money));
            newUser.Email = NormalizeEmail(newUser.Email);

            try
            {
                ValidateUserAlreadyExists(newUser);            
            }
            catch
            {
                Debug.WriteLine("The user is duplicated");
                return new Result()
                {
                    IsSuccess = false,
                    Errors = "The user is duplicated"
                };
            }
            Debug.WriteLine("User Created");

            return new Result()
            {
                IsSuccess = true,
                Errors = "User Created"
            };
        }

        //Validate errors
        private void ValidateErrors(string name, string email, string address, string phone, ref string errors)
        {
            if (name == null)
                //Validate if Name is null
                errors = "The name is required";
            if (email == null)
                //Validate if Email is null
                errors = errors + " The email is required";
            if (address == null)
                //Validate if Address is null
                errors = errors + " The address is required";
            if (phone == null)
                //Validate if Phone is null
                errors = errors + " The phone is required";
        }

        private string NormalizeEmail (string email)
        {
            var aux = email.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

            var atIndex = aux[0].IndexOf("+", StringComparison.Ordinal);

            aux[0] = atIndex < 0 ? aux[0].Replace(".", "") : aux[0].Replace(".", "").Remove(atIndex);

            return string.Join("@", new string[] { aux[0], aux[1] });
        }

        private void LoadUserList()
        {
            var reader = ReadUsersFromFile();

            while (reader.Peek() >= 0)
            {
                var line = reader.ReadLineAsync().Result;
                var user = new User
                {
                    Name = line.Split(',')[0].ToString(),
                    Email = line.Split(',')[1].ToString(),
                    Phone = line.Split(',')[2].ToString(),
                    Address = line.Split(',')[3].ToString(),
                    UserType = line.Split(',')[4].ToString(),
                    Money = decimal.Parse(line.Split(',')[5].ToString()),
                };
                _users.Add(user);
            }
            reader.Close();
        }

        private decimal CalculateUserGif(string userType, decimal currentMoney)
        {
            var gif = decimal.Zero;
            if (userType == "Normal")
            {
                if (currentMoney > 100)
                {
                    var percentage = Convert.ToDecimal(0.12);
                    //If new user is normal and has more than USD100
                    gif = currentMoney * percentage;
                }
                if (currentMoney < 100)
                {
                    if (currentMoney > 10)
                    {
                        var percentage = Convert.ToDecimal(0.8);
                        gif = currentMoney * percentage;
                    }
                }
            }
            if (userType == "SuperUser")
            {
                if (currentMoney > 100)
                {
                    var percentage = Convert.ToDecimal(0.20);
                    gif = currentMoney * percentage;
                }
            }
            if (userType == "Premium")
            {
                if (currentMoney > 100)
                {
                    gif = currentMoney * 2;
                }
            }

            return gif;
        }

        private void ValidateUserAlreadyExists(User newUser)
        {
            LoadUserList();
            foreach (var user in _users)
            {
                if (user.Email == newUser.Email
                    ||
                    user.Phone == newUser.Phone)
                {
                    throw new Exception("User is duplicated");
                }
                else if (user.Name == newUser.Name)
                {
                    if (user.Address == newUser.Address)
                    {
                        throw new Exception("User is duplicated");
                    }
                }
            }
        }
    }
}