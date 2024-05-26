using FER.InfSus.Time.Api.Controllers;
using FER.InfSus.Time.Application.Response;
using FER.InfSus.Time.Application.UseCases.Taskboard.Commands.AddUser;
using FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Assign;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Create;
using FER.InfSus.Time.Application.UseCases.TaskItem.Dtos;
using FER.InfSus.Time.Application.UseCases.User.Commands.Create;
using FER.InfSus.Time.Application.UseCases.User.Commands.Login;
using FER.InfSus.Time.Application.UseCases.User.Commands.Update;
using FER.InfSus.Time.Application.UseCases.User.Dto;
using FER.InfSus.Time.Domain.Enums;
using FluentAssertions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FER.InfSus.Time.Integration.Tests;

public class AlphabeticalOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
    {
        var result = testCases.ToList();
        result.Sort(
            (x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
        return result;
    }
}

[TestCaseOrderer("FER.InfSus.Time.Integration.Tests.AlphabeticalOrderer", "FER.InfSus.Time.Integration.Tests")]
public class TimeIntegrationTest : IClassFixture<WebHostTestBase>
{
    private readonly WebHostTestBase _testBase;

    public JsonSerializerOptions JsonOptions { get; set; }
        = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

    public TimeIntegrationTest(WebHostTestBase fixture)
    {
        _testBase = fixture;
    }

    [Fact]
    public async Task Test01_Login()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/Authentication/Login");
        request.Content = JsonContent.Create(new UserLoginCommand { Email = "user@time.com", Password = "Pa$$w0rd" });

        // Act
        var response = await _testBase.Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().NotBeNullOrEmpty();
        var responseDto = JsonSerializer.Deserialize<LoggedInUserDto>(responseString, JsonOptions);
        responseDto.Should().NotBeNull();
        responseDto!.Token.Should().NotBeNullOrEmpty();
        _testBase.JwToken = responseDto.Token;
        _testBase.CachedValues.Add("JwToken", responseDto.Token);
        _testBase.CachedValues.Add("UserId", responseDto.Id.ToString());
    }

    [Fact]
    public async Task Test02_UdateUser()
    {
        // Arrange
        var request = new HttpRequestMessage(
            HttpMethod.Put,
            $"/api/TenantManagement/updateUser/{_testBase.CachedValues["UserId"]}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testBase.JwToken);
        request.Content = JsonContent.Create(new UserUpdateCommand
            {
                RequestorId = Guid.Parse(_testBase.CachedValues["UserId"]!),
                Id = Guid.Parse(_testBase.CachedValues["UserId"]!),
                FirstName = "FER",
                LastName = "INFSUS",
                DateOfBirth = DateTime.Now,
                NewPassword = null,
                UserType = UserType.ADMIN
            });

        // Act
        var response = await _testBase.Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Test03_CreateUser()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/TenantManagement/createUser");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testBase.JwToken);
        request.Content = JsonContent.Create(new UserRegistrationCommand
            {
                RequestorId = Guid.Parse(_testBase.CachedValues["UserId"]!),
                FirstName = "Second",
                LastName = "User",
                DateOfBirth = DateTime.Now.AddYears(-20),
                Email = "seconduser@time.com",
                Password = "Pa$$w0rd",
                UserType = UserType.USER
            });

        // Act
        var response = await _testBase.Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Test04_GetUsers()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/TenantManagement/getUsers");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testBase.JwToken);

        // Act
        var response = await _testBase.Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().NotBeNullOrEmpty();
        var responseDto = JsonSerializer.Deserialize<PaginatedResponse<UserDto>>(responseString, JsonOptions);
        responseDto.Should().NotBeNull();
        responseDto!.Data.Should().NotBeNullOrEmpty();
        responseDto.Data.Count().Should().Be(2);
        responseDto.Data.First(t => t.FirstName != "Second").FirstName.Should().Be("FER");
        var secondUser = responseDto.Data.First(t => t.FirstName == "Second");
        secondUser.LastName.Should().Be("User");
        _testBase.CachedValues.Add("SecondUserId", secondUser.Id.ToString());
    }

    [Fact]
    public async Task Test05_GetTaskboards()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Taskboard/all");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testBase.JwToken);

        // Act
        var response = await _testBase.Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().NotBeNullOrEmpty();
        var responseDto = JsonSerializer.Deserialize<List<TaskboardSimpleDto>>(responseString, JsonOptions);
        responseDto.Should().NotBeNull();
        responseDto!.Count.Should().Be(1);
        _testBase.CachedValues.Add("TaskboardId", responseDto![0].Id.ToString());
    }

    [Fact]
    public async Task Test06_AssignOtherUserToTaskboard()
    {
        // Arrange
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/Taskboard/assign");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testBase.JwToken);
        request.Content = JsonContent.Create(new TaskboardAddUserCommand()
            {
                RequestorId = Guid.Parse(_testBase.CachedValues["UserId"]!),
                TaskboardId = Guid.Parse(_testBase.CachedValues["TaskboardId"]!),
                UserId = Guid.Parse(_testBase.CachedValues["SecondUserId"]!)
            });

        // Act
        var response = await _testBase.Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Test07_CreateTask()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/TaskItem");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testBase.JwToken);
        request.Content = JsonContent.Create(new TaskItemCreateCommand()
            {
                RequestorId = Guid.Parse(_testBase.CachedValues["UserId"]!),
                TaskboardId = Guid.Parse(_testBase.CachedValues["TaskboardId"]!),
                Name = "Task 1",
                Description = "This is a task",
            });

        // Act
        var response = await _testBase.Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().NotBeNullOrEmpty();
        var responseDto = JsonSerializer.Deserialize<TaskItemSimpleDto>(responseString, JsonOptions);
        responseDto.Should().NotBeNull();
        _testBase.CachedValues.Add("TaskId", responseDto!.Id.ToString());
    }

    [Fact]
    public async Task Test08_AssignTaskToUser()
    {
        // Arrange
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/TaskItem/{_testBase.CachedValues["TaskId"]}/assign");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testBase.JwToken);
        request.Content = JsonContent.Create(new TaskItemAssignCommand()
            {
                RequestorId = Guid.Parse(_testBase.CachedValues["UserId"]!),
                Id = Guid.Parse(_testBase.CachedValues["TaskId"]!),
                AssignedUserId = Guid.Parse(_testBase.CachedValues["SecondUserId"]!)
            });

        // Act
        var response = await _testBase.Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
