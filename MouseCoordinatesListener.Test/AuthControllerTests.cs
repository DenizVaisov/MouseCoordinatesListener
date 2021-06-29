using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Threading.Tasks;
using FakeItEasy;
using FakeItEasy.Sdk;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MouseCoordinatesListener.Controllers;
using MouseCoordinatesListener.Models;
using Xunit;

namespace MouseCoordinatesListener.Test
{
    public class AuthControllerTests
    {
        [Fact]
        public void SignIn_Find_User_In_Db()
        {
            int count = 1;
            
            User user = new User
            {
                Email = "deniz.vaisov@gmail.com",
                Password = "1234d"
            };

            var fakeLogger = A.Fake<ILogger<AuthController>>();
            var fakeContext = A.Fake<RepositoryContext>();
            var fakeConfig = A.Fake<IConfiguration>();
            
            var controller = new AuthController(fakeContext, fakeConfig, fakeLogger);
            
            var actionResult = controller.SignIn(user);
            var result = actionResult.Result as OkObjectResult;
            var returnUser = result.Value as IEnumerable<User>;
            
            Assert.Equal(count, returnUser.Count());
        }
    }
}