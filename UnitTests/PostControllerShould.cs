using ChallengeIndividual.Controllers;
using ChallengeIndividual.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class PostControllerShould
    {
        

        private async Task<DataContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            DataContext databaseContext = new DataContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.Posts.CountAsync() <= 0)
            {
                for (int i = 1; i <= 10; i++)
                {
                    databaseContext.Posts.Add(new Post()
                    {
                        Id = i,
                        Title = $"Titulo{i}",
                        Article = $"Content{i}",
                        CategoryId = 1,
                        CreatedAt = DateTime.UtcNow,
                        Image = $"Image{i}.jpg",

                    });
                    await databaseContext.SaveChangesAsync();
                }
            }

            return databaseContext;
        }

        [Fact]
        public async Task Return_All_Posts_When_Calling_Index_Without_Parameters()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            using var postController = new PostsController(dbContext, null);
            //Act
            var result = await postController.Index() as ViewResult;
            //Assert
            Assert.True(result.ViewData.Count == 10);


        }

        [Fact]
        public async Task Return_View_When_Calling_Index_Without_Parameters()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            using var postController = new PostsController(dbContext, null);
            //Act
            var result = await postController.Index() as ViewResult;
            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Index");
        }




    }
}
