using Xunit;
using HealthSocialMediaApp.Models;
using HealthSocialMediaApp.Data;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework.Options;
using HealthSocialMediaApp.Controllers;
using Microsoft.AspNetCore.Routing;

namespace HealthSocialMediaUnitTest
{
    public class SearchControllerTest
    {
        [Fact]
        public void SearchUserTest()
        {
            //Arrange
            #region context preperation

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            var operationalStoreOptions = Options.Create(
                new OperationalStoreOptions
                {
                    DeviceFlowCodes = new TableConfiguration("DeviceCodes"),
                    PersistedGrants = new TableConfiguration("PersistedGrants")
                });
            var context = new ApplicationDbContext(options, operationalStoreOptions);
            #endregion

            #region data preparation

            var appUserOne = new ApplicationUser
            {
                Email = "SpecificUserName@mail.com",
                UserName = "SpecificUsername01",
                Description = "Bodybuilding guy",
                Id = "specificusername-id"
            };
            context.Users.Add(appUserOne);

            var appUserTwo = new ApplicationUser
            {
                Email = "VerySpecificUsername@mail.com",
                UserName = "VerySpecificUsername999",
                Description = "The best fitness account",
                Id = "veryspecificusername-id"
            };
            context.Users.Add(appUserTwo);

            context.SaveChanges();

            #endregion

            //Act
            SearchController searchController = new SearchController(context);

            string searchPhraseOne = "VerySpecificUsername999";
            string searchPhraseTwo = "specificusername0 ";

            var resultOne = searchController.GetSearchedUsers(searchPhraseOne);
            var resultTwo = searchController.GetSearchedUsers(searchPhraseTwo);

            bool[] arrayOne = SearchUserLoop(searchPhraseOne, resultOne);

            bool onlyOneUserInFirstResult = arrayOne[0];
            bool specificUserInFirstResult = arrayOne[1];

            bool[] arrayTwo = SearchUserLoop(searchPhraseTwo, resultTwo);

            bool onlyOneUserInSecondResult = arrayTwo[0];
            bool specificUserInSecondResult = arrayTwo[1];

            //Assert
            Assert.True(onlyOneUserInFirstResult);
            Assert.True(specificUserInFirstResult);

            Assert.True(onlyOneUserInSecondResult);
            Assert.True(specificUserInSecondResult);
        }

        public bool[] SearchUserLoop(string searchPhrase, System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.IEnumerable>> result)
        {
            bool onlyOneUser = true;
            bool specificUserFound = true;

            foreach (var user in result.Result.Value)
            {
                //code to get parameter of an anonymous object (System.Collections.Generic in Controller made it hard to get specific attribute of the object found).
                //see https://stackoverflow.com/a/14877416 + https://docs.microsoft.com/en-us/dotnet/api/system.web.routing.routevaluedictionary?view=netframework-4.8
                var dictionary = new RouteValueDictionary(user);
                string usernameOfAnonymousObject = dictionary["UserName"] as string;

                if (!usernameOfAnonymousObject.ToLower().Trim().Contains(searchPhrase.ToLower().Trim()))
                {
                    onlyOneUser = false;
                }
                if (usernameOfAnonymousObject.Equals(searchPhrase))
                {
                    specificUserFound = true;
                }

            }

            bool[] arr = new bool[2];
            arr[0] = onlyOneUser;
            arr[1] = specificUserFound;

            return arr;
        }

    }
}